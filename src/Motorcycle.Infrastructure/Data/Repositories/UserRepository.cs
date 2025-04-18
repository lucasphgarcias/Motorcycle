using Microsoft.EntityFrameworkCore;
using Motorcycle.Domain.Entities;
using Motorcycle.Domain.Interfaces.Repositories;
using Motorcycle.Infrastructure.Data.Context;
using System.Threading;
using System.Threading.Tasks;

namespace Motorcycle.Infrastructure.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MotorcycleDbContext _context;

        public UserRepository(MotorcycleDbContext context)
        {
            _context = context;
        }

        public async Task<UserEntity?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
        }

        public async Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<UserEntity> AddAsync(UserEntity user, CancellationToken cancellationToken = default)
        {
            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return user; // Retorne o usuário adicionado
        }

        public async Task UpdateAsync(UserEntity user, CancellationToken cancellationToken = default)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}