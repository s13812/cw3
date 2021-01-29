namespace cw3.Services
{
    public interface IPasswordHashingService
    {
        public string CreateHash(string value, string salt);
        public bool Vadilate(string value, string salt, string hash);
        public string CreateSalt();            
    }
}
