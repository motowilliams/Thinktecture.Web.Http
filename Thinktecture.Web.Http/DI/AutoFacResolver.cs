﻿using System;
using System.Collections.Generic;
using System.Web.Http.Services;
using Autofac;

namespace Thinktecture.Web.Http.DI
{
    // NOTE: this is not working the way it should be - still investigating...
    public class AutoFacResolver : IDependencyResolver
    {
        private readonly IContainer container;

        public AutoFacResolver(IContainer container) : base()
        {
            this.container = container;
        }

        public object GetService(Type serviceType)
        {
            return container.Resolve(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return (IEnumerable<object>)container.Resolve(typeof(IEnumerable<>).MakeGenericType(new[] { serviceType }));
        }
    }
}
