using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Keeper.WebApi.Models;
using Microsoft.AspNetCore.Identity;

namespace Keeper.WebApi.Services
{
    public class TransactionsService : ITransactionsService
    {
        private DatabaseContext context;

        public TransactionsService(DatabaseContext _context)
        {
            context = _context;
        }

        public async Task<int> AddAsync(string id, string newTransaction)
        {
            if(id is null)
                throw new ArgumentNullException(nameof(id));
            var added = context.Transactions.Add(new Transaction()
            {
                Data = newTransaction,
                ProfileId = id
            });
            var result = await context.SaveChangesAsync().ConfigureAwait(false);
            if(result <= 0)
                return result;
            return added.Entity.Id;
        }

        public IEnumerable<Transaction> GetTransactionsFrom(string userId, int id) =>
            context.Transactions.Where(t => t.ProfileId == userId && t.Id > id).ToList();
    }
}