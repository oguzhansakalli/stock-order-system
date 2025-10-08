using Microsoft.AspNetCore.Http;
using SharedKernel.Application.Abstractions;

namespace SharedKernel.Infrastructure.Services
{
    public class TenantProvider : ITenantProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TenantProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public Guid GetCurrentTenantId()
        {
            var tenantIdClaim = _httpContextAccessor.HttpContext?.User
            .FindFirst("TenantId")?.Value;

            if (Guid.TryParse(tenantIdClaim, out var tenantId))
            {
                return tenantId;
            }

            // For development/testing - return a default tenant
            return Guid.Parse("00000000-0000-0000-0000-000000000001");
        }
        public string GetCurrentTenantName()
        {
            return _httpContextAccessor.HttpContext?.User
                .FindFirst("TenantName")?.Value ?? "Default Tenant";
        }
    }
}
