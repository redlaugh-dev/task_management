using System;

namespace TaskManagement.Models;

public class TaskInfo
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public UserInfo AuthorInfo { get; set; }
    public UserInfo ExecutorInfo { get; set; }
    public StatusTaskInfo Status { get; set; }
    public DateOnly Date { get; set; }
}