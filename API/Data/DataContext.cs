using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : IdentityDbContext<AppUser, AppRole, int, IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int> >
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>()
                .HasMany(appUser => appUser.UserRoles)
                .WithOne(appUserRole => appUserRole.User)
                .HasForeignKey(appUserRole => appUserRole.UserId)
                .IsRequired();

            builder.Entity<AppRole>()
                .HasMany(appRole => appRole.UserRoles)
                .WithOne(appUserRole => appUserRole.Role)
                .HasForeignKey(appUserRole => appUserRole.RoleId)
                .IsRequired();

            // Likes many-to-many

            builder.Entity<UserLike>()
                .HasKey(k => new { k.SourceUserId, k.LikedUserId });

            builder.Entity<UserLike>()
                .HasOne(like => like.SourceUser)
                .WithMany(user => user.LikedUsers)
                .HasForeignKey(like => like.SourceUserId)
                .OnDelete(DeleteBehavior.Cascade);
            // .OnDelete(DeleteBehavior.NoAction); für SQL-Server sonst Migration-Error

            builder.Entity<UserLike>()
                .HasOne(like => like.LikedUser)
                .WithMany(user => user.LikedByUsers)
                .HasForeignKey(like => like.LikedUserId)
                .OnDelete(DeleteBehavior.Cascade);
            // .OnDelete(DeleteBehavior.NoAction); für SQL-Server sonst Migration-Error

            builder.Entity<Message>()
                .HasOne(message => message.Recipient)
                .WithMany(recipient => recipient.MessagesReceived)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(message => message.Sender)
                .WithMany(sender => sender.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}