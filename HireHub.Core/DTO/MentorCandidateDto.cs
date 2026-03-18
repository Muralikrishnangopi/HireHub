using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HireHub.Core.Data.Models;

namespace HireHub.Core.DTO
{
    public class MentorCandidateDto
    {
        public int CandidateId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? Address { get; set; }
        public string? College { get; set; }
        public string? PreviousCompany { get; set; }
        public List<string>? Skills { get; set; } = [];
        public string? AttendanceStatus { get; set; }
        public CandidateStatus Status { get; set; }
        public int RoundId { get; set; }

        public string? RoundType { get; set; }
        public string? RoundStatus { get; set; }
        public string? RoundResult { get; set; }

        public int DriveCandidateId { get; set; }

        public int? InterviewerId { get; set; }
        public string? InterviewerName { get; set; }
        public string? InterviewerEmail { get; set; }
        public string? InterviewerPhone { get; set; }
    }
}
