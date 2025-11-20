using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagement.Models;

namespace TaskManagement.Repositories.Users;

public interface ITypeUserRepository
{
    public Task<ICollection<TypeUserInfo>> GetAll();
}