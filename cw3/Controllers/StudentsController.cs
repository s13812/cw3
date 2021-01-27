using cw3.Services;
using cw3.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace cw3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private IStudentsDbService _dbService;

        public StudentsController(IStudentsDbService service)
        {
            _dbService = service;
        }

        [HttpGet]
        public IActionResult GetStudents()
        {
            return Ok(_dbService.GetStudents());            
        }

        [HttpGet("{indexNumber}")]
        public IActionResult GetStudent(string indexNumber)
        {
            try
            {
                return Ok(_dbService.GetEnrollment(indexNumber));
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
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
