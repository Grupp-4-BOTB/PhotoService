using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhotoService.Domain.Entities;
using PhotoService.Domain.ValueObjects;

namespace PhotoService.Infrastructure.Persistance.Configurations;

public class PhotoConfiguration : IEntityTypeConfiguration<Photo>
{
    public void Configure(EntityTypeBuilder<Photo> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasConversion(id => id.Value, value => new PhotoId(value));

        builder.Property(p => p.OwnerId)
            .HasConversion(id => id.Value, value => new OwnerId(value))
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(p => p.FileName)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(p => p.Url)
            .HasMaxLength(2048)
            .IsRequired();

        builder.Property(p => p.ContentType)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(p => p.SizeInBytes)
            .IsRequired();

        builder.Property(p => p.UploadedAt)
            .IsRequired();
    }
}
