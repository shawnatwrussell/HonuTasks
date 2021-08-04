using HonuTasks.Models;
using HonuTasks.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace HonuTasks.Data
{
    //The static keyword is used so that we don't have to create an instance of the class
    //in order to use methods within the class
    public class DataUtility
    {
        //Get company Ids
        private static int company1Id;
        private static int company2Id;
        private static int company3Id;
        private static int company4Id;
        private static int company5Id;
        private static int company6Id;
        private static int company7Id;

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
            var userManagerSvc = svcProvider.GetRequiredService<UserManager<BTUser>>();

            var configuration = svcProvider.GetRequiredService<IConfiguration>();
            var imageSvc = svcProvider.GetRequiredService<IBTImageService>();
            //TsTEP 1: This is the programmatic equivalent to Update-Database
            await dbContextSvc.Database.MigrateAsync();

            //Custom  Bug Tracker Seed Methods
            await SeedRolesAsync(userManagerSvc, roleManagerSvc);
            await SeedDefaultCompaniesAsync(dbContextSvc);
            await SeedDefaultUsersAsync(userManagerSvc, roleManagerSvc, configuration, imageSvc);
            await SeedDemoUsersAsync(userManagerSvc, roleManagerSvc, configuration, imageSvc);
            await SeedDefaultTicketTypeAsync(dbContextSvc);
            await SeedDefaultTicketStatusAsync(dbContextSvc);
            await SeedDefaultTicketPriorityAsync(dbContextSvc);
            await SeedDefaultProjectPriorityAsync(dbContextSvc);
            await SeedDefautProjectsAsync(dbContextSvc);
            await SeedDefautTicketsAsync(dbContextSvc);
        }

        public static async Task SeedRolesAsync(UserManager<BTUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Roles
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.ProjectManager.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Developer.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Submitter.ToString()));
            //await roleManager.CreateAsync(new IdentityRole(Roles.NewUser.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.DemoUser.ToString()));
        }

        public static async Task SeedDefaultCompaniesAsync(ApplicationDbContext context)
        {
            try
            {
                IList<Company> defaultcompanies = new List<Company>() {
                    new Company() { Name = "Company1", Description="This is default Company 1" },
                    new Company() { Name = "Company2", Description="This is default Company 2" },
                    new Company() { Name = "Company3", Description="This is default Company 3" },
                    new Company() { Name = "Company4", Description="This is default Company 4" },
                    new Company() { Name = "Company5", Description="This is default Company 5" },
                    new Company() { Name = "Company6", Description="This is default Company 6" },
                    new Company() { Name = "Company7", Description="This is default Company 7" }
                };

                var dbCompanies = context.Company.Select(c => c.Name).ToList();
                await context.Company.AddRangeAsync(defaultcompanies.Where(c => !dbCompanies.Contains(c.Name)));
                context.SaveChanges();

                //Get company Ids
                company1Id = context.Company.FirstOrDefault(p => p.Name == "Company1").Id;
                company2Id = context.Company.FirstOrDefault(p => p.Name == "Company2").Id;
                company3Id = context.Company.FirstOrDefault(p => p.Name == "Company3").Id;
                company4Id = context.Company.FirstOrDefault(p => p.Name == "Company4").Id;
                company5Id = context.Company.FirstOrDefault(p => p.Name == "Company5").Id;
                company6Id = context.Company.FirstOrDefault(p => p.Name == "Company6").Id;
                company7Id = context.Company.FirstOrDefault(p => p.Name == "Company7").Id;

            }

            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Companies.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }
        }

        public static async Task SeedDefaultProjectPriorityAsync(ApplicationDbContext context)
        {
            try
            {
                IList<ProjectPriority> projectPriorities = new List<ProjectPriority>() {
                                                            new ProjectPriority() { Name = "Low" },
                                                            new ProjectPriority() { Name = "Medium" },
                                                            new ProjectPriority() { Name = "High" },
                                                            new ProjectPriority() { Name = "Urgent" },
                };

                var dbProjectPriorities = context.ProjectPriority.Select(c => c.Name).ToList();
                await context.ProjectPriority.AddRangeAsync(projectPriorities.Where(c => !dbProjectPriorities.Contains(c.Name)));
                context.SaveChanges();

            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Project Priorities.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }
        }

        public static async Task SeedDefautProjectsAsync(ApplicationDbContext context)
        {
            //Get project priority Ids
            int priorityLow = context.ProjectPriority.FirstOrDefault(p => p.Name == "Low").Id;
            int priorityMedium = context.ProjectPriority.FirstOrDefault(p => p.Name == "Medium").Id;
            int priorityHigh = context.ProjectPriority.FirstOrDefault(p => p.Name == "High").Id;
            int priorityUrgent = context.ProjectPriority.FirstOrDefault(p => p.Name == "Urgent").Id;

            try
            {
                IList<Project> projects = new List<Project>() {
                     new Project()
                     {
                         CompanyId = company1Id,
                         Name = "Build a Personal Porfolio",
                         Description="Single page html, css & javascript page.  Serves as a landing page for candidates and contains a bio and links to all applications and challenges." ,
                         StartDate = new DateTime(2021,4,5),
                         EndDate = new DateTime(2021,4,5).AddMonths(3),
                         ProjectPriorityId = priorityLow
                     },
                     new Project()
                     {
                         CompanyId = company2Id,
                         Name = "Build a supplemental Blog Web Application",
                         Description="Candidate's custom built web application using .Net Core with MVC, a postgres database and hosted in a heroku container.  The app is designed for the candidate to create, update and maintain a live blog site.",
                         StartDate = new DateTime(2021,4,5),
                         EndDate = new DateTime(2021,4,5).AddMonths(3),
                         ProjectPriorityId = priorityMedium
                     },
                     new Project()
                     {
                         CompanyId = company3Id,
                         Name = "Build an Issue Tracking Web Application",
                         Description="A custom designed .Net Core application with postgres database.  The application is a multi tennent application designed to track issue tickets' progress.  Implemented with identity and user roles, Tickets are maintained in projects which are maintained by users in the role of projectmanager.  Each project has a team and team members.",
                         StartDate = new DateTime(2021,4,5),
                         EndDate = new DateTime(2021,4,5).AddMonths(3),
                         ProjectPriorityId = priorityHigh
                     },
                    new Project()
                     {
                         CompanyId = company1Id,
                         Name = "Build a Movie Information Web Application",
                         Description="A custom designed .Net Core application with postgres database.  An API based application allows users to input and import movie posters and details including cast and crew information.",
                         StartDate = new DateTime(2021,4,5),
                         EndDate = new DateTime(2021,4,5).AddMonths(3),
                         ProjectPriorityId = priorityHigh
                     },
                     new Project()
                     {
                         CompanyId = company2Id,
                         Name = "Build an Address Book Web Application",
                         Description="A custom designed .Net Core application with postgres database.  This is an application to serve as a rolodex of contacts for a given user..",
                         StartDate = new DateTime(2021,4,5),
                         EndDate = new DateTime(2021,4,5).AddMonths(3),
                         ProjectPriorityId = priorityHigh
                     }
                };

                var dbProjects = context.Project.Select(c => c.Name).ToList();
                await context.Project.AddRangeAsync(projects.Where(c => !dbProjects.Contains(c.Name)));
                context.SaveChanges();
            }

            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Projects.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }
        }

        public static async Task SeedDefaultProjectStatusAsync(ApplicationDbContext context)
        {
            try
            {
                IList<ProjectStatus> projectStatuses = new List<ProjectStatus>() {
                    new ProjectStatus() { Name = "New" },                 // Newly Created ticket having never been assigned
                    new ProjectStatus() { Name = "Unassigned" },          // Ticket has been assigned at some point but is currently unassigned
                    new ProjectStatus() { Name = "Development" },         // Ticket is assigned and currently being worked 
                    new ProjectStatus() { Name = "Testing" },             // Ticket is assigned and is currently being tested
                    new ProjectStatus() { Name = "Resolved" },           // Ticket remains assigned to the developer but work in now complete
                    new ProjectStatus() { Name = "Archived" }            // Ticket remains assigned to the developer but becomes inactive
                };

                var dbProjectStatuses = context.ProjectStatus.Select(c => c.Name).ToList();
                await context.ProjectStatus.AddRangeAsync(projectStatuses.Where(c => !dbProjectStatuses.Contains(c.Name)));
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
            IConfiguration configuration, IImageService imageService)
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
            IConfiguration configuration, IBTImageService imageService)
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
                Debug.WriteLine("Error Seeding Ticket Types.");
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

