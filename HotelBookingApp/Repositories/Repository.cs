
using HotelBookingApp.Models;
using HotelBookingAppWebApi.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingApp.Repositories
{
    public class Repository<K, T> : IRepository<K, T> where T : class
    {
        private readonly HotelBookingContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(HotelBookingContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        // ================= GET ALL =================

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        // ================= GET BY ID =================

        public async Task<T?> GetByIdAsync(K id)
        {
            return await _dbSet.FindAsync(id);
        }

        // ================= ADD =================

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        // ================= UPDATE =================

        public async Task<T?> UpdateAsync(K id, T entity)
        {
            var existing = await _dbSet.FindAsync(id);

            if (existing == null)
                return null;

            _context.Entry(existing).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();

            return existing;
        }

        // ================= DELETE =================

        public async Task<T?> DeleteAsync(K id)
        {
            var entity = await _dbSet.FindAsync(id);

            if (entity == null)
                return null;

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();

            return entity;
        }
    }
}