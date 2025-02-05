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
        private readonly AppDbContext _dbContext;
        private readonly INSCacheProvider _cacheProvider;

        public DbConfigurationSource(AppDbContext dbContext, INSCacheProvider cacheProvider)
        {
            _dbContext = dbContext;
            _cacheProvider = cacheProvider;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new DbConfigurationProvider(_dbContext, _cacheProvider);
        }
    }
}
