using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagement.Models;

namespace TaskManagement.Repositories.Users;

public interface IUserRepository
{
    public Task<ICollection<UserInfo>> GetAll();
    public Task<ICollection<UserInfo>> FindUsers(string searchString);
    public Task<ICollection<UserInfo>> GetAllEmployeeWorkers();
    public Task AddUser(EditUser user);
    public Task<EditUser> GetUserById(int id);
    public Task UpdateUser(EditUser user);
    public Task RemoveUser(int userId);
}