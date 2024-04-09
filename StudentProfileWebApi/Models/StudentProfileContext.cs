using Microsoft.EntityFrameworkCore;

namespace StudentProfileWebApi.Models
{
    public class StudentProfileContext : DbContext
    {
        public StudentProfileContext(DbContextOptions<StudentProfileContext> options) : base(options)
        {

        }
        public DbSet<StudentProfile> StudentProfiles { get; set; } = null;
    }
}
