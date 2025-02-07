using Microsoft.Extensions.Configuration;
using NS.Quizzy.Server.DAL;
using NS.Shared.CacheProvider.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NS.Quizzy.Server.BL.AppConfiguration
{
    internal class DbConfigurationSource : IConfigurationSource
    {
        private readonly IServiceProvider _rootServiceProvider;

        public DbConfigurationSource(IServiceProvider rootServiceProvider)
        {
            _rootServiceProvider = rootServiceProvider;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new DbConfigurationProvider(_rootServiceProvider);
        }
    }
}
