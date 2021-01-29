using cw3.DTOs;

namespace cw3.Services
{
    public interface ITokenGeneratorService
    {
        public string GetToken(LoginDetailsDTO loginDetails);
    }
}
