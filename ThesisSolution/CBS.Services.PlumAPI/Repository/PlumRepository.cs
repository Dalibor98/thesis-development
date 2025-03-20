using CBS.Services.PlumAPI.Data;
using CBS.Services.PlumAPI.Models;
using CBS.Services.PlumAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace CBS.Services.PlumAPI.Repository
{
    public class PlumRepository : IPlumRepository
    {
        private readonly PlumDbContext _db;

        public PlumRepository(PlumDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<IEnumerable<Plum>> GetAllPlumsAsync()
        {
            return await _db.Plums.ToListAsync();
        }

        public async Task<Plum> GetPlumByIdAsync(int id)
        {
            //no validation for id stiill
            return await _db.Plums.FirstOrDefaultAsync(p => p.PlumId == id);
        }

        public async Task<Plum> CreatePlumAsync(Plum plum)
        {
            if (plum == null)
            {
                throw new ArgumentNullException(nameof(plum));
            }

            _db.Plums.Add(plum);
            await _db.SaveChangesAsync();
            return plum;
        }

        public async Task<Plum> UpdatePlumAsync(Plum plum)
        {
            if (plum == null)
            {
                throw new ArgumentNullException(nameof(plum));
            }

            var existingPlum = await _db.Plums.FindAsync(plum.PlumId);

            if (existingPlum == null)
            {
                throw new InvalidOperationException($"Plum with ID {plum.PlumId} not found.");
            }

            //_db.Plums.Update(plum); line bellow has better efficiency
            _db.Entry(existingPlum).CurrentValues.SetValues(plum);
            await _db.SaveChangesAsync();
            return existingPlum;
        }

        public async Task<bool> DeletePlumAsync(int id)
        {
            var plum = await _db.Plums.FindAsync(id);

            if (plum != null)
            {
                _db.Plums.Remove(plum);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }       
    }
}
