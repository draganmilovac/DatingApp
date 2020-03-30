using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private DataContext context;
        public DatingRepository(DataContext context)
        {
            this.context = context;
        }
        public void Add<T>(T entity) where T : class
        {
            context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            context.Remove(entity);
        }

        public async Task<User> GetUser(int id)
        {
            var user = await context.Users.Include(x=>x.Photos).FirstOrDefaultAsync(x=>x.Id==id);
            return user;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
           var users = await context.Users.Include(x=>x.Photos).ToListAsync();
           return users;
        }

        public async Task<bool> SaveAll()
        {
            return await context.SaveChangesAsync() > 0;
        }
    }
}