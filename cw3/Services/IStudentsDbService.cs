﻿using cw3.DTOs;
using cw3.DTOs.Requests;
using cw3.DTOs.Responses;
using cw3.Models;
using System.Collections.Generic;

namespace cw3.Services
{
    public interface IStudentsDbService
    {
        public IEnumerable<Student> GetStudents();
        public Enrollment GetEnrollment(string indexNumber);
        public EnrollStudentResponse EnrollStudent(EnrollStudentRequest request);
        public PromoteStudentsResponse PromoteStudents(PromoteStudentsRequest request);
        public bool IsStudentInDb(string indexNumber);
        public LoginDetailsDTO GetLoginDetails(string indexNumber);
        public void SetRefreshToken(string indexNumber, string refreshToken);
        public LoginDetailsDTO GetLoginDetailsFromRefreshToken(string refToken);
    }
}
