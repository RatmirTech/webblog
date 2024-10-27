using Microsoft.EntityFrameworkCore;
using webblog.backend.BlogApi.Models;

namespace webblog.backend.BlogApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<PostContent> PostsContent { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Post>(entity =>
            {
                entity.ToTable("posts");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Alias).HasColumnName("alias").IsRequired();
                entity.Property(e => e.AuthorId).HasColumnName("author_id").IsRequired();
                entity.Property(e => e.DatePosted).HasColumnName("date_posted").IsRequired();
            });

            modelBuilder.Entity<PostContent>(entity =>
            {
                entity.ToTable("posts_content");
                entity.HasKey(e => e.PostId);
                entity.Property(e => e.PostId).HasColumnName("post_id");
                entity.Property(e => e.Title).HasColumnName("title").IsRequired();
                entity.Property(e => e.PostBody).HasColumnName("post_body").IsRequired();
                entity.HasOne(e => e.Post)
                .WithOne(p => p.Content)
                    .HasForeignKey<PostContent>(pc => pc.PostId);
            });
        }
    }
}
