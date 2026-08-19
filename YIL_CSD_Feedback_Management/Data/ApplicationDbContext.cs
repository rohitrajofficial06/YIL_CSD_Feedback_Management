using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using YIL_CSD_Feedback_Management.Models;

namespace YIL_CSD_Feedback_Management.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ==========================================
        // DbSets
        // ==========================================

        public DbSet<User> Users { get; set; }
        public DbSet<Department> Departments { get; set; }

        public DbSet<FeedbackUpload> FeedbackUploads { get; set; }

        public DbSet<CustomerFeedback> CustomerFeedbacks { get; set; }

        public DbSet<CustomerFeedbackRating> CustomerFeedbackRatings { get; set; }
        public DbSet<FeedbackStatusUpload> FeedbackStatusUploads { get; set; }

        public DbSet<FeedbackStatusUploadHeader> FeedbackStatusUploadHeaders { get; set; }

        public DbSet<FeedbackQuestion> FeedbackQuestions { get; set; }

        public DbSet<ClosedCaseUpload> ClosedCaseUploads { get; set; }

        public DbSet<ClosedCaseUploadDetail> ClosedCaseUploadDetails { get; set; }

        public DbSet<ClosedCase> ClosedCases { get; set; }

        public DbSet<Region> Regions { get; set; }

        public DbSet<UserActiveSession> UserActiveSessions { get; set; }

        // ==========================================
        // Model Configuration
        // ==========================================

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //User Table
            modelBuilder.Entity<User>().ToTable("tblUsers");
            // Department Table
            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("tblDepartment");
            });

            // Feedback Upload Table
            modelBuilder.Entity<FeedbackUpload>(entity =>
            {
                entity.ToTable("trnFeedbackUpload");

                // Ignore calculated property
                entity.Ignore(e => e.DepartmentName);
            });

            modelBuilder.Entity<CustomerFeedback>()
                 .ToTable("trnCustomerFeedback");

            modelBuilder.Entity<CustomerFeedbackRating>()
                .ToTable("trnCustomerFeedbackRating");

            modelBuilder.Entity<FeedbackQuestion>()
            .ToTable("tblFeedbackQuestion");

            modelBuilder.Entity<ClosedCaseUpload>()
    .ToTable("tblClosedCaseUpload");

            modelBuilder.Entity<ClosedCaseUploadDetail>()
                .ToTable("tblClosedCaseUploadDetail");

            modelBuilder.Entity<ClosedCase>()
                .ToTable("tblClosedCases");

            modelBuilder.Entity<CustomerFeedbackRating>()
     .HasOne(r => r.Feedback)
     .WithMany(f => f.Ratings)
     .HasForeignKey(r => r.FeedbackID)
     .OnDelete(DeleteBehavior.Cascade);


        }
    }
}