using Microsoft.Extensions.DependencyInjection;
using System;

namespace Canister.IoC.Attributes
{
    /// <summary>
    /// This attribute is used to register all items of a specific type with Canister and is used to
    /// mark a type for registration.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
    public class RegisterAllAttribute : Attribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public RegisterAllAttribute()
        {
            Lifetime = ServiceLifetime.Transient;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="lifetime">Lifetime of the service</param>
        public RegisterAllAttribute(ServiceLifetime lifetime)
        {
            Lifetime = lifetime;
        }

        /// <summary>
        /// The lifetime of the service.
        /// </summary>
        public ServiceLifetime Lifetime { get; set; }
    }
}