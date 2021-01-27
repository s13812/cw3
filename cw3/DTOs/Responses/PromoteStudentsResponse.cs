using System;

namespace cw3.DTOs.Responses
{
    public class PromoteStudentsResponse
    {
        public int IdEnrollment { get; set; }
        public int Semester { get; set; }
        public int IdStudy { get; set; }
        public string StartDate { get; set; }
    }
}