/*        public static async Task SeedDefautTicketsAsync(ApplicationDbContext context)
        {
            //Get project Ids
            int portfolioId = context.Project.FirstOrDefault(p => p.Name == "Build a Personal Porfolio").Id;
            int blogId = context.Project.FirstOrDefault(p => p.Name == "Build a supplemental Blog Web Application").Id;
            int bugtrackerId = context.Project.FirstOrDefault(p => p.Name == "Build an Issue Tracking Web Application").Id;

            //Get ticket type Ids
            int typeNewDev = context.TicketType.FirstOrDefault(p => p.Name == "New Development").Id;
            int typeRuntime = context.TicketType.FirstOrDefault(p => p.Name == "Runtime").Id;
            int typeUI = context.TicketType.FirstOrDefault(p => p.Name == "UI").Id;
            int typeMaintenance = context.TicketType.FirstOrDefault(p => p.Name == "Maintenance").Id;

            //Get ticket priority Ids
            int priorityLow = context.TicketPriority.FirstOrDefault(p => p.Name == "Low").Id;
            int priorityMedium = context.TicketPriority.FirstOrDefault(p => p.Name == "Medium").Id;
            int priorityHigh = context.TicketPriority.FirstOrDefault(p => p.Name == "High").Id;
            int priorityUrgent = context.TicketPriority.FirstOrDefault(p => p.Name == "Urgent").Id;

            //Get ticket status Ids
            int statusNew = context.TicketStatus.FirstOrDefault(p => p.Name == "New").Id;
            int statusUnassigned = context.TicketStatus.FirstOrDefault(p => p.Name == "Unassigned").Id;
            int statusDev = context.TicketStatus.FirstOrDefault(p => p.Name == "Development").Id;
            int statusTest = context.TicketStatus.FirstOrDefault(p => p.Name == "Testing").Id;
            int statusResolved = context.TicketStatus.FirstOrDefault(p => p.Name == "Resolved").Id;

            try
            {
                IList<Ticket> tickets = new List<Ticket>() {
                                
                                //PORTFOLIO
                                new Ticket() {Title = "Portfolio Ticket 1", Description = "Ticket details for portfolio ticket 1", Created = DateTimeOffset.Now, ProjectId = portfolioId, TicketPriorityId = priorityLow, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Portfolio Ticket 2", Description = "Ticket details for portfolio ticket 2", Created = DateTimeOffset.Now, ProjectId = portfolioId, TicketPriorityId = priorityMedium, TicketStatusId = statusUnassigned, TicketTypeId = typeMaintenance},
                                new Ticket() {Title = "Portfolio Ticket 3", Description = "Ticket details for portfolio ticket 3", Created = DateTimeOffset.Now, ProjectId = portfolioId, TicketPriorityId = priorityHigh, TicketStatusId = statusDev, TicketTypeId = typeUI},
                                new Ticket() {Title = "Portfolio Ticket 4", Description = "Ticket details for portfolio ticket 4", Created = DateTimeOffset.Now, ProjectId = portfolioId, TicketPriorityId = priorityUrgent, TicketStatusId = statusTest, TicketTypeId = typeRuntime},
                                new Ticket() {Title = "Portfolio Ticket 5", Description = "Ticket details for portfolio ticket 5", Created = DateTimeOffset.Now, ProjectId = portfolioId, TicketPriorityId = priorityLow, TicketStatusId = statusNew, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Portfolio Ticket 6", Description = "Ticket details for portfolio ticket 6", Created = DateTimeOffset.Now, ProjectId = portfolioId, TicketPriorityId = priorityMedium, TicketStatusId = statusUnassigned, TicketTypeId = typeMaintenance},
                                new Ticket() {Title = "Portfolio Ticket 7", Description = "Ticket details for portfolio ticket 7", Created = DateTimeOffset.Now, ProjectId = portfolioId, TicketPriorityId = priorityHigh, TicketStatusId = statusDev, TicketTypeId = typeUI},
                                new Ticket() {Title = "Portfolio Ticket 8", Description = "Ticket details for portfolio ticket 8", Created = DateTimeOffset.Now, ProjectId = portfolioId, TicketPriorityId = priorityUrgent, TicketStatusId = statusTest, TicketTypeId = typeRuntime},
                                
                                //BLOG
                                new Ticket() {Title = "Blog Ticket 1", Description = "Ticket details for blog ticket 1", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityLow, TicketStatusId = statusUnassigned, TicketTypeId = typeRuntime},
                                new Ticket() {Title = "Blog Ticket 2", Description = "Ticket details for blog ticket 2", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityMedium, TicketStatusId = statusDev, TicketTypeId = typeUI},
                                new Ticket() {Title = "Blog Ticket 3", Description = "Ticket details for blog ticket 3", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew, TicketTypeId = typeMaintenance},
                                new Ticket() {Title = "Blog Ticket 4", Description = "Ticket details for blog ticket 4", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityUrgent, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Blog Ticket 5", Description = "Ticket details for blog ticket 5", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityLow, TicketStatusId = statusDev,  TicketTypeId = typeRuntime},
                                new Ticket() {Title = "Blog Ticket 6", Description = "Ticket details for blog ticket 6", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityMedium, TicketStatusId = statusNew,  TicketTypeId = typeUI},
                                new Ticket() {Title = "Blog Ticket 7", Description = "Ticket details for blog ticket 7", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeMaintenance},
                                new Ticket() {Title = "Blog Ticket 8", Description = "Ticket details for blog ticket 8", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityUrgent, TicketStatusId = statusDev,  TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Blog Ticket 9", Description = "Ticket details for blog ticket 9", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityLow, TicketStatusId = statusNew,  TicketTypeId = typeRuntime},
                                new Ticket() {Title = "Blog Ticket 10", Description = "Ticket details for blog ticket 10", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityMedium, TicketStatusId = statusUnassigned, TicketTypeId = typeUI},
                                new Ticket() {Title = "Blog Ticket 11", Description = "Ticket details for blog ticket 11", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityHigh, TicketStatusId = statusDev,  TicketTypeId = typeMaintenance},
                                new Ticket() {Title = "Blog Ticket 12", Description = "Ticket details for blog ticket 12", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityUrgent, TicketStatusId = statusNew,  TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Blog Ticket 13", Description = "Ticket details for blog ticket 13", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityLow, TicketStatusId = statusUnassigned, TicketTypeId = typeRuntime},
                                new Ticket() {Title = "Blog Ticket 14", Description = "Ticket details for blog ticket 14", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityMedium, TicketStatusId = statusDev,  TicketTypeId = typeUI},
                                new Ticket() {Title = "Blog Ticket 15", Description = "Ticket details for blog ticket 15", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityHigh, TicketStatusId = statusNew,  TicketTypeId = typeMaintenance},
                                new Ticket() {Title = "Blog Ticket 16", Description = "Ticket details for blog ticket 16", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityUrgent, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Blog Ticket 17", Description = "Ticket details for blog ticket 17", Created = DateTimeOffset.Now, ProjectId = blogId, TicketPriorityId = priorityHigh, TicketStatusId = statusDev,  TicketTypeId = typeNewDev},
                                
                                //BUGTRACKER                                                                                                                         
                                new Ticket() {Title = "Bug Tracker Ticket 1", Description = "Build Landing Page", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 2", Description = "Ticket details for blog ticket 2", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 3", Description = "Ticket details for blog ticket 3", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 4", Description = "Ticket details for blog ticket 4", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 5", Description = "Ticket details for blog ticket 5", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 6", Description = "Ticket details for blog ticket 6", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 7", Description = "Ticket details for blog ticket 7", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 8", Description = "Ticket details for blog ticket 8", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 9", Description = "Ticket details for blog ticket 9", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 10", Description = "Ticket details for blog ticket 10", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 11", Description = "Ticket details for blog ticket 11", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 12", Description = "Ticket details for blog ticket 12", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 13", Description = "Ticket details for blog ticket 13", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 14", Description = "Ticket details for blog ticket 14", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 15", Description = "Ticket details for blog ticket 15", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 16", Description = "Ticket details for blog ticket 16", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 17", Description = "Ticket details for blog ticket 17", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 18", Description = "Ticket details for blog ticket 18", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 19", Description = "Ticket details for blog ticket 19", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 20", Description = "Ticket details for blog ticket 20", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 21", Description = "Ticket details for blog ticket 21", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 22", Description = "Ticket details for blog ticket 22", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 23", Description = "Ticket details for blog ticket 23", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 24", Description = "Ticket details for blog ticket 24", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 25", Description = "Ticket details for blog ticket 25", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 26", Description = "Ticket details for blog ticket 26", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 27", Description = "Ticket details for blog ticket 27", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 28", Description = "Ticket details for blog ticket 28", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 29", Description = "Ticket details for blog ticket 29", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                                new Ticket() {Title = "Bug Tracker Ticket 30", Description = "Ticket details for blog ticket 30", Created = DateTimeOffset.Now, ProjectId = bugtrackerId, TicketPriorityId = priorityHigh, TicketStatusId = statusUnassigned, TicketTypeId = typeNewDev},
                };

                var dbTickets = context.Ticket.Select(c => c.Title).ToList();
                await context.Ticket.AddRangeAsync(tickets.Where(c => !dbTickets.Contains(c.Title)));
                context.SaveChanges();
            }

            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Ticket.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }
        }
*/
    
    }
}
