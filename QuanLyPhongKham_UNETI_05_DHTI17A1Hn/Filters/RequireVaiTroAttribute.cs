using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Data;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Auth;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RequireVaiTroAttribute : Attribute, IAsyncActionFilter
{
    public const string CurrentAccountItemKey = "Auth.CurrentAccount";

    private readonly VaiTro _requiredRole;

    public RequireVaiTroAttribute(VaiTro requiredRole)
    {
        _requiredRole = requiredRole;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var account = await AccountSessionValidator.GetValidAccountAsync(
            context.HttpContext);

        if (account is null)
        {
            context.HttpContext.Session.Clear();
            context.Result = AccountSessionValidator.RedirectToLogin();
            return;
        }

        if (account.VaiTro != _requiredRole)
        {
            context.Result = new RedirectToActionResult("Index", "Home", null);
            return;
        }

        await next();
    }
}

public sealed class AccountRevalidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var account = await AccountSessionValidator.GetValidAccountAsync(
            context.HttpContext);

        if (account is null && context.HttpContext.Session.GetInt32(SessionKeys.MaTaiKhoan).HasValue)
        {
            context.HttpContext.Session.Clear();
            context.Result = AccountSessionValidator.RedirectToLogin();
            return;
        }

        await next();
    }
}

internal static class AccountSessionValidator
{
    public static async Task<TaiKhoan?> GetValidAccountAsync(HttpContext httpContext)
    {
        var accountId = httpContext.Session.GetInt32(SessionKeys.MaTaiKhoan);
        if (!accountId.HasValue)
        {
            return null;
        }

        if (httpContext.Items.TryGetValue(
                RequireVaiTroAttribute.CurrentAccountItemKey,
                out var cachedAccount)
            && cachedAccount is TaiKhoan currentAccount)
        {
            return currentAccount;
        }

        var dbContext = httpContext.RequestServices.GetRequiredService<AppDbContext>();
        var account = await dbContext.TaiKhoans
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.MaTaiKhoan == accountId.Value);

        if (account is null || AuthHelper.IsLocked(account))
        {
            return null;
        }

        httpContext.Items[RequireVaiTroAttribute.CurrentAccountItemKey] = account;
        return account;
    }

    public static RedirectToActionResult RedirectToLogin()
    {
        return new RedirectToActionResult("Login", "Auth", null);
    }
}
