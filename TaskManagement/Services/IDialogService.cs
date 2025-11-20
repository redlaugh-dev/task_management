using System.Threading.Tasks;


namespace TaskManagement.Services;

public enum DialogButtons
{
    Ok, 
    YesNo,
}

public enum DialogResult
{
    Ok, 
    Yes,
    No,
    Cancel
}

public interface IDialogService
{
    public Task<DialogResult> ShowDialog(string title, string message,  DialogButtons buttons);
}