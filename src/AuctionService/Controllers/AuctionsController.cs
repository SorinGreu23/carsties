using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionsController : ControllerBase
{
    private readonly AuctionDbContext _context;
    private readonly IMapper _mapper;

    public AuctionsController(AuctionDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAuctions()
    {
        var auctions = await _context.Auctions
            .Include(x => x.Item)
            .OrderBy(x => x.Item.Make)
            .ToListAsync();

        return _mapper.Map<List<AuctionDto>>(auctions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuction(Guid id)
    {
        var auction = await _context.Auctions
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (auction == null)
            return NotFound();

        return _mapper.Map<AuctionDto>(auction);
    }

    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDto)
    {
        var auction = _mapper.Map<Auction>(auctionDto);
        //TODO: add current user as seller
        auction.Seller = "test";
        
        _context.Auctions.Add(auction);

        var result = await _context.SaveChangesAsync() > 0;

        if (!result) return BadRequest("Could not save changes for creating the auction");
        return CreatedAtAction(nameof(GetAuction), new { auction.Id }, _mapper.Map<AuctionDto>(auction));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
    {
        var auction = await _context.Auctions.Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (auction == null)
            return NotFound();
        
        //TODO: check seller is current user
        
        var updatedItem = new Item
        {
            Make = updateAuctionDto.Make ?? auction.Item.Make,
            Model = updateAuctionDto.Model ?? auction.Item.Make,
            Color = updateAuctionDto.Color ?? auction.Item.Color,
            Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage,
            Year = updateAuctionDto.Year ?? auction.Item.Year
        };

        auction.Item = updatedItem;

        var result = await _context.SaveChangesAsync() > 0;

        if (result)
        {
            return Ok();
        }

        return BadRequest("Could not save changes while updating the auction item");
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await _context.Auctions.FindAsync(id);
        
        if (auction == null)
        {
            return NotFound();
        }
        
        //TODO: check if username matches seller's name

        _context.Auctions.Remove(auction);

        var result = await _context.SaveChangesAsync() > 0;
        
        if (result != true)
            return BadRequest("Could not delete auction");
        
        return Ok();
    }
}