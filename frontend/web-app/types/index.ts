export type PagedResult<T> = {
  results: T[];
  pageCount: number;
  totalCount: number;
}

export type Auction = {
  id: string;
  make: string;
  model: string;
  year: number;
  color: string;
  mileage: number;
  imageUrl: string;
  reservePrice?: number;
  seller: string;
  winner?: any;
  soldAmount?: number;
  currentHighBid?: number;
  status: string;
  createdAt: string;
  updatedAt: string;
  auctionEnd: string;
};