using HonuTasks.Data;
using HonuTasks.Models;
using HonuTasks.Services.Interfaces;
using HonuTasks.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HonuTasks.Services
{
    public class SearchService : IHTSearchService
    {
        private readonly ApplicationDbContext _context;

        public SearchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IOrderedQueryable<Tasks> SearchContent(string searchString)
        {
            //Step One: Get an IQueryable that contains all the Tickets
            //in the event that the User doesn't supply a search string
            var result = _context.Tasks.Where(
                p => p.TasksStatuses == TasksStatuses.Created);
            searchString = searchString.ToLower();

            if (!string.IsNullOrEmpty(searchString))
            {
                //c.Moderated == null &&
                //c.Author.FullName.Contains(searchString)
                //c.Author.Email.Contains(searchString)

                result = result.Where(p => p.Title.ToLower().Contains(searchString) ||
                                           p.Description.ToLower().Contains(searchString)
                                      );

            }

            return result.OrderByDescending(p => p.Created);
        }

    }
}
