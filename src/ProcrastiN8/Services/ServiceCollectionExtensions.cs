// Optional DI extensions (only compiled when Microsoft.Extensions.DependencyInjection is referenced by consumer project).
#if PROCRASTIN8_DI
using Microsoft.Extensions.DependencyInjection;
namespace ProcrastiN8.Services;
/// <summary>DI registration helpers (optional).</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Registers procrastination scheduler services with defaults.</summary>
    public static IServiceCollection AddProcrastiN8Scheduler(this IServiceCollection services)
    {
        services.AddSingleton<IProcrastinationStrategyFactory, DefaultProcrastinationStrategyFactory>();
        services.AddSingleton<IExtendedProcrastinationStrategyFactory, CompositeProcrastinationStrategyFactory>();
        services.AddSingleton<IProcrastinationScheduler>(_ => ProcrastinationSchedulerBuilder.Create().Build());
        return services;
    }
}
#endif