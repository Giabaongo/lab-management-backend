using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabManagement.BLL.Interfaces
{
    public interface IRedisHelper
    {
 
        Task<string?> GetAsync(string key);

   
        Task SetAsync(string key, string value, TimeSpan? expiry = null);


        Task<bool> DeleteAsync(string key);
    }
}
