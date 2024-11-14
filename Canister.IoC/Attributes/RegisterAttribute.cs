using Microsoft.Extensions.DependencyInjection;
using System;

namespace Canister.IoC.Attributes
{
    /// <summary>
    /// This attribute is used to register a type with Canister and is used to mark a type for registration.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public class RegisterAttribute : Attribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public RegisterAttribute()
        {
            Lifetime = ServiceLifetime.Transient;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="lifetime">Lifetime of the service</param>
        /// <param name="serviceKey">The service key to register as (if any)</param>
        public RegisterAttribute(ServiceLifetime lifetime, object? serviceKey = null)
        {
            Lifetime = lifetime;
            ServiceKey = serviceKey;
        }

        /// <summary>
        /// The lifetime of the service.
        /// </summary>
        public ServiceLifetime Lifetime { get; set; }

        /// <summary>
        /// The service key to register as (if any).
        /// </summary>
        public object? ServiceKey { get; set; }
    }
}