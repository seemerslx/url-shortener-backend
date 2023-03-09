using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace UrlShortenerApi.Core
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opts) : base(opts) { }

        public DbSet<UrlManegment> Urls { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UrlManegment>()
                .HasOne(x => x.User)
                .WithMany(u => u.Urls)
            .HasForeignKey(x => x.UserId);

            builder.Entity<UrlManegment>()
                .HasIndex(b => b.Url)
                .IsUnique();

            builder.Entity<UrlManegment>()
                .HasIndex(b => b.ShortUrl)
                .IsUnique();


            base.OnModelCreating(builder);
        }
    }
}
