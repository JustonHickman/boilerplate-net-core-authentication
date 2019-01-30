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

        // This method checks if the user exists in database
        public async Task<bool> IsEmail (string email) {
            if (await _context.Users.AnyAsync(x => x.Email == email)) {
                return true;
            }

            return false;
        }

        // This method logs the user in
        public async Task<User> Login (string username, string password) {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
            
            if (user == null) {
                return null;
            }

            if (!ComparePassword(password, user.Password, user.PasswordSalt)) {
                return null;
            }

            return user;

        }

        private bool ComparePassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hashMac = new HMACSHA512(passwordSalt)) {
                var hashedPassword = hashMac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                int index = 0;
                foreach(byte signleByte in hashedPassword) {
                    if (signleByte != passwordHash[index]) return false;
                    index++;
                }
            }
            return true;
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
                passwordSalt = hashMac.Key;
                passwordHash = hashMac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}