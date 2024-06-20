namespace CoreMvvmLib.Core.IOC
{
    public class ServiceCollection : IServiceCollection
    {
        private readonly Dictionary<Type, ServiceType> _serviceTypes = new Dictionary<Type, ServiceType>();
        private readonly Dictionary<string, Type> _keyTypes = new Dictionary<string, Type>();
        private ServiceContainer _serviceContainer;
        public static IServiceCollection Create()
        {
            return new ServiceCollection();
        }
        public void AddSingleton<TInterface, TImplementation>() where TImplementation : TInterface
        {
            _keyTypes[typeof(TInterface).Name] = typeof(TInterface);
            _serviceTypes[typeof(TInterface)] = new ServiceType()
            {
                Type = typeof(TImplementation),
                IsSingleton = true,
                CalbakcFunc = null,
                Prameter = null
            };
        }

        public void AddSingleton<TImplementation>() where TImplementation : class
        {
            _keyTypes[typeof(TImplementation).Name] = typeof(TImplementation);
            _serviceTypes[typeof(TImplementation)] = new ServiceType()
            {
                Type = typeof(TImplementation),
                IsSingleton = true,
                CalbakcFunc = null,
                Prameter = null
            };
        }

        public void AddSingleton<TImplementation>(TImplementation implementation) where TImplementation : class
        {
            _keyTypes[typeof(TImplementation).Name] = typeof(TImplementation);
            _serviceTypes[typeof(TImplementation)] = new ServiceType()
            {
                Type = typeof(TImplementation),
                IsSingleton = true,
                CalbakcFunc = null,
                Prameter = implementation
            };
        }

        public void AddSingleton<TInterface, TImplementation>(Func<IServiceContainer, TInterface> factory) where TImplementation : TInterface
        {
            _keyTypes[typeof(TInterface).Name] = typeof(TInterface);
            _serviceTypes[typeof(TInterface)] = new ServiceType()
            {
                Type = typeof(TInterface),
                IsSingleton = true,
                CalbakcFunc = factory,
                Prameter = null
            };
        }

        public void AddSingleton<TImplementation>(Func<IServiceContainer, TImplementation> factory) where TImplementation : class
        {
            _keyTypes[typeof(TImplementation).Name] = typeof(TImplementation);
            _serviceTypes[typeof(TImplementation)] = new ServiceType()
            {
                Type = typeof(TImplementation),
                IsSingleton = true,
                CalbakcFunc = factory,
                Prameter = null,
            };
        }

        public void AddTransient<TInterface, TImplementation>() where TImplementation : TInterface
        {
            _keyTypes[typeof(TInterface).Name] = typeof(TInterface);
            _serviceTypes[typeof(TInterface)] = new ServiceType()
            {
                Type = typeof(TImplementation),
                IsSingleton = false,
                CalbakcFunc = null,
                Prameter = null
            };
        }

        public void AddTransient<TImplementation>() where TImplementation : class
        {
            _keyTypes[typeof(TImplementation).Name] = typeof(TImplementation);
            _serviceTypes[typeof(TImplementation)] = new ServiceType()
            {
                Type = typeof(TImplementation),
                IsSingleton = false,
                CalbakcFunc = null,
                Prameter = null
            };
        }

        public void AddTransient<TImplementation>(TImplementation implementation) where TImplementation : class
        {
            _keyTypes[typeof(TImplementation).Name] = typeof(TImplementation);
            _serviceTypes[typeof(TImplementation)] = new ServiceType()
            {
                Type = typeof(TImplementation),
                IsSingleton = false,
                CalbakcFunc = null,
                Prameter = implementation
            };
        }

        public void AddTransient<TInterface, TImplementation>(Func<IServiceContainer, TInterface> factory) where TImplementation : TInterface
        {
            _keyTypes[typeof(TInterface).Name] = typeof(TInterface);
            _serviceTypes[typeof(TInterface)] = new ServiceType()
            {
                Type = typeof(TImplementation),
                IsSingleton = false,
                CalbakcFunc = factory,
                Prameter = null
            };
        }

        public void AddTransient<TImplementation>(Func<IServiceContainer, TImplementation> factory) where TImplementation : class
        {
            _keyTypes[typeof(TImplementation).Name] = typeof(TImplementation);
            _serviceTypes[typeof(TImplementation)] = new ServiceType()
            {
                Type = typeof(TImplementation),
                IsSingleton = false,
                CalbakcFunc = factory,
                Prameter = null
            };
        }

        public bool CheckType(Type type)
        {
            if(_serviceTypes.ContainsKey(type))
                return true;
            return false;
        }

        public IServiceContainer CreateContainer()
        {
            if (_serviceContainer == null)
            {
                _serviceContainer = ServiceContainer.Instance(this);
            }
            return _serviceContainer;
        }

        public ServiceType GetType(Type type)
        {
            if(_serviceTypes.ContainsKey(type))
                return _serviceTypes[type];
            return null;
        }

        public Type KeyType(string name)
        {
            if (_keyTypes.ContainsKey(name))
                return _keyTypes[name];
            else
                return null;
        }
    }
}
