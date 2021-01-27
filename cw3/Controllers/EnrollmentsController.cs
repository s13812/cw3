using cw3.DTOs.Requests;
using cw3.DTOs.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace cw3.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private const string ConString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=s13812;Integrated Security=True";

        [HttpPost]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            using (var con = new SqlConnection(ConString))
            using (var com = new SqlCommand())
            {
                con.Open();
                var tran = con.BeginTransaction();
                
                com.Connection = con;
                com.Transaction = tran;


                var response = new EnrollStudentResponse();
                response.Semester = 1;                
                
                //sprawdzanie czy studia istnieja
                
                com.CommandText = "select IdStudy from studies where Name = @name";
                com.Parameters.AddWithValue("name", request.Studies);

                var dr = com.ExecuteReader();
                if (!dr.Read())
                {
                    dr.Close();
                    tran.Rollback();
                    return BadRequest("Studia nie istnieja");
                }
                response.IdStudy = (int)dr["IdStudy"];

                dr.Close();

                //sprawdzanie unikalnosci id studenta

                com.CommandText = "select IndexNumber from Student where IndexNumber = @indexnumber";
                com.Parameters.AddWithValue("indexnumber", request.IndexNumber);

                dr = com.ExecuteReader();
                if (dr.Read())
                {
                    dr.Close();
                    tran.Rollback();
                    return BadRequest("Student z takim ID juz istnieje");
                }

                dr.Close();

                //sprawdzanie istnienia / dodawnanie wpisu 

                com.CommandText = "select * from Enrollment where IdStudy = @idstudy and Semester = 1 order by StartDate desc";
                com.Parameters.AddWithValue("idstudy", response.IdStudy);
                
                dr = com.ExecuteReader();
                if (dr.Read())
                {
                    response.IdEnrollment = (int)dr["IdEnrollment"];
                    response.StartDate = (DateTime)dr["StartDate"];
                }
                else
                {
                    dr.Close();

                    response.IdEnrollment = 1;
                    com.CommandText = "select top (1) IdEnrollment + 1 as nextId from Enrollment order by IdEnrollment desc";
                    dr = com.ExecuteReader();
                    if (dr.Read())
                    {
                        response.IdEnrollment = (int)dr["nextId"];
                    }
                    dr.Close(); 

                    response.StartDate = DateTime.Now;
                    com.CommandText = "insert into Enrollment values(@idenrollment, 1, @idstudy, @startdate)";
                    com.Parameters.AddWithValue("idenrollment", response.IdEnrollment);
                    com.Parameters.AddWithValue("startdate", response.StartDate);

                    com.ExecuteNonQuery();
                }

                dr.Close();

                //dodawanie studenta

                com.CommandText = "insert into Student values (@indexnumber, @firstname, @lastname, @birthdate, @idenrollmentfinal)";
                com.Parameters.AddWithValue("firstname", request.FirstName);
                com.Parameters.AddWithValue("lastname", request.LastName);
                com.Parameters.AddWithValue("birthdate", request.BirthDate);
                com.Parameters.AddWithValue("idenrollmentfinal", response.IdEnrollment);

                com.ExecuteNonQuery();

                tran.Commit();
                return Ok(response);
            }            
        }
    }
}
