// Models/SupportRequest.cs
using System;

namespace HelpDeskApi.Models;

/// <summary>
/// Модель обращения в техподдержку
/// </summary>
public class SupportRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string ClientEmail { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public RequestStatus Status { get; set; } = RequestStatus.New;
    public string Priority { get; set; } = "Normal";
}