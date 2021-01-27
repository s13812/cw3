using System.ComponentModel.DataAnnotations;

namespace cw3.DTOs.Requests
{
    public class EnrollStudentRequest
    {
        [Required]
        public string IndexNumber { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        public string BirthDate { get; set; }

        [Required]
        public string Studies { get; set; }
    }
}
