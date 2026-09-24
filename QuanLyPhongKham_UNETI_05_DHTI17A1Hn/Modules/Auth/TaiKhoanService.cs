using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Data;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models.Auth;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Auth;

public sealed class TaiKhoanService : ITaiKhoanService
{
    private const string DuplicateUsernameMessage = "Tên đăng nhập đã tồn tại.";

    private readonly AppDbContext _context;

    public TaiKhoanService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<TaiKhoanListViewModel>> GetListAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.TaiKhoans
            .AsNoTracking()
            .OrderBy(account => account.TenDangNhap)
            .Select(account => new TaiKhoanListViewModel
            {
                MaTaiKhoan = account.MaTaiKhoan,
                TenDangNhap = account.TenDangNhap,
                HoTen = account.HoTen,
                Email = account.Email,
                VaiTro = account.VaiTro,
                TrangThai = account.TrangThai
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<TaiKhoanFormViewModel?> GetForEditAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.TaiKhoans
            .AsNoTracking()
            .Where(account => account.MaTaiKhoan == id)
            .Select(account => new TaiKhoanFormViewModel
            {
                MaTaiKhoan = account.MaTaiKhoan,
                TenDangNhap = account.TenDangNhap,
                HoTen = account.HoTen,
                Email = account.Email,
                VaiTro = account.VaiTro,
                TrangThai = account.TrangThai
            })
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<TaiKhoanSaveResult> CreateAsync(
        TaiKhoanFormViewModel model,
        CancellationToken cancellationToken = default)
    {
        var username = model.TenDangNhap?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(username))
        {
            return TaiKhoanSaveResult.Failure(
                nameof(model.TenDangNhap),
                "Tên đăng nhập là bắt buộc.");
        }

        if (string.IsNullOrWhiteSpace(model.MatKhau))
        {
            return TaiKhoanSaveResult.Failure(
                nameof(model.MatKhau),
                "Mật khẩu là bắt buộc.");
        }

        if (!model.VaiTro.HasValue)
        {
            return TaiKhoanSaveResult.Failure(
                nameof(model.VaiTro),
                "Vai trò là bắt buộc.");
        }

        if (await IsDuplicateAsync(username, null, cancellationToken))
        {
            return TaiKhoanSaveResult.Failure(
                nameof(model.TenDangNhap),
                DuplicateUsernameMessage);
        }

        var account = new TaiKhoan
        {
            TenDangNhap = username,
            MatKhau = PasswordHasher.Hash(model.MatKhau),
            HoTen = model.HoTen.Trim(),
            Email = NormalizeEmail(model.Email),
            VaiTro = model.VaiTro.Value,
            TrangThai = model.TrangThai ?? TrangThaiTaiKhoan.HoatDong
        };

        _context.TaiKhoans.Add(account);
        return await SaveWithDuplicateHandlingAsync(account, username, cancellationToken);
    }

    public async Task<TaiKhoanSaveResult> UpdateAsync(
        int id,
        TaiKhoanFormViewModel model,
        CancellationToken cancellationToken = default)
    {
        var account = await _context.TaiKhoans
            .SingleOrDefaultAsync(item => item.MaTaiKhoan == id, cancellationToken);

        if (account is null)
        {
            return TaiKhoanSaveResult.Failure(
                nameof(model.MaTaiKhoan),
                "Không tìm thấy tài khoản.");
        }

        var username = model.TenDangNhap?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(username))
        {
            return TaiKhoanSaveResult.Failure(
                nameof(model.TenDangNhap),
                "Tên đăng nhập là bắt buộc.");
        }

        if (!model.VaiTro.HasValue)
        {
            return TaiKhoanSaveResult.Failure(
                nameof(model.VaiTro),
                "Vai trò là bắt buộc.");
        }

        if (await IsDuplicateAsync(username, id, cancellationToken))
        {
            return TaiKhoanSaveResult.Failure(
                nameof(model.TenDangNhap),
                DuplicateUsernameMessage);
        }

        account.TenDangNhap = username;
        account.HoTen = model.HoTen.Trim();
        account.Email = NormalizeEmail(model.Email);
        account.VaiTro = model.VaiTro.Value;
        account.TrangThai = model.TrangThai ?? TrangThaiTaiKhoan.HoatDong;

        if (!string.IsNullOrWhiteSpace(model.MatKhau))
        {
            account.MatKhau = PasswordHasher.Hash(model.MatKhau);
        }

        return await SaveWithDuplicateHandlingAsync(account, username, cancellationToken);
    }

    public async Task<bool> SetLockAsync(
        int id,
        bool isLocked,
        CancellationToken cancellationToken = default)
    {
        var account = await _context.TaiKhoans
            .SingleOrDefaultAsync(item => item.MaTaiKhoan == id, cancellationToken);

        if (account is null)
        {
            return false;
        }

        account.TrangThai = isLocked
            ? TrangThaiTaiKhoan.Khoa
            : TrangThaiTaiKhoan.HoatDong;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task<bool> IsDuplicateAsync(
        string username,
        int? id,
        CancellationToken cancellationToken)
    {
        return id.HasValue
            ? await _context.TaiKhoans.AnyAsync(
                account => account.TenDangNhap == username
                    && account.MaTaiKhoan != id.Value,
                cancellationToken)
            : await _context.TaiKhoans.AnyAsync(
                account => account.TenDangNhap == username,
                cancellationToken);
    }

    private async Task<TaiKhoanSaveResult> SaveWithDuplicateHandlingAsync(
        TaiKhoan account,
        string username,
        CancellationToken cancellationToken)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return TaiKhoanSaveResult.Success();
        }
        catch (DbUpdateException)
        {
            _context.Entry(account).State = EntityState.Detached;

            if (await _context.TaiKhoans.AnyAsync(
                    item => item.TenDangNhap == username,
                    cancellationToken))
            {
                return TaiKhoanSaveResult.Failure(
                    nameof(TaiKhoanFormViewModel.TenDangNhap),
                    DuplicateUsernameMessage);
            }

            throw;
        }
    }

    private static string? NormalizeEmail(string? email)
    {
        return string.IsNullOrWhiteSpace(email) ? null : email.Trim();
    }
}
