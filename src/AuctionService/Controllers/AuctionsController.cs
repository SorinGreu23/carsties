using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionsController : ControllerBase
{
    private readonly AuctionDbContext _context;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;
    
    public AuctionsController(AuctionDbContext context, IMapper mapper, 
        IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
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
    [Authorize]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDto)
    {
        var auction = _mapper.Map<Auction>(auctionDto);
        if (User.Identity != null) auction.Seller = User.Identity.Name;

        _context.Auctions.Add(auction);
        
        var newAuction = _mapper.Map<AuctionDto>(auction);

        await _publishEndpoint.Publish(_mapper.Map<AuctionCreated>(newAuction));

        var result = await _context.SaveChangesAsync() > 0;
        
        if (!result) return BadRequest("Could not save changes for creating the auction");
        
        return CreatedAtAction(nameof(GetAuction), new { auction.Id }, newAuction);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
    {
        var auction = await _context.Auctions.Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (auction == null)
            return NotFound();

        if (User.Identity is { Name: not null } && auction.Seller != User.Identity.Name) return Forbid();
        
        var updatedItem = new Item
        {
            Make = updateAuctionDto.Make ?? auction.Item.Make,
            Model = updateAuctionDto.Model ?? auction.Item.Make,
            Color = updateAuctionDto.Color ?? auction.Item.Color,
            Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage,
            Year = updateAuctionDto.Year ?? auction.Item.Year
        };

        auction.Item = updatedItem;
        
        await _publishEndpoint.Publish(_mapper.Map<AuctionUpdated>(auction));

        var result = await _context.SaveChangesAsync() > 0;

        if (result)
        {
            return Ok();
        }

        return BadRequest("Could not save changes while updating the auction item");
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await _context.Auctions.FindAsync(id);
        
        if (auction == null)
        {
            return NotFound();
        }
        
        if (User.Identity != null && auction.Seller != User.Identity.Name) return Forbid();
        
        _context.Auctions.Remove(auction);

        await _publishEndpoint.Publish<AuctionDeleted>(new
        {
            Id = auction.Id.ToString()
        });
        
        var result = await _context.SaveChangesAsync() > 0;
        
        if (result != true)
            return BadRequest("Could not delete auction");
        
        return Ok();
    }
}