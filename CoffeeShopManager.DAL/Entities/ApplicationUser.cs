using Microsoft.AspNetCore.Identity;

namespace CoffeeShopManager.DAL.Entities;

public class ApplicationUser : IdentityUser
{
    public int? MaNV { get; set; }

    public virtual NhanVien? NhanVien { get; set; }
}
