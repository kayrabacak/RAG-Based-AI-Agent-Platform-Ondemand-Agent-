namespace OndemandAgent.UI.Dtos
{
    public class EventDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        
        
        public List<DocumentDto> Documents { get; set; } = new List<DocumentDto>();
    }

    public class DocumentDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public object Status { get; set; } = string.Empty; 
        public string StatusDisplay => Status?.ToString() ?? "İşleniyor";
    }

   
    public class CreateEventRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(7);
        public bool IsActive { get; set; } = true;
    }

    public class ChatLogDto
    {
        public string UserQuestion { get; set; } = string.Empty;
        public string AgentAnswer { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}