using boilerplate.API.Models;
using Microsoft.EntityFrameworkCore;

namespace boilerplate.API.Data {
    public class DataContext : DbContext {
        public DataContext (DbContextOptions<DataContext> options) : base (options) { }

        // User model data context
        public DbSet<User> Users { get; set; }
    }
}