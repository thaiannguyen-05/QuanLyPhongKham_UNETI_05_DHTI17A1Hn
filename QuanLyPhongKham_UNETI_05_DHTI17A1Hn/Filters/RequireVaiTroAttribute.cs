using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Data;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Auth;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RequireVaiTroAttribute : TypeFilterAttribute
{
    public RequireVaiTroAttribute()
        : base(typeof(RequireVaiTroFilter))
    {
    }

    public RequireVaiTroAttribute(VaiTro requiredRole)
        : base(typeof(RequireVaiTroFilter))
    {
        RequiredRole = requiredRole;
    }

    public VaiTro? RequiredRole { get; }
}

public sealed class RequireVaiTroFilter : IAsyncAuthorizationFilter
{
    private readonly AppDbContext _dbContext;

    public RequireVaiTroFilter(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var metadata = context.ActionDescriptor.EndpointMetadata
            .OfType<RequireVaiTroAttribute>()
            .FirstOrDefault();

        if (metadata is null)
        {
            return;
        }

        var accountId = context.HttpContext.Session.GetInt32(SessionKeys.MaTaiKhoan);
        if (!accountId.HasValue)
        {
            if (metadata.RequiredRole is null)
            {
                return;
            }

            RedirectToLogin(context);
            return;
        }

        var account = await _dbContext.TaiKhoans.FindAsync(accountId.Value);
        if (account is null || AuthHelper.IsLocked(account))
        {
            context.HttpContext.Session.Clear();
            RedirectToLogin(context);
            return;
        }

        if (metadata.RequiredRole.HasValue
            && account.VaiTro != metadata.RequiredRole.Value)
        {
            context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
            return;
        }

        var httpContext = context.HttpContext;
        httpContext.Items[AuthItems.MaTaiKhoan] = account.MaTaiKhoan;
        httpContext.Items[AuthItems.VaiTro] = account.VaiTro;
        httpContext.Items[AuthItems.HoTen] = account.HoTen;

        if (account.VaiTro == VaiTro.BenhNhan)
        {
            httpContext.Items[AuthItems.MaBenhNhan] = await _dbContext.BenhNhans
                .Where(patient => patient.MaTaiKhoan == account.MaTaiKhoan)
                .Select(patient => (int?)patient.MaBenhNhan)
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
    public const string MaTaiKhoan = "MaTaiKhoan";
    public const string VaiTro = "VaiTro";
    public const string HoTen = "HoTen";
    public const string MaBenhNhan = "MaBenhNhan";
}
