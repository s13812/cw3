using cw3.Services;
using cw3.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;
using cw3.Exceptions;
using Microsoft.AspNetCore.Authorization;

namespace cw3.Controllers
{
    [Route("api/enrollments")]   
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {

        private IStudentsDbService _dbService;

        public EnrollmentsController(IStudentsDbService service)
        {
            _dbService = service;
        }

        private const string ConString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=s13812;Integrated Security=True";

        [HttpPost]
        [Authorize(Roles = "employee")]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            try
            {
                return Ok(_dbService.EnrollStudent(request));
            }
            catch (RequestException e)
            {
                if (e.Code == 404)
                {
                    return NotFound(e.Message);
                }
                return BadRequest(e.Message);
            }
        }

        [HttpPost("promotions")]
        [Authorize(Roles = "employee")]
        public IActionResult PromoteStudents(PromoteStudentsRequest request)
        {
            try
            {
                return Ok(_dbService.PromoteStudents(request));
            }
            catch (RequestException e)
            {
                if (e.Code == 404)
                {
                    return NotFound(e.Message);
                }
                return BadRequest(e.Message);
            }       
        }
    }
}
