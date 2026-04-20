using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectApprovalSystem.Models;

namespace ProjectApprovalSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<ResearchArea> ResearchAreas { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<SupervisorExpertise> SupervisorExpertises { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure relationships
            builder.Entity<Project>()
                .HasOne(p => p.Student)
                .WithMany(u => u.StudentProjects)
                .HasForeignKey(p => p.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Project>()
                .HasOne(p => p.Supervisor)
                .WithMany(u => u.SupervisedProjects)
                .HasForeignKey(p => p.SupervisorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Match>()
                .HasOne(m => m.Student)
                .WithMany(u => u.StudentMatches)
                .HasForeignKey(m => m.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Match>()
                .HasOne(m => m.Supervisor)
                .WithMany(u => u.SupervisorMatches)
                .HasForeignKey(m => m.SupervisorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Match>()
                .HasOne(m => m.Project)
                .WithMany(p => p.Matches)
                .HasForeignKey(m => m.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed Research Areas
            builder.Entity<ResearchArea>().HasData(
                new ResearchArea { Id = 1, Name = "Artificial Intelligence", Description = "Machine Learning, Deep Learning, NLP" },
                new ResearchArea { Id = 2, Name = "Web Development", Description = "Frontend, Backend, Full Stack" },
                new ResearchArea { Id = 3, Name = "Cybersecurity", Description = "Network Security, Cryptography" },
                new ResearchArea { Id = 4, Name = "Cloud Computing", Description = "AWS, Azure, DevOps" },
                new ResearchArea { Id = 5, Name = "Mobile Development", Description = "iOS, Android, Cross-platform" },
                new ResearchArea { Id = 6, Name = "Data Science", Description = "Big Data, Analytics, Visualization" }
            );
        }
    }
}