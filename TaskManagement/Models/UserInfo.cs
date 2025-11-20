namespace TaskManagement.Models;

public class UserInfo
{
    public int Id { get; set; }
    public string Surname { get; set; }
    public string Name { get; set; }
    public TypeUserInfo TypeUser { get; set; }
}