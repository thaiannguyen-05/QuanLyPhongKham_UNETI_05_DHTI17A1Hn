using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Data;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Auth;

public sealed class AccountStatusFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var accountId = context.HttpContext.Session.GetInt32(SessionKeys.AccountId);
        if (!accountId.HasValue)
        {
            await next();
            return;
        }

        var dbContext = context.HttpContext.RequestServices
            .GetRequiredService<AppDbContext>();
        var account = await dbContext.TaiKhoans
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.MaTaiKhoan == accountId.Value);

        if (account is null || AuthHelper.IsLocked(account))
        {
            context.HttpContext.Session.Clear();
            context.Result = new RedirectToActionResult("Login", "Auth", null);
            return;
        }

        await next();
    }
}
