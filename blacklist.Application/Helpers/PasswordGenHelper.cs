using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Application.Helpers
{
    public static class PasswordGenHelper
    {
        public static string GenerateRandomPassword(int length)
        {
            const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
            const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digitChars = "1234567890";

            if (length < 3)
            {
                throw new ArgumentException("Password length must be at least 3 characters.");
            }

            Random random = new Random();

            // Create placeholders for each character type
            char[] password = new char[length];
            password[0] = lowerChars[random.Next(lowerChars.Length)];
            password[1] = upperChars[random.Next(upperChars.Length)];
            password[2] = digitChars[random.Next(digitChars.Length)];

            const string allChars = lowerChars + upperChars + digitChars;

            for (int i = 3; i < length; i++)
            {
                password[i] = allChars[random.Next(allChars.Length)];
            }

            // Shuffle the characters in the password
            for (int i = 0; i < length; i++)
            {
                int r = i + random.Next(length - i);
                char temp = password[i];
                password[i] = password[r];
                password[r] = temp;
            }

            return new string(password);
        }

    }
}
