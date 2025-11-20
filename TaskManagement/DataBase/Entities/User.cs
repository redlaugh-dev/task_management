using System.Collections.Generic;

namespace TaskManagement.DataBase.Entities;

public class User
{
    public int Id { get; set; }
    public string Login { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int TypeUserId { get; set; }
    
    public virtual UserType UserType { get; set; } = null!;
    public virtual ICollection<EmployeeTask> AuthorTasks { get; set; } = null!;
    public virtual ICollection<EmployeeTask> ExecutorTasks { get; set; } = null!;
}