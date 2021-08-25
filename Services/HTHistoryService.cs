using HonuTasks.Data;
using HonuTasks.Models;
using HonuTasks.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HonuTasks.Services
{
    public class HTHistoryService : IHTHistoryService
    {
        private readonly ApplicationDbContext _context;

        public HTHistoryService(ApplicationDbContext context
            )
        {
            _context = context;
        }

        public async Task AddHistoryAsync(Tasks oldTask, Tasks newTask, string userId)
        {
            //NEW Task HAS BEEN ADDED
            if (oldTask == null && newTask != null) //Old Task is null = no prev task, means there's a New Task
            {
                TaskHistory history = new()
                {
                    TaskId = newTask.Id,
                    Property = "",
                    OldValue = "",
                    NewValue = "",
                    Created = DateTimeOffset.Now,
                    UserId = userId,
                    Description = "New Task Created"
                };
                await _context.TaskHistory.AddAsync(history);
                await _context.SaveChangesAsync();
            }

            else
            {
                //Check for a Task Title
                if (oldTask.Title != newTask.Title)
                {
                    TaskHistory history = new()
                    {
                        TaskId = newTask.Id,
                        Property = "Title",
                        OldValue = oldTask.Title,
                        NewValue = newTask.Title,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New Task Title: {newTask.Title}"
                    };
                    await _context.TaskHistory.AddAsync(history);
                }

                //Check for a Task Description
                if (oldTask.Description != newTask.Description)
                {
                    TaskHistory history = new()
                    {
                        TaskId = newTask.Id,
                        Property = "Description",
                        OldValue = oldTask.Description,
                        NewValue = newTask.Description,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New Task Description: {newTask.Description}"
                    };
                    await _context.TaskHistory.AddAsync(history);
                }

                //Check for a Ticket Type
                if (oldTask.TaskTypeId != newTask.TaskTypeId)
                {
                    TaskHistory history = new()
                    {
                        TaskId = newTask.Id,
                        Property = "Task Type",
                        OldValue = oldTask.TaskType.Name,
                        NewValue = newTask.TaskType.Name,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New Task Type: {newTask.TaskTypeId}"
                    };
                    await _context.TaskHistory.AddAsync(history);
                }
                //Check for a Task Priority
                if (oldTask.TaskPriorityId != newTask.TaskPriorityId)
                {
                    TaskHistory history = new()
                    {
                        TaskId = newTask.Id,
                        Property = "Task Priority",
                        OldValue = oldTask.TaskPriority.Name,
                        NewValue = newTask.TaskPriority.Name,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New Task Priority: {newTask.TaskPriority.Name}"
                    };
                    await _context.TaskHistory.AddAsync(history);
                }

                //Check for a Task Status
                if (oldTask.TaskStatusId != newTask.TaskStatusId)
                {
                    TaskHistory history = new()
                    {
                        TaskId = newTask.Id,
                        Property = "Task Status",
                        OldValue = oldTask.TaskStatus.ToString(),
                        NewValue = newTask.TaskStatus.ToString(),
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New Task Status: {newTask.TaskStatus}"
                    };
                    await _context.TaskHistory.AddAsync(history);
                }

                //Check for a Task User
                if (oldTask.AssignedUserId != newTask.AssignedUserId)
                {
                    TaskHistory history = new()
                    {
                        TaskId = newTask.Id,
                        Property = "Assigned User",
                        OldValue = oldTask.AssignedUser?.FullName ?? "Not Assigned",
                        NewValue = newTask.AssignedUser?.FullName,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New Assigned User: {newTask.AssignedUser.FullName}"
                    };
                    await _context.TaskHistory.AddAsync(history);
                }

                //Save the TaskHistory DataBaseSet to the database
                await _context.SaveChangesAsync();
            }
        }

        //Get the task history for a s specific Event for a SPECIFIC CREATOR
        public async Task<List<TaskHistory>> GetCreatorTaskHistoriesAsync(int creatorId)
        {
            Creator creator = await _context.Creator
                                                    .Include(c => c.Events)
                                                        .ThenInclude(p => p.Tasks)
                                                            .ThenInclude(t => t.History)
                                                    .FirstOrDefaultAsync(c => c.Id == creatorId);

            List<TaskHistory> taskHistory = creator.Events.SelectMany(p => p.Tasks).SelectMany(t => t.History).ToList();

            return taskHistory;
        }

        //Get the Task History for a SPECIFIC Task for a Specific Event
        public async Task<List<TaskHistory>> GetEventTaskHistoriesAsync(int eventId)
        {
            Events events = await _context.Events
                                                .Include(p => p.Tasks)
                                                    .ThenInclude(t => t.History)
                                                .FirstOrDefaultAsync(p => p.Id == eventId);

            List<TaskHistory> taskHistory = events.Tasks.SelectMany(t => t.History).ToList();

            return taskHistory;
        }
    }
}
