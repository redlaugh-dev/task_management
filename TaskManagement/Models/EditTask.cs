using System;

namespace TaskManagement.Models;

public class EditTask
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateOnly Date { get; set; }
    public int ExecutorId { get; set; }
    public int StatusId { get; set; }
}