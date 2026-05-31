using System.Text.Json;
using HelpDeskApi.Models;

namespace HelpDeskApi.Repositories;

/// <summary>
/// Репозиторий для работы с JSON файлом
/// </summary>
public class JsonRequestRepository : IRequestRepository
{
    private readonly string _filePath;

    public JsonRequestRepository(string filePath)
    {
        _filePath = filePath;
    }

    public async Task<List<SupportRequest>> GetAllAsync()
    {
        if (!File.Exists(_filePath))
            return new List<SupportRequest>();

        var json = await File.ReadAllTextAsync(_filePath);
        return JsonSerializer.Deserialize<List<SupportRequest>>(json) ?? new List<SupportRequest>();
    }

    public async Task<SupportRequest?> GetByIdAsync(Guid id)
    {
        var requests = await GetAllAsync();
        return requests.FirstOrDefault(r => r.Id == id);
    }

    public async Task AddAsync(SupportRequest request)
    {
        var requests = await GetAllAsync();
        requests.Add(request);
        await SaveAllAsync(requests);
    }

    public async Task UpdateAsync(SupportRequest request)
    {
        var requests = await GetAllAsync();
        var index = requests.FindIndex(r => r.Id == request.Id);
        if (index >= 0)
        {
            requests[index] = request;
            await SaveAllAsync(requests);
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var requests = await GetAllAsync();
        requests.RemoveAll(r => r.Id == id);
        await SaveAllAsync(requests);
    }

    private async Task SaveAllAsync(List<SupportRequest> requests)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(requests, options);
        await File.WriteAllTextAsync(_filePath, json);
    }
}