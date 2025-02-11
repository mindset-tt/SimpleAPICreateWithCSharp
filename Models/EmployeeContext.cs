using Microsoft.EntityFrameworkCore;
using TestAPICreateWithC_.Models;

namespace TestAPICreateWithC_.Models
{
    public class EmployeeContext : DbContext
    {
        public EmployeeContext(DbContextOptions<EmployeeContext> options)
            : base(options)
        {
        }

        public DbSet<EmployeeItem> EmployeeItems { get; set; } = null!;
    }
}