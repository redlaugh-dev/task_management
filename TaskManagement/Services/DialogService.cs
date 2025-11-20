using System.Threading.Tasks;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Models;

namespace TaskManagement.Services;

public class DialogService : IDialogService
{
    public async Task<DialogResult> ShowDialog(string title, string message, DialogButtons buttons)
    {
        var messageBox = MessageBoxManager.GetMessageBoxCustom(new MessageBoxCustomParams()
        {
            ContentTitle = title,
            ContentMessage = message,
            MaxWidth = 450,
            ButtonDefinitions = buttons == DialogButtons.Ok
                ? new ButtonDefinition[]
                {
                    new ButtonDefinition { Name = "Ок", }
                }
                : new[] { new ButtonDefinition { Name = "Да", }, new ButtonDefinition { Name = "Нет", } }
        });
        var result = await messageBox.ShowWindowAsync();
        switch (result)
        {
            case "Ок":
                return DialogResult.Ok;
            case "Да":
                return DialogResult.Yes;
            case "Нет":
                return DialogResult.No;
        }
        return DialogResult.Cancel;
    }
}