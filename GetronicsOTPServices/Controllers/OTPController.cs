using GetronicsOTPServices.Configs;
using GetronicsOTPServices.Constants;
using GetronicsOTPServices.Interfaces;
using GetronicsOTPServices.Repositories;
using log4net;
using Microsoft.AspNetCore.Mvc;

namespace GetronicsOTPServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OTPController : ControllerBase
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(OTPController));
        private readonly IOTPRepository _otpRepository;
        private readonly IEmailValidation _emailValidation;
        public OTPController(IOTPRepository otpRepository,
            IEmailValidation emailValidation)
        {
            _otpRepository = otpRepository;
            _emailValidation = emailValidation;
        }

        [HttpPost("generate_OTP_email")]
        public async Task<IActionResult> SendOTP(string email)
        {
            // email is not valid
            if(!_emailValidation.VerifyEmail(email))
            {
                return Ok(EmailStatusCode.STATUS_EMAIL_FAIL); 
            }

            _log.Info($"Generate OTP and sending Email to {email}");

            var response = await _otpRepository.SendOTP(email);

            // I am not clear about the retrun code here
            // do we need to return string only or we need to status code with reponse body eg. status code 500 with STATUS_EMAIL_FAIL?
            // according to the requirement in docs, currently I have retrun string only with status Ok(200).
            // We can discuss more, if I have a chance to be part of the team.
            return Ok(response);
        }

        [HttpPost("check_OTP")]
        public async Task<IActionResult> VerifyOTP(string email, string otp)
        {
            // understand in requirement need to accept IOSteam as an input
            // but I don't get what's mean, if I have a chance to be part of the team, we can discuss more about on this.
            // currently I have accepted email and otp from user input.

            // email is not valid
            if (!_emailValidation.VerifyEmail(email))
            {
                return Ok(EmailStatusCode.STATUS_EMAIL_FAIL);
            }

            _log.Info($"Start verify OTP for email {email}");

            var response = await _otpRepository.VerifyOTP(email, otp);
            return Ok(response);
        }
    }
}
