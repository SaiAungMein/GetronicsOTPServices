namespace GetronicsOTPServices.CommonServices
{
    public static class CommonUtilty
    {
        public static string GenerateRandomNumber()
        {
            Random generator = new Random();
            String number = generator.Next(0, 999999).ToString("D6");
            return number;
        }
    }
}
