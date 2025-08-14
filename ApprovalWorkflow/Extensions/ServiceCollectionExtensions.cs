using ApprovalSystem.Data;
using ApprovalSystem.Services;
using System.Reflection;

namespace ApprovalSystem.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddTransient(typeof(IRepository<,>), typeof(Repository<,>));
            services.AddTransient(typeof(IApprovableRepository<,>), typeof(ApprovableRepository<,>));

            var definedTypes = Assembly.GetExecutingAssembly().DefinedTypes;
            var scopedInterfaces = definedTypes.Where(t => t.IsAssignableTo(typeof(IApplicationScopedService)) && 
            t != typeof(IApplicationScopedService) && t.IsInterface);

            foreach (var interfac in scopedInterfaces)
            {
                var implement = definedTypes.Where(t => t.IsAssignableTo(interfac) && t.IsClass && !t.IsAbstract).FirstOrDefault();
                if (implement == null)
                {
                    throw new InvalidOperationException("Cannot register an interface without a type implementation. " +
                        "Either remove it or provide a class implementing the interface appropriately");
                }

                services.AddScoped(interfac, implement);
            }

            return services;
        }
    }
}
