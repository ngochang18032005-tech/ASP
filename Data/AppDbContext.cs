using Microsoft.EntityFrameworkCore;
using VANTHINGOCHANG_2123110352.Models;

namespace VANTHINGOCHANG_2123110352.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Overtime> Overtimes { get; set; }
        public DbSet<Salary> Salaries { get; set; }
        public DbSet<Bonus> Bonuses { get; set; }
        public DbSet<Penalty> Penalties { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<ShiftAssignment> ShiftAssignments { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Branch>().HasKey(b => b.BranchId);

            // Hai dòng này phải khớp tên với thuộc tính trong class Model
            modelBuilder.Entity<Department>().HasKey(d => d.DepartmentId);
            modelBuilder.Entity<Position>().HasKey(p => p.PositionId);

            modelBuilder.Entity<AuditLog>().HasKey(a => a.Id);
        }
    }
}