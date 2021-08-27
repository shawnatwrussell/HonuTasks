using HonuTasks.Models;
using HonuTasks.Models.Enums;
using HonuTasks.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using HonuTasks.Models;
using HonuTasks.Models.Enums;
using HonuTasks.Services.Interfaces;



namespace HonuTasks.Data
{
    //The static keyword is used so that we don't have to create an instance of the class
    //in order to use methods within the class
    public class DataUtility
    {
        //Get company Ids
        private static int creator1Id;
        private static int creator2Id;
        private static int creator3Id;
        private static int creator4Id;
        private static int creator5Id;
        private static int creator6Id;
        private static int creator7Id;

        public static string GetConnectionString(IConfiguration configuration)

        {
            //The default connection string will come from appSettings like usual
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            //It will be automatically overwritten if we are running on Heroku
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            return string.IsNullOrEmpty(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl);
        }

        public static string BuildConnectionString(string databaseUrl)
        {
            //Provides an object representation of a uniform resource identifier (URI) and easy access to the parts of the URI.
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');
            //Provides a simple way to create and manage the contents of connection strings used by the NpgsqlConnection class.
            var builder = new NpgsqlConnectionStringBuilder

            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Prefer,
                TrustServerCertificate = true
            };

            return builder.ToString();
        }

        //Add Data to the Database
        public static async Task ManageDataAsync(IHost host)
        {
            using var svcScope = host.Services.CreateScope();
            var svcProvider = svcScope.ServiceProvider;

            //Service: An instance of RoleManager
            var dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();

            //Service: An instance of RoleManager
            var roleManagerSvc = svcProvider.GetRequiredService<RoleManager<IdentityRole>>();

            //Service: An instance of the UserManager
            var userManagerSvc = svcProvider.GetRequiredService<UserManager<HTUser>>();

            var configuration = svcProvider.GetRequiredService<IConfiguration>();
            var imageSvc = svcProvider.GetRequiredService<IHTImageService>();
            //TsTEP 1: This is the programmatic equivalent to Update-Database
            await dbContextSvc.Database.MigrateAsync();

            //Custom  Bug Tracker Seed Methods
            await SeedRolesAsync(userManagerSvc, roleManagerSvc);
            await SeedDefaultCreatorsAsync(dbContextSvc);
            await SeedDefaultUsersAsync(userManagerSvc, roleManagerSvc, configuration, imageSvc);
            await SeedDemoUsersAsync(userManagerSvc, roleManagerSvc, configuration, imageSvc);
            await SeedDefaultTaskTypeAsync(dbContextSvc);
            await SeedDefaultTasksStatusAsync(dbContextSvc);
            await SeedDefaultTaskPriorityAsync(dbContextSvc);
            await SeedDefaultEventPriorityAsync(dbContextSvc);
            await SeedDefautEventsAsync(dbContextSvc);
            await SeedDefautTasksAsync(dbContextSvc);
        }

