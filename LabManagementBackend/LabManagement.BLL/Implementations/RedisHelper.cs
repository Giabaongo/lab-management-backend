
using LabManagement.BLL.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabManagement.BLL.Implementations
{
    public class RedisHelper : IRedisHelper
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public RedisHelper(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
        }

        private IDatabase GetDatabase()
        {
        
            return _connectionMultiplexer.GetDatabase();
        }
        public async Task<string?> GetAsync(string key)
        {
            var db = GetDatabase();
          
            return await db.StringGetAsync(key);
        }
        public async Task SetAsync(string key, string value, TimeSpan? expiry = null)
        {
            var db = GetDatabase();
           
            await db.StringSetAsync(key, value, expiry);
        }

        public async Task<bool> DeleteAsync(string key)
        {
            var db = GetDatabase();
          
            return await db.KeyDeleteAsync(key);
        }


    }
}
