using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using boilerplate.API.Models;
using Microsoft.EntityFrameworkCore;

namespace boilerplate.API.Data {
    public class AuthRepository : IAuthRepository {
        private readonly DataContext _context;
        public AuthRepository (DataContext context) {
            _context = context;
        }

        // This method checks if the user exists in database
        public async Task<bool> IsUser (string username) {
            if (await _context.Users.AnyAsync(x => x.Username == username)) {
                return true;
            }

            return false;
        }

        // This method logs the user in
        public Task<User> Login (string username, string password) {
            throw new System.NotImplementedException ();
        }

        // This method registers the user in the database
        public async Task<User> RegisterAsync (User user, string password) {
            byte[] passwordHash, passwordSalt;
            CreatePassword(password, out passwordHash, out passwordSalt);

            user.Password = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        // This method generates hashed + salted password to be saved in database
        private void CreatePassword(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hashMac = new HMACSHA512()) {
                passwordHash = hashMac.Key;
                passwordSalt = hashMac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}