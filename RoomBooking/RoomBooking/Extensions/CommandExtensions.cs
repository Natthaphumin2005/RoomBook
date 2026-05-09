using System.Windows.Input;

namespace RoomBooking.Extensions
{
    /// <summary>
    /// Extension methods สำหรับ ICommand เพื่อรองรับ async execution
    /// </summary>
    public static class CommandExtensions
    {
        /// <summary>Execute ICommand แบบ async</summary>
        public static async Task ExecuteAsync(this ICommand command, object parameter = null)
        {
            if (command.CanExecute(parameter))
            {
                command.Execute(parameter);
                await Task.CompletedTask;
            }
        }
    }
}
