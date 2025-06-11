using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fitscan_DBL.Context;
using Fitscan_DBL.Entities;
using Fitscan_DBL.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;



namespace Fitscan_DBL.Repositories
{

    public class UserRepository : IUserRepository
    {

        private readonly FitscanContext _context;

        public UserRepository(FitscanContext context)
        {
            _context = context;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

    }
}
