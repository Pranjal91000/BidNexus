using Core.Abstraction.Services;
using Core.Entities.Master;
using Infrastructure.EntityConfigurations.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations.Master
{
    public class UnitEntityConfiguration(IJwtHelperService jwtHelperService) : IEntityTypeConfiguration<Unit>
    {
        private readonly IJwtHelperService _jwtHelper = jwtHelperService;

        public void Configure(EntityTypeBuilder<Unit> builder)
        {
            builder.ToTable("Unit", "Master");
            MasterBaseEntityConfiguration.Configure(builder);
            builder.HasQueryFilter(x => x.TenantId == _jwtHelper.GetTenantId());
        }
    }
}
