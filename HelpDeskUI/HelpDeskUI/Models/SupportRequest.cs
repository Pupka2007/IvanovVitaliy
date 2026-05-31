using System;

namespace HelpDeskUI.Models
{
    public enum RequestStatus
    {
        New,
        InProgress,
        Resolved,
        Cancelled
    }

    public class SupportRequest
    {
        public Guid Id { get; set; }
        public string RequestNumber { get; set; } = "";
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string ClientName { get; set; } = "";
        public string ClientEmail { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public RequestStatus Status { get; set; }
        public string Priority { get; set; } = "Normal";
    }
}