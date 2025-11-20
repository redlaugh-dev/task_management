using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManagement.DataBase;
using TaskManagement.DataBase.Entities;
using TaskManagement.Models;

namespace TaskManagement.Repositories.Tasks;

public class TaskSqLiteRepository : ITaskRepository<int>
{
    private readonly DataBaseContext _dbContext;

    public TaskSqLiteRepository(DataBaseContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<ICollection<TaskInfo>> GetAll(int userToken, TaskFilters filters = null)
    {
        int userRole;
        try
        {
            userRole = _dbContext.Users.Where(u => u.Id == userToken)
                .Select(u => u.TypeUserId)
                .SingleOrDefault();
        }
        catch (Exception ex)
        {
            throw new Exception("Произошла ошибка при обращении к базе данных");
        }

        if (userRole == 0)
        {
            throw new Exception("Не удалось определить пользователя запроса");
        }
        IQueryable<EmployeeTask> query;
        if (userRole == 1)
        {
            query = _dbContext.EmployeeTasks.Where(p=>p.ExecutorId == userToken);
        }
        else if (userRole == 2)
        {
            query = _dbContext.EmployeeTasks.Where(p=>p.AuthorId == userToken);
        }
        else
        {
            query = _dbContext.EmployeeTasks;
        }

        if (filters != null)
        {
            if (!String.IsNullOrEmpty(filters.SearchString))
            {
                string lowerSearch = filters.SearchString.ToLower();
                query = query.Where(p => p.Title.ToLower().Contains(lowerSearch)
                || p.Description.ToLower().Contains(lowerSearch));
            }
            if (filters.ExecutorId != null)
            {
                query = query.Where(p => p.ExecutorId == filters.ExecutorId);
            }
            if (filters.Date != null)
            {
                query = query.Where(p => p.DateCreate ==  filters.Date);
            }

            if (filters.StatusId != null)
            {
                query = query.Where(p => p.StatusTaskId == filters.StatusId);
            }
        }

        try
        {
            return await query.Include(p=>p.Executor)
                .Include(p => p.StatusTask)
                .Include(p => p.Author)
                .Select(p => new TaskInfo()
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                Date = p.DateCreate,
                ExecutorInfo = new UserInfo()
                {
                    Name = p.Executor.Name,
                    Surname = p.Executor.Surname,
                },
                AuthorInfo = new UserInfo()
                {
                    Name = p.Author.Name,
                    Surname = p.Author.Surname,
                },
                Status = new StatusTaskInfo()
                {
                    Id = p.StatusTaskId,
                    Name = p.StatusTask.Name,
                }
            }).ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Произошла ошибка при обращении к базе данных");
        }
    }

    public async Task<EditTask> GetById(int idTask)
    {
        try
        {
            var task =  await _dbContext.EmployeeTasks.Select(p => new EditTask()
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                Date = p.DateCreate,
                ExecutorId = p.ExecutorId,
                StatusId = p.StatusTaskId,
            })
            .FirstOrDefaultAsync(p => p.Id == idTask);
            if (task == null)
            {
                throw new Exception("Задача не найдена в базе данных");
            }

            return task;
        }
        catch (Exception ex)
        {
            throw new Exception("Произошла ошибка при обращении к базе данных");
        }
        
    }

    public async Task UpdateStatusTask(int idTask, int IdStatus)
    {
        EmployeeTask? task;
        try
        {
            task = await _dbContext.EmployeeTasks.FindAsync(idTask);
        }
        catch (Exception ex)
        {
            throw new Exception("Произошла ошибка при обращении к базе данных");
        }

        if (task == null)
        {
            throw new Exception("Не существует задачи с указанным id");
        }

        try
        {
            task.StatusTaskId = IdStatus;
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new Exception("Указанный статус отсуствует в базе данных");
        }
        catch (Exception e)
        {
            _dbContext.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified)
                .ToList()
                .ForEach(e => e.State = EntityState.Unchanged);
            throw new Exception("Произошла ошибка при обращении к базе данных");
        }
    }

    public async Task Remove(int idTask)
    {
        EmployeeTask? task;
        try
        {
            task = await _dbContext.EmployeeTasks.FindAsync(idTask);
        }
        catch (Exception ex)
        {
            throw new Exception("Произошла ошибка при обращении к базе данных");
        }

        if (task == null)
        {
            throw new Exception("Не существует задачи с указанным id");
        }

        try
        {
            _dbContext.EmployeeTasks.Remove(task);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _dbContext.ChangeTracker.Entries().Where(e => e.State == EntityState.Deleted)
                .ToList()
                .ForEach(e => e.State = EntityState.Unchanged);
            throw new Exception("Произошла ошибка при обращении к базе данных");
        }
    }

    public async Task Add(EditTask editTask, int userToken)
    {
        EmployeeTask task = new EmployeeTask()
        {
            Title = editTask.Title,
            Description = editTask.Description,
            StatusTaskId = 1,
            ExecutorId = editTask.ExecutorId,
            AuthorId = userToken,
            DateCreate = editTask.Date
        };
        try
        {
            await _dbContext.EmployeeTasks.AddAsync(task);
            await _dbContext.SaveChangesAsync();
            
        }
        catch (Exception e)
        {
            _dbContext.ChangeTracker.Entries().Where(e => e.State == EntityState.Added)
                .ToList()
                .ForEach(e => e.State = EntityState.Detached);
            throw new Exception("Произошла ошибка при обращении к базе данных");
        }
    }

    public async Task Update(EditTask editTask)
    {
        EmployeeTask? task;
        try
        {
            task = await _dbContext.EmployeeTasks.FindAsync(editTask.Id);
        }
        catch (Exception ex)
        {
            throw new Exception("Произошла ошибка при обращении к базе данных");
        }

        if (task == null)
        {
            throw new Exception("Не существует задачи с указанным id");
        }
        try
        {
            task.Title = editTask.Title;
            task.Description = editTask.Description;
            task.DateCreate = editTask.Date;
            task.ExecutorId = editTask.ExecutorId;
            task.StatusTaskId = editTask.StatusId;
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _dbContext.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified)
                .ToList()
                .ForEach(e => e.State = EntityState.Unchanged);
            throw new Exception("Произошла ошибка при обращении к базе данных");
        }
    }
}