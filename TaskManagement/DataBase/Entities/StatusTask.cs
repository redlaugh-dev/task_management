using System.Collections.Generic;

namespace TaskManagement.DataBase.Entities;

public class StatusTask
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    
    public virtual ICollection<EmployeeTask> Tasks { get; set; } = null!;
}