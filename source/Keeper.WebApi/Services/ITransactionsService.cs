using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Keeper.WebApi.Models;
using Microsoft.AspNetCore.Identity;

namespace Keeper.WebApi.Services
{
    public interface ITransactionsService
    {
        Task<int> AddAsync(string id, string newTransaction);
        IEnumerable<Transaction> GetTransactionsFrom(string userId, int id);
    }
}