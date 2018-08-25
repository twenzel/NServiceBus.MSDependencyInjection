using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NServiceBus.MSDependencyInjection.AcceptanceTests
{
    class ServiceCollectionDecorator : IServiceCollection, IDisposable
    {
        private readonly IServiceCollection _services;

        public bool Disposed { get; private set; }

        public int Count => _services.Count;

        public bool IsReadOnly => _services.IsReadOnly;

        public ServiceDescriptor this[int index] { get => _services[index]; set => _services[index] = value; }

        public ServiceCollectionDecorator(IServiceCollection services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public void Dispose()
        {
            (_services as IDisposable)?.Dispose();
            Disposed = true;
        }

        public int IndexOf(ServiceDescriptor item)
        {
            return _services.IndexOf(item);
        }

        public void Insert(int index, ServiceDescriptor item)
        {
            _services.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _services.RemoveAt(index);
        }

        public void Add(ServiceDescriptor item)
        {
            _services.Add(item);
        }

        public void Clear()
        {
            _services.Clear();
        }

        public bool Contains(ServiceDescriptor item)
        {
            return _services.Contains(item);
        }

        public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
        {
            _services.CopyTo(array, arrayIndex);
        }

        public bool Remove(ServiceDescriptor item)
        {
            return _services.Remove(item);
        }

        public IEnumerator<ServiceDescriptor> GetEnumerator()
        {
            return _services.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _services.GetEnumerator();
        }
    }
}
