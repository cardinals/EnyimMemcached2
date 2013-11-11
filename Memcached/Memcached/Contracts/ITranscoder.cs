using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enyim.Caching.Memcached
{
	public interface ITranscoder
	{
		/// <summary>
		/// Serializes an object for storing in the cache.
		/// </summary>
		/// <param name="value">The object to serialize</param>
		/// <returns>The serialized object</returns>
		CacheItem Serialize(object value);

		/// <summary>
		/// Deserializes the <see cref="T:CacheItem"/> into an object.
		/// </summary>
		/// <param name="item">The stream that contains the data to deserialize.</param>
		/// <returns>The deserialized object</returns>
		object Deserialize(CacheItem item);
	}
}
