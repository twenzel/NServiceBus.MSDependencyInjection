using Microsoft.Extensions.DependencyInjection;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NServiceBus.MSDependencyInjection.Tests
{
    [TestFixture]
    public class DisposalTests
    {
        [Test]
        public void Owned_container_should_be_disposed()
        {
            var fakesServiceProvider = new FakeServiceProvider();

            var container = new ServicesObjectBuilder(fakesServiceProvider, true);
            container.Dispose();

            Assert.True(fakesServiceProvider.Disposed);
        }

        [Test]
        public void Externally_owned_container_should_not_be_disposed()
        {
            var fakesServiceProvider = new FakeServiceProvider();

            var container = new ServicesObjectBuilder(fakesServiceProvider, false);
            container.Dispose();

            Assert.False(fakesServiceProvider.Disposed);
        }

        class FakeServiceProvider : IServiceCollection, IDisposable
        {
            public ServiceDescriptor this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public bool Disposed { get; private set; }

            public int Count => throw new NotImplementedException();

            public bool IsReadOnly => throw new NotImplementedException();

            public void Add(ServiceDescriptor item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(ServiceDescriptor item)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {
                Disposed = true;
            }

            public IEnumerator<ServiceDescriptor> GetEnumerator()
            {
                var list = new List<ServiceDescriptor>();
                foreach (var item in list)
                    yield return item;
            }

            public int IndexOf(ServiceDescriptor item)
            {
                throw new NotImplementedException();
            }

            public void Insert(int index, ServiceDescriptor item)
            {
                throw new NotImplementedException();
            }

            public bool Remove(ServiceDescriptor item)
            {
                throw new NotImplementedException();
            }

            public void RemoveAt(int index)
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }
    }
}
