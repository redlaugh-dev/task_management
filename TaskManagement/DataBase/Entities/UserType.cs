using System.Collections.Generic;

namespace TaskManagement.DataBase.Entities;

public class UserType
{
    public int Id { get; set; }
    public string Type { get; set; }
    
    public virtual ICollection<User> Users { get; set; }  = null!;
}