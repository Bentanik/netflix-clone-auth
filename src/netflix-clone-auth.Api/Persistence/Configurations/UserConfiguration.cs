namespace netflix_clone_auth.Api.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(nameof(User));

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(u => u.IsEmailConfirmed)
            .HasDefaultValue(false);

        builder.Property(u => u.AvatarId)
            .HasDefaultValue(false);

        builder.Property(u => u.AvatarUrl)
            .HasDefaultValue(false);

        // Unique index
        builder.HasIndex(u => u.Email)
            .IsUnique();
        
        builder.HasIndex(u => u.DisplayName)
            .IsUnique();
    }
}