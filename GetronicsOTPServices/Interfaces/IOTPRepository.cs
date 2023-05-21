
namespace GetronicsOTPServices.Interfaces
{
    public interface IOTPRepository
    {
        Task<string> SendOTP(string email);
        Task<string> VerifyOTP(string email, string otp);
    }
}
