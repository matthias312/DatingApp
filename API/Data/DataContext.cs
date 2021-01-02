using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

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