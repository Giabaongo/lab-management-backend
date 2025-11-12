using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabManagement.BLL.Interfaces
{
    public interface IRedisHelper
    {
        /// <summary>
        /// Gets a string value from the cache.
        /// </summary>
        /// <param name="key">The key of the item to get.</param>
        /// <returns>The cached string value, or null if not found.</returns>
        Task<string?> GetAsync(string key);

        /// <summary>
        /// Sets a string value in the cache.
        /// </summary>
        /// <param name="key">The key of the item to set.</param>
        /// <param name="value">The string value to store.</param>
        /// <param name="expiry">Optional: How long the item should stay in the cache.</param>
        Task SetAsync(string key, string value, TimeSpan? expiry = null);

        /// <summary>
        /// Deletes a value from the cache.
        /// </summary>
        /// <param name="key">The key of the item to delete.</param>
        /// <returns>True if the item was deleted, false otherwise.</returns>
        Task<bool> DeleteAsync(string key);
    }
}
