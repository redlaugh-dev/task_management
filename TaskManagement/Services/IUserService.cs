using System.Threading.Tasks;
using TaskManagement.Models;

namespace TaskManagement.Services;

public interface IUserService
{
    public Task LogIn(string username, string password);
    public Task<UserInfo> GetUserInfo();
    public int UserToken { get; }
}