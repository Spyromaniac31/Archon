using System.Collections.ObjectModel;

namespace Archon.Services
{
    public static class ErrorReporterService
    {
        public static ObservableCollection<AppError> AppErrors { get; } = new ObservableCollection<AppError>();

        public static void ReportError(string title, string message, string severity)
        {
            AppErrors.Add(new AppError(title, message, severity));
        }
    }

    public class AppError
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string Severity { get; set; }

        public AppError(string title, string message, string severity)
        {
            Title = title;
            Message = message;
            Severity = severity;
        }
    }
}
