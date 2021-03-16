using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ProgramB.Model;

namespace ProgramB.Repositories
{
    public class HandRepository : IHandRepository
    {
        private readonly AppDbContext _dbContext;
        private object saveLock = new object();
        public HandRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Hand GetHandById(string handId)
        {
            return _dbContext.Hands.FirstOrDefault(h => h.HandId == handId);
        }

        public async Task<IEnumerable<Hand>> GetAllAsync()
        {
            return await _dbContext.Hands.ToListAsync();
        }

        public void Add(Hand hand)
        {
            lock (saveLock)
            {
                _dbContext.Hands.Add(hand);
            }
        }

        public void Save()
        {
            lock (saveLock)
            {
                _dbContext.SaveChanges();
            }
        }
    }
}
