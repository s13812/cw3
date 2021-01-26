using cw3.DAL;
using cw3.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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

        [HttpGet]
        public IActionResult GetStudents(string orderBy)
        {
            return Ok(_dbService.GetStudents());
        }

        [HttpGet("{id}")]
        public IActionResult GetStudent(int id)
        {
            if (id == 1)
            {
                return Ok("Kowalski");
            } else if (id == 2)
            {
                return Ok("Malewski");
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
