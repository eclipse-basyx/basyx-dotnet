using Microsoft.Extensions.DependencyInjection;
using System;

namespace BaSyx.Utils.DependencyInjection
{
    public static class DependencyInjectionActivator
    {
        public static object CreateInstance(IServiceProvider serviceProvider, Type interfaceType)
        {
            if (serviceProvider == null || interfaceType == null || !interfaceType.IsInterface)
                return null;

            object instance = ActivatorUtilities.CreateInstance(serviceProvider, interfaceType);
            return instance;
        }

        public static object CreateStandardInstance(Type interfaceType)
        {
            IServiceProvider serviceProvider = DefaultImplementation.GetStandardServiceProvider();
            object instance = CreateInstance(serviceProvider, interfaceType);
            return instance;
        }
        
        public static T CreateStandardInstance<T>()
        {
            object instance = CreateStandardInstance(typeof(T));
            if (instance is T castedInstance)
                return castedInstance;
            else
                return default;
        }

        public static T CreateInstance<T>(IServiceProvider serviceProvider)
        {
            object instance = CreateInstance(serviceProvider, typeof(T));
            if (instance is T castedInstance)
                return castedInstance;
            else
                return default;            
        }
    }
}
