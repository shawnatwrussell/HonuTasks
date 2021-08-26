using HonuTasks.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace HonuTasks.Data
{
    public class ApplicationDbContext : IdentityDbContext<HTUser>
    {
        private readonly IConfiguration Configuration;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
            : base(options)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseNpgsql(
                    DataUtility.GetConnectionString(Configuration),
            o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
        }


        //SEED DATA
        /*        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                {
                    optionsBuilder
                        .UseNpgsql(
                            DataUtility.GetConnectionString(Configuration),
                    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
                }
        */



        public DbSet<HonuTasks.Models.HTUser> User { get; set; }
        public DbSet<HonuTasks.Models.Invite> Invite { get; set; }
        public DbSet<HonuTasks.Models.Notification> Notification { get; set; }
        public DbSet<HonuTasks.Models.Events> Events { get; set; }
        public DbSet<HonuTasks.Models.EventPriority> EventPriority { get; set; }
        public DbSet<HonuTasks.Models.EventStatus> EventStatus { get; set; }
        public DbSet<HonuTasks.Models.Creator> Creator { get; set; }
        public DbSet<HonuTasks.Models.Tasks> Tasks { get; set; }
        public DbSet<HonuTasks.Models.TaskAttachment> TaskAttachment { get; set; }
        public DbSet<HonuTasks.Models.TaskComment> TaskComment { get; set; }
        public DbSet<HonuTasks.Models.TaskHistory> TaskHistory { get; set; }
        public DbSet<HonuTasks.Models.TaskPriority> TaskPriority { get; set; }
        public DbSet<HonuTasks.Models.TasksStatus> TasksStatus { get; set; }
        public DbSet<HonuTasks.Models.TaskType> TaskType { get; set; }


    }
}
