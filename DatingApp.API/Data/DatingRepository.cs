using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
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

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await context.Likes.FirstOrDefaultAsync(u => u.LikerId == userId
                && u.LikeeId == recipientId);
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await context.Photos.Where(u=>u.UserId == userId).FirstOrDefaultAsync(p=>p.IsMain);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo= await context.Photos.FirstOrDefaultAsync(p=>p.Id == id);
            return photo;
        }
        public async Task<User> GetUser(int id)
        {
            var user = await context.Users.Include(x=>x.Photos).FirstOrDefaultAsync(x=>x.Id==id);
            return user;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
           var users = context.Users.Include(x=>x.Photos).AsQueryable();
           users = users.Where(u => u.Id != userParams.UserId);
           users = users.Where(u => u.Gender == userParams.Gender);

            if(userParams.Likees)
            {
                var userLikers = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(u => userLikers.Contains(u.Id));
            }
            if(userParams.Likers)
            {
                var userLikees = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(u => userLikees.Contains(u.Id));
            }

           if(userParams.MinAge != 18 || userParams.MaxAge != 99)
           {
               var minDob = DateTime.Today.AddYears(-userParams.MaxAge -1);
               var maxDob = DateTime.Today.AddYears(userParams.MinAge);

               users = users.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
           }
           return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
        {
            var users = await context.Users
            .Include(u=> u.Likers)
            .Include(u=>u.Likees)
            .FirstOrDefaultAsync(x=> x.Id ==id);

            if(likers)
            {
                return users.Likers.Where(u => u.LikeeId == id).Select(i => i.LikerId);
            }
            else 
            {
                return users.Likees.Where(u => u.LikerId == id).Select(i => i.LikeeId);
            }
        }
        public async Task<bool> SaveAll()
        {
            return await context.SaveChangesAsync() > 0;
        }
    }
}