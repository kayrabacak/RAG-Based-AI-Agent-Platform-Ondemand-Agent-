using System;

namespace OndemandAgent.Web.Dtos
{
    public class CreateEventRequest
    {
        public required string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public required string Description { get; set; }
    }
}