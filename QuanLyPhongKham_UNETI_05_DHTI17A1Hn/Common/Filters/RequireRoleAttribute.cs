using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Data;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Auth.Services;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Common.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RequireRoleAttribute : TypeFilterAttribute
{
    public RequireRoleAttribute()
        : base(typeof(RequireRoleFilter))
    {
    }

    public RequireRoleAttribute(Role requiredRole)
        : base(typeof(RequireRoleFilter))
    {
        RequiredRole = requiredRole;
    }

    public Role? RequiredRole { get; }
}

public sealed class RequireRoleFilter : IAsyncAuthorizationFilter
{
    private readonly AppDbContext _dbContext;

    public RequireRoleFilter(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var metadata = context.ActionDescriptor.EndpointMetadata
            .OfType<RequireRoleAttribute>()
            .FirstOrDefault();

        if (metadata is null)
        {
            return;
        }

        var accountId = context.HttpContext.Session.GetInt32(SessionKeys.AccountId);
        if (!accountId.HasValue)
        {
            if (metadata.RequiredRole is null)
            {
                return;
            }

            RedirectToLogin(context);
            return;
        }

        var account = await _dbContext.Accounts.FindAsync(accountId.Value);
        if (account is null || account.Status == AccountStatus.Locked)
        {
            context.HttpContext.Session.Clear();
            RedirectToLogin(context);
            return;
        }

        if (metadata.RequiredRole.HasValue
            && account.Role != metadata.RequiredRole.Value)
        {
            context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
            return;
        }

        var httpContext = context.HttpContext;
        httpContext.Items[AuthItems.AccountId] = account.Id;
        httpContext.Items[AuthItems.Role] = account.Role;
        httpContext.Items[AuthItems.FullName] = account.FullName;

        if (account.Role == Role.Patient)
        {
            httpContext.Items[AuthItems.PatientId] = await _dbContext.Patients
                .Where(patient => patient.AccountId == account.Id)
                .Select(patient => (int?)patient.Id)
                .SingleOrDefaultAsync();
        }
    }

    private static void RedirectToLogin(AuthorizationFilterContext context)
    {
        context.Result = new RedirectToActionResult("Login", "Auth", null);
    }
}

public static class AuthItems
{
    public const string AccountId = "AccountId";
    public const string Role = "Role";
    public const string FullName = "FullName";
    public const string PatientId = "PatientId";
}
