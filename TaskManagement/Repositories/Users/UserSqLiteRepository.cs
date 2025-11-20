using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManagement.DataBase;
using TaskManagement.DataBase.Entities;
using TaskManagement.Models;

namespace TaskManagement.Repositories.Users;

public class UserSqLiteRepository : IUserRepository
{
    private readonly DataBaseContext _dataBaseContext;

    public UserSqLiteRepository(DataBaseContext dataBaseContext)
    {
        _dataBaseContext = dataBaseContext;
    }
    public async Task<ICollection<UserInfo>> GetAll()
    {
        try
        {
            return await _dataBaseContext.Users
                .Include(u => u.UserType)
                .Select(u => new UserInfo()
                {
                    Id = u.Id,
                    Name = u.Name,
                    Surname = u.Surname,
                    TypeUser = new TypeUserInfo()
                    {
                        Id = u.TypeUserId,
                        Name = u.UserType.Type
                    }
                }).ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Произошла ошибка при обращении к базе данных");
        }
    }

    public async Task<ICollection<UserInfo>> FindUsers(string searchString)
    {
        try
        {
            string searchStringLower = searchString.ToLower();
            return await _dataBaseContext.Users
                .Include(u => u.UserType)
                .Where(u => u.Surname.ToLower().Contains(searchStringLower) || u.Name.ToLower().Contains(searchStringLower))
                .Select(u => new UserInfo()
                {
                    Id = u.Id,
                    Name = u.Name,
                    Surname = u.Surname,
                    TypeUser = new TypeUserInfo()
                    {
                        Id = u.TypeUserId,
                        Name = u.UserType.Type
                    }
                }).ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Произошла ошибка при обращении к базе данных");
        }
    }

    public async Task<ICollection<UserInfo>> GetAllEmployeeWorkers()
    {
        try
        {
            return await _dataBaseContext.Users
                .Include(u => u.UserType)
                .Where(u => u.TypeUserId == 1)
                .Select(u => new UserInfo()
                {
                    Id = u.Id,
                    Name = u.Name,
                    Surname = u.Surname,
                    TypeUser = new TypeUserInfo()
                    {
                        Id = u.TypeUserId,
                        Name = u.UserType.Type
                    }
                }).ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Произошла ошибка при обращении к базе данных");
        }
    }

    public async Task AddUser(EditUser editUser)
    {
        User user = new User()
        {
            Name = editUser.Name,
            Surname = editUser.Surname,
            Login = editUser.Login,
            Password = editUser.Password,
            TypeUserId = editUser.TypeUserId,
        };
        try
        {
            await _dataBaseContext.Users.AddAsync(user);
            await _dataBaseContext.SaveChangesAsync();
            
        }
        catch (DbUpdateException e)
        {
            _dataBaseContext.ChangeTracker.Entries().Where(e => e.State == EntityState.Added)
                .ToList()
                .ForEach(e => e.State = EntityState.Detached);
            throw new Exception("Данный логин уже используется в системе");
        }
        catch (Exception e)
        {
            _dataBaseContext.ChangeTracker.Entries().Where(e => e.State == EntityState.Added)
                .ToList()
                .ForEach(e => e.State = EntityState.Detached);
            throw new Exception("Произошла ошибка при обращении к базе данных");
        }
    }

    public async Task<EditUser> GetUserById(int id)
    {
        EditUser? user;
        try
        {
            user = await _dataBaseContext.Users.Select(u => new EditUser()
                {
                    Id = u.Id, 
                    Name = u.Name,
                    Surname = u.Surname,
                    TypeUserId = u.TypeUserId,
                    Login = u.Login, 
                    Password = u.Password
                })
                .FirstAsync(u => u.Id == id);
        }
        catch (Exception ex)
        {
            throw new Exception("Произошла ошибка при обращении к базе данных");
        }

        if (user == null)
        {
            throw new Exception("Не существует пользователя с указанным id");
        }

        return user;
    }

    public async Task UpdateUser(EditUser editUser)
    {
        User? user;
        try
        {
            user = await _dataBaseContext.Users.FindAsync(editUser.Id);
        }
        catch (Exception ex)
        {
            throw new Exception("Произошла ошибка при обращении к базе данных");
        }

        if (user == null)
        {
            throw new Exception("Не существует пользователя с указанным id");
        }

        try
        {
            user.Name = editUser.Name;
            user.Surname = editUser.Surname;
            user.TypeUserId = editUser.TypeUserId;
            user.Login = editUser.Login;
            user.Password = editUser.Password;
            await _dataBaseContext.SaveChangesAsync();
        }
        catch (DbUpdateException e)
        {
            _dataBaseContext.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified)
                .ToList()
                .ForEach(e => e.State = EntityState.Unchanged);
            throw new Exception("Данный логин уже используется в системе");
        }
        catch (Exception e)
        {
            _dataBaseContext.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified)
                .ToList()
                .ForEach(e => e.State = EntityState.Unchanged);
            throw new Exception("Произошла ошибка при обращении к базе данных");
        }
    }

    public async Task RemoveUser(int userId)
    {
        User? user;
        try
        {
            user = await _dataBaseContext.Users.FindAsync(userId);
        }
        catch (Exception ex)
        {
            throw new Exception("Произошла ошибка при обращении к базе данных");
        }

        if (user == null)
        {
            throw new Exception("Не существует пользователя с указанным id");
        }

        try
        {
            _dataBaseContext.Users.Remove(user);
            await _dataBaseContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _dataBaseContext.ChangeTracker.Entries().Where(e => e.State == EntityState.Deleted)
                .ToList()
                .ForEach(e => e.State = EntityState.Unchanged);
            throw new Exception("Произошла ошибка при обращении к базе данных");
        }
    }
}