// Repositories/IRequestRepository.cs
using HelpDeskApi.Models;

namespace HelpDeskApi.Repositories;

/// <summary>
/// Интерфейс репозитория обращений
/// </summary>
public interface IRequestRepository
{
    Task<List<SupportRequest>> GetAllAsync();
    Task<SupportRequest?> GetByIdAsync(Guid id);
    Task AddAsync(SupportRequest request);
    Task UpdateAsync(SupportRequest request);
    Task DeleteAsync(Guid id);
}