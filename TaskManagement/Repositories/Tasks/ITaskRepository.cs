using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagement.Models;

namespace TaskManagement.Repositories.Tasks;

public interface ITaskRepository<T>
{
    public Task<ICollection<TaskInfo>> GetAll(T userToken, TaskFilters filters = null);
    public Task<EditTask> GetById(int idTask);
    public Task UpdateStatusTask(int idTask, int IdStatus);
    public Task Remove(int idTask);
    public Task Add(EditTask task, T userToken);
    public Task Update(EditTask task);
}