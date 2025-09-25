using Microsoft.EntityFrameworkCore;
using Stripe_Integration.Context;

namespace Stripe_Integration.Repositories
{
    public class Repository<T> where T: class
    {
        private readonly StripeDbContext _context;
        public Repository(StripeDbContext context)
        {
            _context = context;
        }
        public async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            //await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<T> GetById(int id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if(entity is null)
            {
                throw new ArgumentException($"Couldn't find an entity with ID: {id}");
            }
            else
            {
                return entity;
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public void UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            //_context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
