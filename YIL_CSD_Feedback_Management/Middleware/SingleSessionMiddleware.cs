using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using YIL_CSD_Feedback_Management.Data;
using YIL_CSD_Feedback_Management.Helpers;

namespace YIL_CSD_Feedback_Management.Middleware
{
    public class SingleSessionMiddleware
    {
        private readonly RequestDelegate _next;

        private const int SessionTimeoutMinutes = 30;

        public SingleSessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            ApplicationDbContext db)
        {
            // Only check authenticated users
            if (context.User.Identity?.IsAuthenticated == true)
            {
                string? userName =
                    context.User.FindFirstValue(
                        ClaimTypes.Name);

                string? sessionTokenValue =
                    context.User.FindFirstValue(
                        "SessionToken");

                if (!string.IsNullOrWhiteSpace(userName) &&
                    Guid.TryParse(
                        sessionTokenValue,
                        out Guid sessionToken))
                {
                    var session =
                        await db.UserActiveSessions
                            .FirstOrDefaultAsync(x =>
                                x.UserName == userName &&
                                x.SessionToken == sessionToken &&
                                x.IsActive);

                    //==================================================
                    // Session does not exist / another session replaced it
                    //==================================================

                    if (session == null)
                    {
                        await context.SignOutAsync();

                        context.Response.Redirect(
                            "/Account/Login?sessionExpired=true");

                        return;
                    }

                    //==================================================
                    // Check 30 Minutes Inactivity
                    //==================================================

                    DateTime now = DateTimeHelper.Now;

                    if (session.LastActivityDate
                        .AddMinutes(SessionTimeoutMinutes) <= now)
                    {
                        session.IsActive = false;

                        await db.SaveChangesAsync();

                        await context.SignOutAsync();

                        context.Response.Redirect(
                            "/Account/Login?sessionExpired=true");

                        return;
                    }

                    //==================================================
                    // User is active
                    // Update Last Activity
                    //==================================================

                    session.LastActivityDate = now;

                    await db.SaveChangesAsync();
                }
            }

            await _next(context);
        }
    }
}