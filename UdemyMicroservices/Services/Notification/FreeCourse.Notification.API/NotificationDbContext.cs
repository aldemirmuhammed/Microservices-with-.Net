using Microsoft.EntityFrameworkCore;
using FreeCourse.Notification.API.Entities.Notification;

namespace FreeCourse.Notification.API
{
    public class NotificationDbContext : DbContext
    {
        public const string DEFAULT_SCHEMA = "sch_notifications";

        public DbSet<NotificationDto> Notifications { get; set; }
        public DbSet<NotificationSettings> NotificationSettings { get; set; }
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NotificationDto>().ToTable("Notifications", DEFAULT_SCHEMA);
            modelBuilder.Entity<NotificationSettings>().ToTable("NotificationSetting", DEFAULT_SCHEMA);
           
            base.OnModelCreating(modelBuilder);
        }
    }
}
