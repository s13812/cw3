using cw3.DAL;
using cw3.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace cw3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private IDbService _dbService;

        public StudentsController(IDbService service)
        {
            _dbService = service;
        }

        private const string ConString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=s13812;Integrated Security=True";

        [HttpGet]
        public IActionResult GetStudents()
        {
            //return Ok(_dbService.GetStudents());
            var list = new List<Student>();

            using (var con = new SqlConnection(ConString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select Student.IndexNumber, Student.FirstName, Student.LastName, Student.BirthDate, Studies.Name as StudiesName, Enrollment.Semester " +
                                  "from Student, Enrollment, Studies " +
                                  "where Student.IdEnrollment = Enrollment.IdEnrollment and Studies.IdStudy = Enrollment.IdStudy";
                con.Open();
                var dr = com.ExecuteReader();
                while(dr.Read())
                {
                    list.Add(new Student
                    {
                        IndexNumber = dr["IndexNumber"].ToString(),
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        BirthDate = dr["BirthDate"].ToString(),
                        StudiesName = dr["StudiesName"].ToString(),
                        Semester = (int)dr["Semester"]
                    });
                }
            }

            return Ok(list);
        }

        [HttpGet("{indexNumber}")]
        public IActionResult GetStudent(string indexNumber)
        {
            using (var con = new SqlConnection(ConString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select Student.IndexNumber, Enrollment.Semester, Studies.Name, Enrollment.StartDate " +
                                  "from Student, Enrollment, Studies " +
                                  "where Student.IndexNumber = @indexNumber and Student.IdEnrollment = Enrollment.IdEnrollment and Studies.IdStudy = Enrollment.IdStudy";
                com.Parameters.AddWithValue("indexNumber", indexNumber);
                con.Open();
                var dr = com.ExecuteReader();
                if (dr.Read())
                {
                    var wpis = new Wpis
                    {
                        IndexNumber = dr["IndexNumber"].ToString(),
                        Semester = (int)dr["Semester"],
                        Name = dr["Name"].ToString(),
                        StartDate = dr["StartDate"].ToString()
                    };
                    return Ok(wpis);
                }
            }
            return NotFound("Nie znaleziono studenta");
        }

        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            //... add to database
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok (student);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateStudent(int id)
        {
            //... update in database
            return Ok("Aktualizacja ukonczona");
        }

        [HttpDelete("{id}")]
        public IActionResult RemoveStudent(int id)
        {
            //... remove from database
            return Ok("Usuwanie ukonczone");
        }
    }
}
