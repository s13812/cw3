using cw3.DTOs.Requests;
using cw3.DTOs.Responses;
using cw3.Exceptions;
using cw3.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace cw3.Services
{
    public class SqlServerDbService : IStudentsDbService
    {

        private const string ConString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=s13812;Integrated Security=True";

        public EnrollStudentResponse EnrollStudent(EnrollStudentRequest request)
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
                    throw new RequestException("Studia nie istnieja", 404);
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
                    throw new RequestException("Student z takim ID juz istnieje");
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

                return response;
            }
        }

        public Enrollment GetEnrollment(string indexNumber)
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
                    var wpis = new Enrollment
                    {
                        IndexNumber = dr["IndexNumber"].ToString(),
                        Semester = (int)dr["Semester"],
                        Name = dr["Name"].ToString(),
                        StartDate = dr["StartDate"].ToString()
                    };
                    return wpis;
                }
            }
            throw new RequestException("Nie znaleziono studenta", 404);
        }

        public IEnumerable<Student> GetStudents()
        {
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
                while (dr.Read())
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

            return list;
        }

        public bool IsStudentInDb(string indexNumber)
        {
            using (var con = new SqlConnection(ConString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select IndexNumber " +
                                  "from Student " +
                                  "where IndexNumber = @indexNumber";
                com.Parameters.AddWithValue("indexNumber", indexNumber);
                con.Open();
                var dr = com.ExecuteReader();

                return dr.Read();
            }            
        }

        public PromoteStudentsResponse PromoteStudents(PromoteStudentsRequest request)
        {
            using (var con = new SqlConnection(ConString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();

                com.CommandText = "select * from Enrollment, Studies where Enrollment.IdStudy = Studies.IdStudy and Enrollment.Semester = @semester and Studies.Name = @studies";
                com.Parameters.AddWithValue("semester", request.Semester);
                com.Parameters.AddWithValue("studies", request.Studies);

                var dr = com.ExecuteReader();

                if (!dr.Read())
                {
                    throw new RequestException("Brak wpisu w bazie :(", 404);
                }

                dr.Close();

                com.CommandText = "exec PromoteStudents @Studies = @studies, @Semester = @semester";

                dr = com.ExecuteReader();

                if (!dr.Read())
                {
                    throw new RequestException("Something went terribly wrong T.T");                    
                }

                var response = new PromoteStudentsResponse
                {
                    IdEnrollment = (int)dr["IdEnrollment"],
                    Semester = (int)dr["Semester"],
                    IdStudy = (int)dr["IdStudy"],
                    StartDate = dr["StartDate"].ToString()
                };

                return response;
            }
        }
    }
}
