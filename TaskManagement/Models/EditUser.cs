namespace TaskManagement.Models;

public class EditUser
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public int TypeUserId { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
}