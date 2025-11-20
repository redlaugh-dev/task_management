using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagement.Models;

namespace TaskManagement.Repositories.Tasks;

public interface IStatusTaskRepository
{
    public Task<ICollection<StatusTaskInfo>> GetAll();
}