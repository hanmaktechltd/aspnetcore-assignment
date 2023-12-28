using Microsoft.AspNetCore.Http;
using Queue_Management_System.Models;

public static class UserUtility
{
    private static string loggedInUserName;

    public static string GetLoggedInUser(HttpContext httpContext)
    {
        loggedInUserName = httpContext.User.Identity.Name;
        return string.IsNullOrEmpty(loggedInUserName) ? "No user logged in" : loggedInUserName;
    }

    public static void SetLoggedInUser(string userName)
    {
        loggedInUserName = userName;
    }

    // Method to get the currently set logged-in user's name (example)
    public static string GetCurrentLoggedInUser()
    {
        return string.IsNullOrEmpty(loggedInUserName) ? "No user logged in" : loggedInUserName;
    }
}
