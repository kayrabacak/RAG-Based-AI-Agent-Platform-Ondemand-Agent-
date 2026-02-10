using System;
using System.Collections.Generic;

namespace OndemandAgent.UI.Dtos
{
    public class AnalyticsSummaryDto
    {
        public int TotalQuestions { get; set; }
        public List<TopQuestionDto> TopQuestions { get; set; } = new();
        public List<EventStatDto> EventStats { get; set; } = new();
    }

    public class TopQuestionDto
    {
        public string Text { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class EventStatDto
    {
        public Guid EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public int QuestionCount { get; set; }
    }
}
