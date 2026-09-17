using System.Reflection;
using CoffeeShopManager.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Xunit;

namespace CoffeeShopManager.Tests.Security;

public class IdentityPasswordTests
{
    [Fact]
    public void PasswordHasher_CreatesHash_WithoutStoringPlainText()
    {
        var user = new ApplicationUser { UserName = "staff" };
        var password = "CoffeeShop123!";
        var hasher = new PasswordHasher<ApplicationUser>();

        var hash = hasher.HashPassword(user, password);

        Assert.NotEqual(password, hash);
        Assert.True(hash.Length > password.Length);
    }

    [Fact]
    public void PasswordHasher_VerifiesCorrectPassword()
    {
        var user = new ApplicationUser { UserName = "staff" };
        var hasher = new PasswordHasher<ApplicationUser>();
        var hash = hasher.HashPassword(user, "CoffeeShop123!");

        var result = hasher.VerifyHashedPassword(user, hash, "CoffeeShop123!");

        Assert.Equal(PasswordVerificationResult.Success, result);
    }

    [Fact]
    public void ApplicationUser_DoesNotContainPasswordProperty()
    {
        var passwordProperty = typeof(ApplicationUser).GetProperty("Password", BindingFlags.Public | BindingFlags.Instance);

        Assert.Null(passwordProperty);
    }
}
