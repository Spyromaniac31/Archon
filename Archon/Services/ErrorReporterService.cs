using System.Collections.ObjectModel;

namespace Archon.Services
{
    public static class ErrorReporterService
    {
        public static ObservableCollection<AppError> AppErrors { get; } = new ObservableCollection<AppError>();

        public static void ReportError(string title, string message, string severity)
        {
            AppError newError = new AppError(title, message, severity);
            bool alreadyReported = false;
            foreach (AppError appError in AppErrors)
            {
                if (appError.Equals(newError))
                {
                    alreadyReported = true;
                }
            }
            if (!alreadyReported)
            {
                AppErrors.Add(newError);
            }
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

        public bool Equals(AppError other)
        {
            return Title == other.Title && Message == other.Message && Severity == other.Severity;
        }

        public void RemoveFromList()
        {
            _ = ErrorReporterService.AppErrors.Remove(this);
        }
    }
}
