using System;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Reflection;

namespace AppFramework.Auth.Data
{
    public static class ConfigurationsExtensions
    {
        [Obsolete("Use EF6 method after upgrade")]
        public static void AddFromAssembly(this ConfigurationRegistrar configurations, Assembly assembly)
        {
            var typesToRegister = assembly
                    .GetTypes()
                    .Where(type =>
                            !string.IsNullOrEmpty(type.Namespace)
                            && !type.IsAbstract
                            && type.BaseType != null && type.BaseType.IsGenericType
                            && type.BaseType.GetGenericTypeDefinition() == typeof (EntityTypeConfiguration<>));

            foreach (var type in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                configurations.Add(configurationInstance);
            }
        }
    }
}