        public static async System.Threading.Tasks.Task SeedRolesAsync(UserManager<HTUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Roles
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.EventManager.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.AssignedUser.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.OwnerUser.ToString()));
            //await roleManager.CreateAsync(new IdentityRole(Roles.NewUser.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.DemoUser.ToString()));
        }

        public static async Task SeedDefaultCreatorsAsync(ApplicationDbContext context)
        {
            try
            {
                IList<Creator> defaultCreators = new List<Creator>() {
                    new Creator() { Name = "Creator1", Description="This is default Creator 1" },
                    new Creator() { Name = "Creator2", Description="This is default Creator 2" },
                    new Creator() { Name = "Creator3", Description="This is default Creator 3" },
                    new Creator() { Name = "Creator4", Description="This is default Creator 4" },
                    new Creator() { Name = "Creator5", Description="This is default Creator 5" },
                    new Creator() { Name = "Creator6", Description="This is default Creator 6" },
                    new Creator() { Name = "Creator7", Description="This is default Creator 7" }
                };

                var dbCreators = context.Creator.Select(c => c.Name).ToList();
                await context.Creator.AddRangeAsync(defaultCreators.Where(c => !dbCreators.Contains(c.Name)));
                context.SaveChanges();

                //Get company Ids
                creator1Id = context.Creator.FirstOrDefault(p => p.Name == "Creator1").Id;
                creator2Id = context.Creator.FirstOrDefault(p => p.Name == "Creator2").Id;
                creator3Id = context.Creator.FirstOrDefault(p => p.Name == "Creator3").Id;
                creator4Id = context.Creator.FirstOrDefault(p => p.Name == "Creator4").Id;
                creator5Id = context.Creator.FirstOrDefault(p => p.Name == "Creator5").Id;
                creator6Id = context.Creator.FirstOrDefault(p => p.Name == "Creator6").Id;
                creator7Id = context.Creator.FirstOrDefault(p => p.Name == "Creator7").Id;

            }

            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Creators.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }
        }

        public static async Task SeedDefaultEventPriorityAsync(ApplicationDbContext context)
        {
            try
            {
                IList<EventPriority> eventPriorities = new List<EventPriority>() {
                                                            new EventPriority() { Name = "Low" },
                                                            new EventPriority() { Name = "Medium" },
                                                            new EventPriority() { Name = "High" },
                                                            new EventPriority() { Name = "Urgent" },
                };

                var dbEventPriorities = context.EventPriority.Select(c => c.Name).ToList();
                await context.EventPriority.AddRangeAsync(eventPriorities.Where(c => !dbEventPriorities.Contains(c.Name)));
                context.SaveChanges();

            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Event Priorities.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }
        }

        public static async Task SeedDefautEventsAsync(ApplicationDbContext context)
        {
            //Get project priority Ids
            int priorityLow = context.EventPriority.FirstOrDefault(p => p.Name == "Low").Id;
            int priorityMedium = context.EventPriority.FirstOrDefault(p => p.Name == "Medium").Id;
            int priorityHigh = context.EventPriority.FirstOrDefault(p => p.Name == "High").Id;
            int priorityUrgent = context.EventPriority.FirstOrDefault(p => p.Name == "Urgent").Id;

            try
            {
                IList<Events> events = new List<Events>() {
                     new Events()
                     {
                         CreatorId = creator1Id,
                         Name = "Build a Personal Porfolio",
                         Description="Single page html, css & javascript page.  Serves as a landing page for candidates and contains a bio and links to all applications and challenges." ,
                         StartDate = new DateTime(2021,4,5),
                         EndDate = new DateTime(2021,4,5).AddMonths(3),
                         EventPriorityId = priorityLow
                     },
                     new Events()
                     {
                         CreatorId = creator2Id,
                         Name = "Build a supplemental Blog Web Application",
                         Description="Candidate's custom built web application using .Net Core with MVC, a postgres database and hosted in a heroku container.  The app is designed for the candidate to create, update and maintain a live blog site.",
                         StartDate = new DateTime(2021,4,5),
                         EndDate = new DateTime(2021,4,5).AddMonths(3),
                         EventPriorityId = priorityMedium
                     },
                     new Events()
                     {
                         CreatorId = creator3Id,
                         Name = "Build an Issue Tracking Web Application",
                         Description="A custom designed .Net Core application with postgres database.  The application is a multi tennent application designed to track issue tickets' progress.  Implemented with identity and user roles, Tickets are maintained in projects which are maintained by users in the role of projectmanager.  Each project has a team and team members.",
                         StartDate = new DateTime(2021,4,5),
                         EndDate = new DateTime(2021,4,5).AddMonths(3),
                         EventPriorityId = priorityHigh
                     },
                    new Events()
                     {
                         CreatorId = creator1Id,
                         Name = "Build a Movie Information Web Application",
                         Description="A custom designed .Net Core application with postgres database.  An API based application allows users to input and import movie posters and details including cast and crew information.",
                         StartDate = new DateTime(2021,4,5),
                         EndDate = new DateTime(2021,4,5).AddMonths(3),
                         EventPriorityId = priorityHigh
                     },
                     new Events()
                     {
                         CreatorId = creator2Id,
                         Name = "Build an Address Book Web Application",
                         Description="A custom designed .Net Core application with postgres database.  This is an application to serve as a rolodex of contacts for a given user..",
                         StartDate = new DateTime(2021,4,5),
                         EndDate = new DateTime(2021,4,5).AddMonths(3),
                         EventPriorityId = priorityHigh
                     }
                };

                var dbEvents = context.Events.Select(c => c.Name).ToList();
                await context.Events.AddRangeAsync(events.Where(c => !dbEvents.Contains(c.Name)));
                context.SaveChanges();
            }

            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Events.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }
        }

        public static async Task SeedDefaultEventStatusAsync(ApplicationDbContext context)
        {
            try
            {
                IList<EventStatus> eventStatuses = new List<EventStatus>() {
                    new EventStatus() { Name = "New" },                 // Newly Created ticket having never been assigned
                    new EventStatus() { Name = "Unassigned" },          // Ticket has been assigned at some point but is currently unassigned
                    new EventStatus() { Name = "Development" },         // Ticket is assigned and currently being worked 
                    new EventStatus() { Name = "Testing" },             // Ticket is assigned and is currently being tested
                    new EventStatus() { Name = "Resolved" },           // Ticket remains assigned to the developer but work in now complete
                    new EventStatus() { Name = "Archived" }            // Ticket remains assigned to the developer but becomes inactive
                };

                var dbEventStatuses = context.EventStatus.Select(c => c.Name).ToList();
                await context.EventStatus.AddRangeAsync(eventStatuses.Where(c => !dbEventStatuses.Contains(c.Name)));
                context.SaveChanges();

            }

            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Project Statuses.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }
        }

        public static async Task SeedDefaultUsersAsync(UserManager<HTUser> userManager, RoleManager<IdentityRole> roleManager,
            IConfiguration configuration, IHTImageService imageService)
        {
            var defaultImageData = await imageService.EncodeFileAsync(configuration["DefaultUserImage"]);
            var defaultContentType = configuration["DefaultUserImage"].Split('.')[1];

            //Seed Default Admin User
            var defaultUser = new HTUser
            {
                UserName = "shawnatwrussell@gmail.com",
                Email = "shawnatwrussell@gmail.com",
                FirstName = "Shawna",
                LastName = "Russell",
                EmailConfirmed = true,
                AvatarFileData = defaultImageData,
                ContentType = defaultContentType
            };

            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Default Admin User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }


        }

        public static async Task SeedDemoUsersAsync(UserManager<HTUser> userManager, RoleManager<IdentityRole> roleManager,
            IConfiguration configuration, IHTImageService imageService)
        {
            var defaultImageData = await imageService.EncodeFileAsync(configuration["DefaultUserImage"]);
            var defaultContentType = configuration["DefaultUserImage"].Split('.')[1];

            //Seed Demo Admin User
            var defaultUser = new HTUser
            {
                UserName = "demoadmin@stargatecommand.com",
                Email = "demoadmin@stargatecommand.com",
                FirstName = "Demo",
                LastName = "Admin",
                EmailConfirmed = true,
                AvatarFileData = defaultImageData,
                ContentType = defaultContentType

            };

            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.DemoUser.ToString());

                }
            }

            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Demo Admin User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }

            //Seed Demo ProjectManager User
            defaultUser = new HTUser
            {
                UserName = "demopm@stargatecommand.com",
                Email = "demopm@stargatecommand.com",
                FirstName = "Demo",
                LastName = "ProjectManager",
                EmailConfirmed = true,
                AvatarFileData = defaultImageData,
                ContentType = defaultContentType

            };

            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.EventManager.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.DemoUser.ToString());
                }
            }

            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Demo Event Manager User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }

            //Seed Demo Assigned User
            defaultUser = new HTUser
            {
                UserName = "demodev@stargatecommand.com",
                Email = "demodev@stargatecommand.com",
                FirstName = "Demo",
                LastName = "Developer",
                EmailConfirmed = true,
                AvatarFileData = defaultImageData,
                ContentType = defaultContentType

            };

            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.AssignedUser.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.DemoUser.ToString());
                }
            }

            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Demo AssignedUser1 User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }

            //Seed Demo Creator User
            defaultUser = new HTUser
            {
                UserName = "demosub@stargatecommand.com",
                Email = "demosub@stargatecommand.com",
                FirstName = "Demo",
                LastName = "Submitter",
                EmailConfirmed = true,
                AvatarFileData = defaultImageData,
                ContentType = defaultContentType

            };

            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.OwnerUser.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.DemoUser.ToString());
                }
            }

            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Demo Creator User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }

            //Seed Demo New User
            defaultUser = new HTUser
            {
                UserName = "demonew@stargatecommand.com",
                Email = "demonew@stargatecommand.com",
                FirstName = "Demo",
                LastName = "NewUser",
                EmailConfirmed = true,
                AvatarFileData = defaultImageData,
                ContentType = defaultContentType

            };

            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.OwnerUser.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.DemoUser.ToString());
                }
            }

            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Demo Creator User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }
        }

        public static async Task SeedDefaultTaskTypeAsync(ApplicationDbContext context)
        {
            try
            {
                IList<TaskType> taskTypes = new List<TaskType>() {
                     new TaskType() { Name = "New Development" },
                     new TaskType() { Name = "Runtime" },
                     new TaskType() { Name = "UI" },
                     new TaskType() { Name = "Maintenance" },
                };

                var dbTaskTypes = context.TaskType.Select(c => c.Name).ToList();
                await context.TaskType.AddRangeAsync(taskTypes.Where(c => !dbTaskTypes.Contains(c.Name)));
                context.SaveChanges();

            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Task Types.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }
        }

        public static async Task SeedDefaultTasksStatusAsync(ApplicationDbContext context)
        {
            try
            {
                IList<TasksStatus> tasksStatuses = new List<TasksStatus>() {
                    new TasksStatus() { Name = "New" },                 // Newly Created task having never been assigned
                    new TasksStatus() { Name = "Unassigned" },          // Task has been assigned at some point but is currently unassigned
                    new TasksStatus() { Name = "Development" },         // Task is assigned and currently being worked 
                    new TasksStatus() { Name = "Testing" },             // Task is assigned and is currently being tested
                    new TasksStatus() { Name = "Resolved" },           // Task remains assigned to the User but work in now complete
                    new TasksStatus() { Name = "Archived" }            // Task remains assigned to the USer but becomes inactive
                };

                var dbTasksStatuses = context.TasksStatus.Select(c => c.Name).ToList();
                await context.TasksStatus.AddRangeAsync(tasksStatuses.Where(c => !dbTasksStatuses.Contains(c.Name)));
                context.SaveChanges();

            }

            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Task Statuses.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }
        }

        public static async Task SeedDefaultTaskPriorityAsync(ApplicationDbContext context)
        {
            try
            {
                IList<TaskPriority> taskPriorities = new List<TaskPriority>() {
                                                    new TaskPriority() { Name = "Low" },
                                                    new TaskPriority() { Name = "Medium" },
                                                    new TaskPriority() { Name = "High" },
                                                    new TaskPriority() { Name = "Urgent" },
                };

                var dbTaskPriorities = context.TaskPriority.Select(c => c.Name).ToList();
                await context.TaskPriority.AddRangeAsync(taskPriorities.Where(c => !dbTaskPriorities.Contains(c.Name)));
                context.SaveChanges();

            }

            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Task Priorities.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }
        }

        public static async Task SeedDefautTasksAsync(ApplicationDbContext context)
        {
            //Get project Ids
            int portfolioId = context.Events.FirstOrDefault(p => p.Name == "Build a Personal Porfolio").Id;
            int blogId = context.Events.FirstOrDefault(p => p.Name == "Build a supplemental Blog Web Application").Id;
            int bugtrackerId = context.Events.FirstOrDefault(p => p.Name == "Build an Issue Tracking Web Application").Id;

            //Get ticket type Ids
            int typeNewDev = context.TaskType.FirstOrDefault(p => p.Name == "New Development").Id;
            int typeRuntime = context.TaskType.FirstOrDefault(p => p.Name == "Runtime").Id;
            int typeUI = context.TaskType.FirstOrDefault(p => p.Name == "UI").Id;
            int typeMaintenance = context.TaskType.FirstOrDefault(p => p.Name == "Maintenance").Id;

            //Get ticket priority Ids
            int priorityLow = context.TaskPriority.FirstOrDefault(p => p.Name == "Low").Id;
            int priorityMedium = context.TaskPriority.FirstOrDefault(p => p.Name == "Medium").Id;
            int priorityHigh = context.TaskPriority.FirstOrDefault(p => p.Name == "High").Id;
            int priorityUrgent = context.TaskPriority.FirstOrDefault(p => p.Name == "Urgent").Id;

            //Get ticket status Ids
            int statusNew = context.TasksStatus.FirstOrDefault(p => p.Name == "New").Id;
            int statusUnassigned = context.TasksStatus.FirstOrDefault(p => p.Name == "Unassigned").Id;
            int statusDev = context.TasksStatus.FirstOrDefault(p => p.Name == "Development").Id;
            int statusTest = context.TasksStatus.FirstOrDefault(p => p.Name == "Testing").Id;
            int statusResolved = context.TasksStatus.FirstOrDefault(p => p.Name == "Resolved").Id;

            try
            {
                IList<Tasks> tasks = new List<Tasks>() {
                                
                                //PORTFOLIO
                                new Tasks() {Title = "Portfolio Ticket 1", Description = "Ticket details for portfolio ticket 1", Created = DateTimeOffset.Now, EventId = portfolioId, TaskPriorityId = priorityLow, TaskStatusId = statusNew, TaskTypeId = typeNewDev},
                                new Tasks() {Title = "Portfolio Ticket 2", Description = "Ticket details for portfolio ticket 2", Created = DateTimeOffset.Now, EventId = portfolioId, TaskPriorityId = priorityMedium, TaskStatusId = statusUnassigned, TaskTypeId = typeMaintenance},
                                new Tasks() {Title = "Portfolio Ticket 3", Description = "Ticket details for portfolio ticket 3", Created = DateTimeOffset.Now, EventId = portfolioId, TaskPriorityId = priorityHigh, TaskStatusId = statusDev, TaskTypeId = typeUI},
                                new Tasks() {Title = "Portfolio Ticket 4", Description = "Ticket details for portfolio ticket 4", Created = DateTimeOffset.Now, EventId = portfolioId, TaskPriorityId = priorityUrgent, TaskStatusId = statusTest, TaskTypeId = typeRuntime},
                                new Tasks() {Title = "Portfolio Ticket 5", Description = "Ticket details for portfolio ticket 5", Created = DateTimeOffset.Now, EventId = portfolioId, TaskPriorityId = priorityLow, TaskStatusId = statusNew, TaskTypeId = typeNewDev},
                                new Tasks() {Title = "Portfolio Ticket 6", Description = "Ticket details for portfolio ticket 6", Created = DateTimeOffset.Now, EventId = portfolioId, TaskPriorityId = priorityMedium, TaskStatusId = statusUnassigned, TaskTypeId = typeMaintenance},
                                new Tasks() {Title = "Portfolio Ticket 7", Description = "Ticket details for portfolio ticket 7", Created = DateTimeOffset.Now, EventId = portfolioId, TaskPriorityId = priorityHigh, TaskStatusId = statusDev, TaskTypeId = typeUI},
                                new Tasks() {Title = "Portfolio Ticket 8", Description = "Ticket details for portfolio ticket 8", Created = DateTimeOffset.Now, EventId = portfolioId, TaskPriorityId = priorityUrgent, TaskStatusId = statusTest, TaskTypeId = typeRuntime},
                                
                                //BLOG
                                new Tasks() {Title = "Blog Ticket 1", Description = "Ticket details for blog ticket 1", Created = DateTimeOffset.Now, EventId = blogId, TaskPriorityId = priorityLow, TaskStatusId = statusUnassigned, TaskTypeId = typeRuntime},
                                new Tasks() {Title = "Blog Ticket 2", Description = "Ticket details for blog ticket 2", Created = DateTimeOffset.Now, EventId = blogId, TaskPriorityId = priorityMedium, TaskStatusId = statusDev, TaskTypeId = typeUI},
                                new Tasks() {Title = "Blog Ticket 3", Description = "Ticket details for blog ticket 3", Created = DateTimeOffset.Now, EventId = blogId, TaskPriorityId = priorityHigh, TaskStatusId = statusNew, TaskTypeId = typeMaintenance},
                                new Tasks() {Title = "Blog Ticket 4", Description = "Ticket details for blog ticket 4", Created = DateTimeOffset.Now, EventId = blogId, TaskPriorityId = priorityUrgent, TaskStatusId = statusUnassigned, TaskTypeId = typeNewDev},
                                new Tasks() {Title = "Blog Ticket 5", Description = "Ticket details for blog ticket 5", Created = DateTimeOffset.Now, EventId = blogId, TaskPriorityId = priorityLow, TaskStatusId = statusDev,  TaskTypeId = typeRuntime},
                                new Tasks() {Title = "Blog Ticket 6", Description = "Ticket details for blog ticket 6", Created = DateTimeOffset.Now, EventId = blogId, TaskPriorityId = priorityMedium, TaskStatusId = statusNew,  TaskTypeId = typeUI},
                                new Tasks() {Title = "Blog Ticket 7", Description = "Ticket details for blog ticket 7", Created = DateTimeOffset.Now, EventId = blogId, TaskPriorityId = priorityHigh, TaskStatusId = statusUnassigned, TaskTypeId = typeMaintenance},
                                new Tasks() {Title = "Blog Ticket 8", Description = "Ticket details for blog ticket 8", Created = DateTimeOffset.Now, EventId = blogId, TaskPriorityId = priorityUrgent, TaskStatusId = statusDev,  TaskTypeId = typeNewDev},
                                new Tasks() {Title = "Blog Ticket 9", Description = "Ticket details for blog ticket 9", Created = DateTimeOffset.Now, EventId = blogId, TaskPriorityId = priorityLow, TaskStatusId = statusNew,  TaskTypeId = typeRuntime},
                                new Tasks() {Title = "Blog Ticket 10", Description = "Ticket details for blog ticket 10", Created = DateTimeOffset.Now, EventId = blogId, TaskPriorityId = priorityMedium, TaskStatusId = statusUnassigned, TaskTypeId = typeUI},
                                new Tasks() {Title = "Blog Ticket 11", Description = "Ticket details for blog ticket 11", Created = DateTimeOffset.Now, EventId = blogId, TaskPriorityId = priorityHigh, TaskStatusId = statusDev,  TaskTypeId = typeMaintenance},
                                new Tasks() {Title = "Blog Ticket 12", Description = "Ticket details for blog ticket 12", Created = DateTimeOffset.Now, EventId = blogId, TaskPriorityId = priorityUrgent, TaskStatusId = statusNew,  TaskTypeId = typeNewDev},
                                new Tasks() {Title = "Blog Ticket 13", Description = "Ticket details for blog ticket 13", Created = DateTimeOffset.Now, EventId = blogId, TaskPriorityId = priorityLow, TaskStatusId = statusUnassigned, TaskTypeId = typeRuntime},
                                new Tasks() {Title = "Blog Ticket 14", Description = "Ticket details for blog ticket 14", Created = DateTimeOffset.Now, EventId = blogId, TaskPriorityId = priorityMedium, TaskStatusId = statusDev,  TaskTypeId = typeUI},
                                new Tasks() {Title = "Blog Ticket 15", Description = "Ticket details for blog ticket 15", Created = DateTimeOffset.Now, EventId = blogId, TaskPriorityId = priorityHigh, TaskStatusId = statusNew,  TaskTypeId = typeMaintenance},
                                new Tasks() {Title = "Blog Ticket 16", Description = "Ticket details for blog ticket 16", Created = DateTimeOffset.Now, EventId = blogId, TaskPriorityId = priorityUrgent, TaskStatusId = statusUnassigned, TaskTypeId = typeNewDev},
                                new Tasks() {Title = "Blog Ticket 17", Description = "Ticket details for blog ticket 17", Created = DateTimeOffset.Now, EventId = blogId, TaskPriorityId = priorityHigh, TaskStatusId = statusDev,  TaskTypeId = typeNewDev},
                                
                                //BUGTRACKER                                                                                                                         
                                new Tasks() { Title = "Bug Tracker Ticket 1", Description = "Build Landing Page", Created = DateTimeOffset.Now, EventId = bugtrackerId, TaskPriorityId = priorityHigh, TaskStatusId = statusUnassigned, TaskTypeId = typeNewDev},
                                new Tasks() { Title = "Bug Tracker Ticket 2", Description = "Ticket details for blog ticket 2", Created = DateTimeOffset.Now, EventId = bugtrackerId, TaskPriorityId = priorityHigh, TaskStatusId = statusUnassigned, TaskTypeId = typeNewDev},
                                new Tasks() { Title = "Bug Tracker Ticket 3", Description = "Ticket details for blog ticket 3", Created = DateTimeOffset.Now, EventId = bugtrackerId, TaskPriorityId = priorityHigh, TaskStatusId = statusUnassigned, TaskTypeId = typeNewDev},
                                new Tasks() { Title = "Bug Tracker Ticket 4", Description = "Ticket details for blog ticket 4", Created = DateTimeOffset.Now, EventId = bugtrackerId, TaskPriorityId = priorityHigh, TaskStatusId = statusUnassigned, TaskTypeId = typeNewDev},
                                new Tasks() { Title = "Bug Tracker Ticket 5", Description = "Ticket details for blog ticket 5", Created = DateTimeOffset.Now, EventId = bugtrackerId, TaskPriorityId = priorityHigh, TaskStatusId = statusUnassigned, TaskTypeId = typeNewDev},
                                new Tasks() { Title = "Bug Tracker Ticket 6", Description = "Ticket details for blog ticket 6", Created = DateTimeOffset.Now, EventId = bugtrackerId, TaskPriorityId = priorityHigh, TaskStatusId = statusUnassigned, TaskTypeId = typeNewDev},
                                new Tasks() { Title = "Bug Tracker Ticket 7", Description = "Ticket details for blog ticket 7", Created = DateTimeOffset.Now, EventId = bugtrackerId, TaskPriorityId = priorityHigh, TaskStatusId = statusUnassigned, TaskTypeId = typeNewDev},
                                new Tasks() { Title = "Bug Tracker Ticket 8", Description = "Ticket details for blog ticket 8", Created = DateTimeOffset.Now, EventId = bugtrackerId, TaskPriorityId = priorityHigh, TaskStatusId = statusUnassigned, TaskTypeId = typeNewDev},
                                new Tasks() { Title = "Bug Tracker Ticket 9", Description = "Ticket details for blog ticket 9", Created = DateTimeOffset.Now, EventId = bugtrackerId, TaskPriorityId = priorityHigh, TaskStatusId = statusUnassigned, TaskTypeId = typeNewDev},
                                new Tasks() { Title = "Bug Tracker Ticket 10", Description = "Ticket details for blog ticket 10", Created = DateTimeOffset.Now, EventId = bugtrackerId, TaskPriorityId = priorityHigh, TaskStatusId = statusUnassigned, TaskTypeId = typeNewDev},
                                new Tasks() { Title = "Bug Tracker Ticket 11", Description = "Ticket details for blog ticket 11", Created = DateTimeOffset.Now, EventId = bugtrackerId, TaskPriorityId = priorityHigh, TaskStatusId = statusUnassigned, TaskTypeId = typeNewDev},
                                new Tasks() { Title = "Bug Tracker Ticket 12", Description = "Ticket details for blog ticket 12", Created = DateTimeOffset.Now, EventId = bugtrackerId, TaskPriorityId = priorityHigh, TaskStatusId = statusUnassigned, TaskTypeId = typeNewDev},
                                new Tasks() { Title = "Bug Tracker Ticket 13", Description = "Ticket details for blog ticket 13", Created = DateTimeOffset.Now, EventId = bugtrackerId, TaskPriorityId = priorityHigh, TaskStatusId = statusUnassigned, TaskTypeId = typeNewDev},
                                new Tasks() { Title = "Bug Tracker Ticket 14", Description = "Ticket details for blog ticket 14", Created = DateTimeOffset.Now, EventId = bugtrackerId, TaskPriorityId = priorityHigh, TaskStatusId = statusUnassigned, TaskTypeId = typeNewDev},
                                new Tasks() { Title = "Bug Tracker Ticket 15", Description = "Ticket details for blog ticket 15", Created = DateTimeOffset.Now, EventId = bugtrackerId, TaskPriorityId = priorityHigh, TaskStatusId = statusUnassigned, TaskTypeId = typeNewDev},
                                new Tasks() { Title = "Bug Tracker Ticket 16", Description = "Ticket details for blog ticket 16", Created = DateTimeOffset.Now, EventId = bugtrackerId, TaskPriorityId = priorityHigh, TaskStatusId = statusUnassigned, TaskTypeId = typeNewDev},
                                new Tasks() { Title = "Bug Tracker Ticket 17", Description = "Ticket details for blog ticket 17", Created = DateTimeOffset.Now, EventId = bugtrackerId, TaskPriorityId = priorityHigh, TaskStatusId = statusUnassigned, TaskTypeId = typeNewDev},
                                new Tasks() { Title = "Bug Tracker Ticket 18", Description = "Ticket details for blog ticket 18", Created = DateTimeOffset.Now, EventId = bugtrackerId, TaskPriorityId = priorityHigh, TaskStatusId = statusUnassigned, TaskTypeId = typeNewDev},
                                new Tasks() { Title = "Bug Tracker Ticket 19", Description = "Ticket details for blog ticket 19", Created = DateTimeOffset.Now, EventId = bugtrackerId, TaskPriorityId = priorityHigh, TaskStatusId = statusUnassigned, TaskTypeId = typeNewDev},
                                new Tasks() { Title = "Bug Tracker Ticket 20", Description = "Ticket details for blog ticket 20", Created = DateTimeOffset.Now, EventId = bugtrackerId, TaskPriorityId = priorityHigh, TaskStatusId = statusUnassigned, TaskTypeId = typeNewDev},
                                new Tasks() { Title = "Bug Tracker Ticket 21", Description = "Ticket details for blog ticket 21", Created = DateTimeOffset.Now, EventId = bugtrackerId, TaskPriorityId = priorityHigh, TaskStatusId = statusUnassigned, TaskTypeId = typeNewDev},
                                new Tasks() { Title = "Bug Tracker Ticket 22", Description = "Ticket details for blog ticket 22", Created = DateTimeOffset.Now, EventId = bugtrackerId, TaskPriorityId = priorityHigh, TaskStatusId = statusUnassigned, TaskTypeId = typeNewDev},
                                new Tasks() { Title = "Bug Tracker Ticket 23", Description = "Ticket details for blog ticket 23", Created = DateTimeOffset.Now, EventId = bugtrackerId, TaskPriorityId = priorityHigh, TaskStatusId = statusUnassigned, TaskTypeId = typeNewDev},
                                new Tasks() { Title = "Bug Tracker Ticket 24", Description = "Ticket details for blog ticket 24", Created = DateTimeOffset.Now, EventId = bugtrackerId, TaskPriorityId = priorityHigh, TaskStatusId = statusUnassigned, TaskTypeId = typeNewDev},
                                new Tasks() { Title = "Bug Tracker Ticket 25", Description = "Ticket details for blog ticket 25", Created = DateTimeOffset.Now, EventId = bugtrackerId, TaskPriorityId = priorityHigh, TaskStatusId = statusUnassigned, TaskTypeId = typeNewDev},
                                new Tasks() { Title = "Bug Tracker Ticket 26", Description = "Ticket details for blog ticket 26", Created = DateTimeOffset.Now, EventId = bugtrackerId, TaskPriorityId = priorityHigh, TaskStatusId = statusUnassigned, TaskTypeId = typeNewDev},
                                new Tasks() { Title = "Bug Tracker Ticket 27", Description = "Ticket details for blog ticket 27", Created = DateTimeOffset.Now, EventId = bugtrackerId, TaskPriorityId = priorityHigh, TaskStatusId = statusUnassigned, TaskTypeId = typeNewDev},
                                new Tasks() { Title = "Bug Tracker Ticket 28", Description = "Ticket details for blog ticket 28", Created = DateTimeOffset.Now, EventId = bugtrackerId, TaskPriorityId = priorityHigh, TaskStatusId = statusUnassigned, TaskTypeId = typeNewDev},
                                new Tasks() { Title = "Bug Tracker Ticket 29", Description = "Ticket details for blog ticket 29", Created = DateTimeOffset.Now, EventId = bugtrackerId, TaskPriorityId = priorityHigh, TaskStatusId = statusUnassigned, TaskTypeId = typeNewDev},
                                new Tasks() { Title = "Bug Tracker Ticket 30", Description = "Ticket details for blog ticket 30", Created = DateTimeOffset.Now, EventId = bugtrackerId, TaskPriorityId = priorityHigh, TaskStatusId = statusUnassigned, TaskTypeId = typeNewDev},
                };

                var dbTasks = context.Tasks.Select(c => c.Title).ToList();
                await context.Tasks.AddRangeAsync(tasks.Where(c => !dbTasks.Contains(c.Title)));
                context.SaveChanges();
            }

            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Task.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }
        }
    }
}
