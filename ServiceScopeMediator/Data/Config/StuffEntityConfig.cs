using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServiceScopeMediator.Model;

namespace ServiceScopeMediator.Data.Config;

public class StuffEntityConfig : IEntityTypeConfiguration<Stuff>
{
    public void Configure(EntityTypeBuilder<Stuff> builder)
    {
        builder.ToTable("Stuffs");
    }
}