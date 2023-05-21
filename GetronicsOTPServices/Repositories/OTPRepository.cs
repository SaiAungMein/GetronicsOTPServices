using Azure.Core;
using Azure;
using GetronicsOTPServices.CommonServices;
using GetronicsOTPServices.Constants;
using GetronicsOTPServices.Contexts;
using GetronicsOTPServices.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.Serialization;
using log4net;

namespace GetronicsOTPServices.Repositories
{
    public class OTPRepository : IOTPRepository
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(OTPRepository));
        private readonly DBContext _context;
        private readonly IEmailService _emailService;

        public OTPRepository(DBContext context,
            IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<string> SendOTP(string email)
        {
            // try catch shouldn't have to write in every method, we should implement in global exception
            // but because of the tight deadline, currently I have hanlded exception in every method using try catch.
            try
            {
                // 1. inactive the existing code
                var existingCode = await _context
                                   .OTPCode
                                   .FirstOrDefaultAsync(x => x.Email.Equals(email) && x.IsActive);

                if (existingCode != null)
                {
                    // inactive the existing code
                    existingCode.IsActive = false;
                    existingCode.UpdatedDate = DateTime.Now;
                }

                // 2. generate new code
                string code = CommonUtilty.GenerateRandomNumber();

                // 3. insert a new code
                var otpCode = new OTPCode()
                {
                    Email = email,
                    Code = code,
                    IsActive = true,
                    AttemptFailCount = 0,
                    CreatedDate = DateTime.Now,
                };
                _context.OTPCode.Add(otpCode);
                await _context.SaveChangesAsync();

                // 4. Sending email
                // understood in requirement that we can assume sending email will have in another module
                // for email, we should have email template, shouldn't have a hardcode here for subject and body, should improve in the next step
                var emailStatus = _emailService.Send(email, "OTP Code", $"You OTP Code is {code}. The code is valid for 1 minute.");

                // 5. failed to send email, return STATUS_EMAIL_FAIL
                if (!emailStatus)
                {
                    return EmailStatusCode.STATUS_EMAIL_FAIL;
                }

                // 6. successfull sent OTP code
                return EmailStatusCode.STATUS_EMAIL_OK;
            }
            catch (Exception e)
            {
                _log.Error(e);

                return EmailStatusCode.STATUS_EMAIL_FAIL; // return fail if internal server occured
            }            
        }

        public async Task<string> VerifyOTP(string email, string otp)
        {
            try
            {
                // 1. get active code by email
                var existingCode = await _context.OTPCode
                .FirstOrDefaultAsync(x => x.Email.Equals(email) && x.IsActive);

                // 2. email not found, return STATUS_OTP_FAIL
                if (existingCode == null ||
                    existingCode.AttemptFailCount >= Constant.OTP_ATTEMPT_FAIL_COUNT) // attempt fail more than 10 time, return STATUS_OTP_FAIL
                {
                    return OTPStatusCode.STATUS_OTP_FAIL;
                }

                var totalMins = (int)DateTime.Now.Subtract(existingCode.CreatedDate).TotalMinutes;

                // 3. expired code in 1 minute
                if (totalMins > Constant.OTP_EXPIRED_MIN)
                {
                    return OTPStatusCode.STATUS_OTP_TIMEOUT;
                }

                // 4. email already have otp code generated, but user input is not match
                else if (existingCode.Code != otp)
                {
                    // increased to fail count
                    existingCode.AttemptFailCount++;
                    await _context.SaveChangesAsync();
                    return OTPStatusCode.STATUS_OTP_FAIL;
                }

                _log.Info($"Successfully verify OTP code for email {email}");
                // 5. successfully verify OTP code
                return OTPStatusCode.STATUS_OTP_OK;
            }
            catch (Exception e)
            {
                _log.Error(e);

                return EmailStatusCode.STATUS_EMAIL_FAIL; // return fail if internal server occured
            }    
        }
    }
}
