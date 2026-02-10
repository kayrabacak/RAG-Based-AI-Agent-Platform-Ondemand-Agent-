// Logları listelemek için kullanacağımız sınıf
    public class ChatLogDto
    {
        public string UserQuestion { get; set; } = string.Empty;
        public string AgentAnswer { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }