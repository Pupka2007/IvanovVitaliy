// Models/RequestStatus.cs
namespace HelpDeskApi.Models;

/// <summary>
/// Статусы обращений в техподдержку
/// </summary>
public enum RequestStatus
{
    New = 0,           // Новое
    InProgress = 1,    // В работе  
    Resolved = 2,      // Решено
    Cancelled = 3      // Отменено
}