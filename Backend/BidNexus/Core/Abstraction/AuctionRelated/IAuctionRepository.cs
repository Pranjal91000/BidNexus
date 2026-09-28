using Core.Entities.Auction;
using Core.Models.AuctionRelated;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Abstraction.AuctionRelated
{
    public interface IAuctionRepository
    {
        public Task<AuctionResponseModel> AddAsync(Auction input);
        public Task<AuctionResponseModel> UpdateAsync(Auction input);
        public Task<AuctionDataModel> GetById(int Id);
        public Task<List<AuctionGetDataModel>> Get(short categoryId, short pageNo, short pageSize);
        public Task<bool> DeleteAsync(int Id);

    }
}
