using System;

namespace TaskManagement.Models;

public class MenuItem
{
    public string Title { get; set; }
    public string Icon { get; set; }
    public Action Action { get; set; }

    public MenuItem(string title, string icon, Action action)
    {
        Title = title;
        Icon = icon;
        Action = action;
    }
}