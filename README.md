# OTP Management API

The OTP Management API provides endpoints to generate and verify OTPs (One-Time Passwords) via email. This API allows you to securely generate OTPs and validate them against user-provided email and OTP codes.

## Endpoints

### Generate OTP via Email

**Endpoint:** `POST /generate_otp_email`

Generates a 6-digit OTP and sends it to the user's email address.

#### Request Parameters

| Parameter | Type   | Description                  |
| --------- | ------ | ---------------------------- |
| email     | string | The email address of the user |

#### Response

If successful, a 200 OK response is returned with STATUS_EMAIL_OK, indicating that the OTP has been generated and sent to the user's email address.

### Verify OTP

**Endpoint:** `POST /check_otp`

Verifies whether the provided email and OTP code match.

#### Request Parameters

| Parameter | Type   | Description                  |
| --------- | ------ | ---------------------------- |
| email     | string | The email address of the user |
| otp       | string | The OTP code to be verified   |

#### Response

If the email and OTP code match, a 200 OK response is returned with STATUS_OTP_OK, indicating successful verification. Otherwise, STATUS_OTP_FAIL or STATUS_OTP_TIMEOUT response is returned.

## Getting Started

To use the OTP Management API, follow these steps:

1. Clone the repository: `git clone https://github.com/SaiAungMein/GetronicsOTPServices.git`
2. Install .NET 6 runtime on local machine
3. Changed connection string in appSetting.json
4. Run Update-Database in Visual Stuido or restore database up to MSSQL (find bak file in AppData folder)
5. Access the API endpoints as described above.

#Noted
1. To fail sending email, you can change email password in appSetting.json
2. You can change valid email domain or add multiple accept email domain in appSetting.json
3. To see the logs, please find in GetronicsOTPServices\logs or GetronicsOTPServices\bin\Debug\net6.0\logs 
