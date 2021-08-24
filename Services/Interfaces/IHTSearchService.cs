using HonuTasks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HonuTasks.Services.Interfaces
{
    public interface IHTSearchService
    {
        public IOrderedQueryable<Tasks> SearchContent(string searchString);
    }
}
