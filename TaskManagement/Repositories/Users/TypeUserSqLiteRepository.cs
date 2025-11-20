using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManagement.DataBase;
using TaskManagement.Models;

namespace TaskManagement.Repositories.Users;

public class TypeUserSqLiteRepository : ITypeUserRepository
{
    private readonly DataBaseContext _dataBaseContext;

    public TypeUserSqLiteRepository(DataBaseContext dataBaseContext)
    {
        _dataBaseContext = dataBaseContext; 
    }
    public async Task<ICollection<TypeUserInfo>> GetAll()
    {
        try
        {
            return await _dataBaseContext.TypeUsers.Select(t => new TypeUserInfo()
            {
                Id = t.Id,
                Name = t.Type
            }).ToListAsync();
        }
        catch (Exception)
        {
            throw new Exception("Произошла ошибка при обращении к базе данных");
        }
    }
}