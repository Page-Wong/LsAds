using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Specialized;
using System.Text;
using System.Threading;

namespace LsAdmin.Utility.Service {
    public static class HttpHelper
    {
        public static IServiceProvider ServiceProvider;
        public static HttpContext GetContext() {
            object factory = ServiceProvider.GetService(typeof(IHttpContextAccessor));
            HttpContext context = ((IHttpContextAccessor)factory).HttpContext;
            return context;
        }

        public static ISession GetSession() {
            return GetContext().Session;
        }
    }
}
