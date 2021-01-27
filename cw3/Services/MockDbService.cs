using cw3.DTOs.Requests;
using cw3.DTOs.Responses;
using cw3.Models;
using System;
using System.Collections.Generic;

namespace cw3.Services
{
    public class MockDbService : IStudentsDbService
    {
        private static readonly IEnumerable<Student> _students;

        static MockDbService()
        {
            _students = new List<Student>
            {
                new Student{FirstName="Jan", LastName="Kowalski"},
                new Student{FirstName="Anna", LastName="Malewski"},
                new Student{FirstName="Andrzej", LastName="Andrzejewicz"}
            };
        }

        public EnrollStudentResponse EnrollStudent(EnrollStudentRequest request)
        {
            throw new NotImplementedException();
        }

        public Enrollment GetEnrollment(string indexNumber)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Student> GetStudents()
        {
            return _students;
        }

        public bool IsStudentInDb(string indexNumber)
        {
            return true;
        }

        public PromoteStudentsResponse PromoteStudents(PromoteStudentsRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
