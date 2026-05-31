// Services/RequestService.cs
using HelpDeskApi.Models;
using HelpDeskApi.Repositories;

namespace HelpDeskApi.Services;

/// <summary>
/// Сервис для работы с обращениями
/// </summary>
public class RequestService : IRequestService
{
    private readonly IRequestRepository _repository;

    public RequestService(IRequestRepository repository)
    {
        _repository = repository;
    }

    public Task<List<SupportRequest>> GetAllAsync() => _repository.GetAllAsync();

    public Task<SupportRequest?> GetByIdAsync(Guid id) => _repository.GetByIdAsync(id);

    public async Task<SupportRequest> CreateAsync(SupportRequest request)
    {
        request.Id = Guid.NewGuid();
        request.CreatedAt = DateTime.Now;
        request.Status = RequestStatus.New;

        await _repository.AddAsync(request);
        return request;
    }

    public async Task<bool> UpdateAsync(Guid id, SupportRequest request)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return false;

        request.Id = id;
        request.CreatedAt = existing.CreatedAt;

        await _repository.UpdateAsync(request);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return false;

        await _repository.DeleteAsync(id);
        return true;
    }

    public async Task<bool> ChangeStatusAsync(Guid id, RequestStatus status)
    {
        var request = await _repository.GetByIdAsync(id);
        if (request == null) return false;

        request.Status = status;
        await _repository.UpdateAsync(request);
        return true;
    }
}