// Services/IRequestService.cs
using HelpDeskApi.Models;

namespace HelpDeskApi.Services;

/// <summary>
/// Интерфейс сервиса обращений
/// </summary>
public interface IRequestService
{
    Task<List<SupportRequest>> GetAllAsync();
    Task<SupportRequest?> GetByIdAsync(Guid id);
    Task<SupportRequest> CreateAsync(SupportRequest request);
    Task<bool> UpdateAsync(Guid id, SupportRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ChangeStatusAsync(Guid id, RequestStatus status);
}