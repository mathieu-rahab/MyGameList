using System;
using System.Globalization;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Mygamelist.Entity;

namespace Mygamelist.Utiles;

static class UserSystems
{
    public static bool IsValidPseudo(string pseudo)
    {
        if (string.IsNullOrWhiteSpace(pseudo))
            return false;
        
        try
        {
            return Regex.IsMatch(pseudo,
                @"^[a-zA-Z0-9_\-\.]{3,20}$",
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            // Normalize the domain
            email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                RegexOptions.None, TimeSpan.FromMilliseconds(200));

            // Examines the domain part of the email and normalizes it.
            string DomainMapper(Match match)
            {
                // Use IdnMapping class to convert Unicode domain names.
                var idn = new IdnMapping();

                // Pull out and process domain name (throws ArgumentException on invalid)
                string domainName = idn.GetAscii(match.Groups[2].Value);

                return match.Groups[1].Value + domainName;
            }
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }

        try
        {
            return Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }

    public static bool IsValidPassword(string password)
    {
        return (password.Length > 6);
    }
    
    public static bool IsAdminOrSelf(ClaimsPrincipal claims, int id)
    {
        var role = claims.FindFirstValue("userRole");
        if (role == "admin") return true;

        var userIdClaim = claims.FindFirstValue("userId");
        return int.TryParse(userIdClaim, out int tokenUserId) && tokenUserId == id;
    }


}