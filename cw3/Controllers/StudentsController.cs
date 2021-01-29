using cw3.Services;
using cw3.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Authorization;
using cw3.DTOs.Requests;
using Microsoft.Extensions.Configuration;

namespace cw3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private IStudentsDbService _dbService;
        private IPasswordHashingService _passwordService;
        private ITokenGeneratorService _tokenService;
        public IConfiguration Configuration { get; set; }

        public StudentsController(IStudentsDbService dbService, IPasswordHashingService passwordService, ITokenGeneratorService tokenService, IConfiguration configuration)
        {
            _dbService = dbService;
            _passwordService = passwordService;
            _tokenService = tokenService;
            Configuration = configuration;
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
            return Ok(student);
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

        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            var loginDetails = _dbService.GetLoginDetails(request.Login);

            if (loginDetails == null)
            {
                return Unauthorized("Bledny login");
            }


            if (!_passwordService.Vadilate(request.Password, loginDetails.Salt, loginDetails.HashedPassword))
            {
                return Unauthorized("Bledne haslo");
            }

            var refreshToken = Guid.NewGuid().ToString();

            _dbService.SetRefreshToken(loginDetails.IndexNumber, refreshToken);

            return Ok(new
            {
                accessToken = _tokenService.GetToken(loginDetails),
                refreshToken
            });
        }

        [HttpPost("refresh-token/{refToken}")]
        [AllowAnonymous]
        public IActionResult RefreshToken(string refToken)
        {
            var loginDetails = _dbService.GetLoginDetailsFromRefreshToken(refToken);
            if (loginDetails == null)
            {
                return Unauthorized("Token niepoprawny");
            }

            var refreshToken = Guid.NewGuid().ToString();

            _dbService.SetRefreshToken(loginDetails.IndexNumber, refreshToken);

            return Ok(new
            {
                accessToken = _tokenService.GetToken(loginDetails),
                refreshToken
            });
        }

        [HttpPost("test/{password}")]
        [AllowAnonymous]
        public IActionResult TestPasswords(string password)
        {
            var salt = _passwordService.CreateSalt();

            return Ok(new
            {
                salt,
                hashedPassword = _passwordService.CreateHash(password, salt)
            });
        }
    }
}
