using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP.Infra.Base.BaseHandler
{
    public class ServiceLocator
    {
        private static readonly object SingletonServiceProviderLock = new object();

        private static ServiceProvider _singletonServiceProvider;

        private readonly ServiceProvider _currentServiceProvider;

        public ServiceLocator(ServiceProvider currentServiceProvider)
        {
            _currentServiceProvider = currentServiceProvider;
        }

        public static ServiceLocator Current => _singletonServiceProvider != null ? new ServiceLocator(_singletonServiceProvider) : null;

        public static void SetLocatorProvider(ServiceProvider serviceProvider)
        {
            lock (SingletonServiceProviderLock)
            {
                _singletonServiceProvider = serviceProvider;
            }
        }

        public object GetInstance(Type serviceType)
        {
            return _currentServiceProvider?.GetService(serviceType);
        }

        public TService GetInstance<TService>()
        {
            return _currentServiceProvider.GetService<TService>();
        }
    }
}
