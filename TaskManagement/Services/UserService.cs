using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManagement.DataBase;
using TaskManagement.DataBase.Entities;
using TaskManagement.Models;

namespace TaskManagement.Services;

public class UserService : IUserService
{
    
    private int? _idUser; // Для простоты использую id как токен пользователя
    private readonly DataBaseContext _dbContext;

    public UserService(DataBaseContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task LogIn(string login, string password)
    {
        User? user = null;
        try
        {
            user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Login == login &&  u.Password == password);
        }
        catch (Exception ex)
        {
            throw new Exception("Произошла ошибка при обращении к базе данных");
        }
        if (user == null)
        {
            throw new Exception("Неверный логин или пароль!");
        }
        _idUser = user.Id;
    }

    public async Task<UserInfo> GetUserInfo()
    {
        UserInfo? userInfo = null;
        try
        {
            userInfo = await _dbContext.Users.Where(u => u.Id == _idUser)
                .Include(u => u.UserType)
                .Select(u => new UserInfo()
                {
                    Name = u.Name,
                    Surname = u.Surname,
                    TypeUser = new TypeUserInfo()
                    {
                        Id = u.TypeUserId,
                        Name = u.UserType.Type
                    }
                })
                .SingleOrDefaultAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Произошла ошибка при обращении к базе данных");
        }
        if (userInfo == null)
        {
            throw new Exception("Пользователь не найден");
        }
        return userInfo;
    }

    public int UserToken => _idUser.Value;
}