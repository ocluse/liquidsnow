using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Ocluse.LiquidSnow.DependencyInjection;

/// <summary>
/// Allows a class to be excluded from auto registration in the DI Container when using methods such as 
/// <see cref="BuilderExtensions.TryAddImplementersOfGenericAsImplemented(IServiceCollection, Type, Assembly, ServiceLifetime)"/>
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple =true, Inherited = false)]
public class NotRegisteredAttribute : Attribute
{
    /// <summary>
    /// If null, exclude this class from all registrations.
    /// Otherwise, only exclude when registering as exactly this service type.
    /// </summary>
    public Type? ServiceType { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="NotRegisteredAttribute"/> class that excludes the class from all registrations.
    /// </summary>
    public NotRegisteredAttribute() { }

    /// <summary>
    /// Creates a new instance of the <see cref="NotRegisteredAttribute"/> class that excludes the class from registration as the provided service type.
    /// </summary>
    /// <param name="serviceType">The type to exclude when registering into the DI Container.</param>
    public NotRegisteredAttribute(Type? serviceType)
    {
        ServiceType = serviceType;
    }
}
