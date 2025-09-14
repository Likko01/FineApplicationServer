using FinesApplication.API.DAL.Models;
using Microsoft.EntityFrameworkCore;

    public class ApplicationDbContext :DbContext
    {
        public DbSet<Fine> Fines { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<History> Histories { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
}
