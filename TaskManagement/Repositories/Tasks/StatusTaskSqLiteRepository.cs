using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManagement.DataBase;
using TaskManagement.Models;

namespace TaskManagement.Repositories.Tasks;

public class StatusTaskSqLiteRepository : IStatusTaskRepository
{
    private readonly DataBaseContext _dbContext;

    public StatusTaskSqLiteRepository(DataBaseContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<ICollection<StatusTaskInfo>> GetAll()
    {
        try
        {
            return await _dbContext.StatusTasks.Select(t => new StatusTaskInfo()
            {
                Id = t.Id,
                Name = t.Name
            }).ToListAsync();
        }
        catch (Exception e)
        {
            throw new Exception("Произошла ошибка при обращении к базе данных");
        }
    }
}