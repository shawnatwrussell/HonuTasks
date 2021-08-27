using HonuTasks.Data;
using HonuTasks.Models;
using HonuTasks.Models.ViewModels;
using HonuTasks.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace HonuTasks.Controllers
{
    [Authorize]

    public class HomeController : Controller
    {
    }
}

