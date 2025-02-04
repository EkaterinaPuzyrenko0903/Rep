using Microsoft.EntityFrameworkCore;
namespace _11_Calculator.Data
{
    public class CalculatorContext : DbContext
    {
        public DbSet<DataInputVariant> DataInputVariants { get; set; }
        public CalculatorContext(DbContextOptions<CalculatorContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //OnModelCreating(modelBuilder);
        }


    }
}
