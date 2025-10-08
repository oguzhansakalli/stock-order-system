namespace SharedKernel.Application.Abstractions
{
    public interface ITenantProvider
    {
        Guid GetCurrentTenantId();
        string GetCurrentTenantName();
    }
}
