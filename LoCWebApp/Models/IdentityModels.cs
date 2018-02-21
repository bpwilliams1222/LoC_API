using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System;

namespace LoCWebApp.Models
{
    public class ApplicationUserRole : IdentityUserRole
    {

    }
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole()
        {
            Id = Guid.NewGuid().ToString();
        }
    }

    public enum UserStatus
    {
        Active,
        Inactive,
        Applied,
        Deleted
    }
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public UserStatus Status { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TimeZoneOffset { get; set; }
        public DateTime Created { get; set; }
        public string LoCApikey { get; set; }
        public string EEApikey { get; set; }

        public ApplicationUser()
        {
            Created = DateTime.UtcNow;
            TimeZoneOffset = TimeZoneInfo.Local.BaseUtcOffset.ToString();
            Status = UserStatus.Applied;
            LoCApikey = null;
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
    
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public new DbSet<ApplicationRole> Roles { get; set; }
        public DbSet<ApplicationUserRole> UserRoles { get; set; }
    }
}