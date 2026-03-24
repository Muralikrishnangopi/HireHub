using HireHub.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HireHub.Core.DTO
{
    public class CandidateFeedbackDto
    {
        public int RoundId { get; set; }
        public string? RoundType { get; set; }
        public string? Result { get; set; }

        public int FeedbackId { get; set; }
        public int? OverallRating { get; set; }
        public string? TechnicalSkill { get; set; }
        public string? Communication { get; set; }
        public string? ProblemSolving { get; set; }
        public string? OverallFeedback { get; set; }
        public Recommendation Recommendation { get; set; }
        public DateTime SubmittedDate { get; set; }
    }
}
