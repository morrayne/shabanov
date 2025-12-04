using Microsoft.EntityFrameworkCore;
using RESTfull.Domain.Entities;

namespace RESTfull.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<ContactInfo> ContactInfos { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<StudyProgram> StudyPrograms { get; set; }
        public DbSet<StudentGroup> StudentGroups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Студенты и учебная программа
            modelBuilder.Entity<Student>()
                .HasOne(s => s.StudyProgram)
                .WithMany(sp => sp.Students)
                .HasForeignKey(s => s.StudyProgramId)
                .OnDelete(DeleteBehavior.Restrict); // запрещаем каскадное удаление

            // Студенты и группа
            modelBuilder.Entity<Student>()
                .HasOne(s => s.StudentGroup)
                .WithMany(g => g.Students)
                .HasForeignKey(s => s.StudentGroupId)
                .OnDelete(DeleteBehavior.Cascade); // разрешаем каскадное удаление

            // Группы и программы
            modelBuilder.Entity<StudentGroup>()
                .HasOne(g => g.StudyProgram)
                .WithMany(sp => sp.StudentGroups)
                .HasForeignKey(g => g.StudyProgramId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
