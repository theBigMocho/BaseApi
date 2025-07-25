using BaseApi.Data;
using BaseApi.DTOs;
using BaseApi.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace BaseApi.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _context.Users
                .OrderBy(u => u.Username)
                .ToListAsync();

            return users.Select(u => MapToUserDto(u));
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return user != null ? MapToUserDto(user) : null;
        }

        public async Task<UserDto?> CreateUserAsync(CreateUserDto createUserDto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == createUserDto.Username))
            {
                return null;
            }

            if (await _context.Users.AnyAsync(u => u.Email == createUserDto.Email))
            {
                return null;
            }

            var user = new User
            {
                Username = createUserDto.Username,
                Email = createUserDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password),
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                IsActive = createUserDto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return MapToUserDto(user);
        }

        public async Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(updateUserDto.Username) && updateUserDto.Username != user.Username)
            {
                if (await _context.Users.AnyAsync(u => u.Username == updateUserDto.Username && u.Id != id))
                {
                    return null;
                }
                user.Username = updateUserDto.Username;
            }

            if (!string.IsNullOrEmpty(updateUserDto.Email) && updateUserDto.Email != user.Email)
            {
                if (await _context.Users.AnyAsync(u => u.Email == updateUserDto.Email && u.Id != id))
                {
                    return null;
                }
                user.Email = updateUserDto.Email;
            }

            if (updateUserDto.FirstName != null)
            {
                user.FirstName = updateUserDto.FirstName;
            }

            if (updateUserDto.LastName != null)
            {
                user.LastName = updateUserDto.LastName;
            }

            if (updateUserDto.IsActive.HasValue)
            {
                user.IsActive = updateUserDto.IsActive.Value;
            }

            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return MapToUserDto(user);
        }

        public async Task<bool> DeactivateUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return false;
            }

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ActivateUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return false;
            }

            user.IsActive = true;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return false;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        private static UserDto MapToUserDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                LastLoginAt = user.LastLoginAt
            };
        }
    }
}