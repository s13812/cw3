namespace cw3.DTOs
{
    public class LoginDetailsDTO
    {
        public string IndexNumber { get; set; }
        public string HashedPassword { get; set; }
        public string Salt { get; set; }
        public string Role { get; set; }
    }
}
