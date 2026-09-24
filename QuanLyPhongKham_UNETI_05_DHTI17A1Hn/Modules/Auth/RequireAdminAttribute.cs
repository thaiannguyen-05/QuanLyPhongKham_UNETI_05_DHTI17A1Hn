using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Auth;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RequireAdminAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var role = context.HttpContext.Session.GetString(SessionKeys.Role);
        if (!string.Equals(role, nameof(VaiTro.Admin), StringComparison.Ordinal))
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
            return;
        }

        await next();
    }
}
