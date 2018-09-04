using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus.UnitOfWork;

namespace NServiceBus.ObjectBuilder.MSDependencyInjection
{
    /// <summary>
    /// Implements a UnitOfWork to create a scope for the dependencyInjection
    /// </summary>
    internal class UnitOfWorkScope : IManageUnitsOfWork
    {
        private readonly ServicesObjectBuilder _serviceProvider;        

        public UnitOfWorkScope(ServicesObjectBuilder builder)
        {
            _serviceProvider = builder ?? throw new ArgumentNullException(nameof(builder));
        }

        public Task Begin()
        {
            _serviceProvider.BeginScope();
            return Task.CompletedTask;
        }

        public Task End(Exception ex = null)
        {
            _serviceProvider.EndScope();
            return Task.CompletedTask;
        }
    }
}
