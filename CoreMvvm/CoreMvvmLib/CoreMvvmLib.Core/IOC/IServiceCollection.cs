using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreMvvmLib.Core.IOC
{
    public interface IServiceCollection
    {
        public void AddSingleton<TInterface, TImplementation>() where TImplementation : TInterface;
        public void AddSingleton<TImplementation>() where TImplementation : class;
        public void AddSingleton<TImplementation>(TImplementation implementation) where TImplementation : class;
        public void AddSingleton<TInterface, TImplementation>(Func<IServiceContainer, TInterface> factory) where TImplementation : TInterface;
        public void AddSingleton<TImplementation>(Func<IServiceContainer, TImplementation> factory) where TImplementation : class;

        public void AddTransient<TInterface, TImplementation>() where TImplementation : TInterface;
        public void AddTransient<TImplementation>() where TImplementation : class;
        public void AddTransient<TImplementation>(TImplementation implementation) where TImplementation : class;
        public void AddTransient<TInterface, TImplementation>(Func<IServiceContainer, TInterface> factory) where TImplementation : TInterface;
        public void AddTransient<TImplementation>(Func<IServiceContainer, TImplementation> factory) where TImplementation : class;

        public bool CheckType(Type type);
        public Type KeyType(string name);
        public ServiceType GetType(Type type);
        public IServiceContainer CreateContainer();
    }
    public class ServiceType
    {
        public Type? Type { get; set; }
        public bool IsSingleton { get; set; }
        public object? Prameter { get; set; }
        public object? CalbakcFunc { get; set; }
    }
}
