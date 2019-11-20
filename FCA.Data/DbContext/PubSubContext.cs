using FCA.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FCA.Data
{
    public class PubSubContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public PubSubContext()
        {
        }

        public PubSubContext(DbContextOptions<PubSubContext> options) : base(options)
        {
        }

        public DbSet<Student> Student { get; set; }
    }
}
