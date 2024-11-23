using Microsoft.Extensions.DependencyInjection;
using NS.Quizzy.Server.DAL.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.Quizzy.Server.BL.Extensions
{
    public static class ExtensionMethods
    {
        public static IServiceCollection AddBLServices(this IServiceCollection services)
        {
            services.AddDALServices();
            return services;
        }
    }
}
