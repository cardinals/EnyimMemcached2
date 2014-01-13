using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;

namespace Enyim.Caching.Configuration
{
	/// <summary>
	/// Represents a collection of <see cref="T:EndPointElement"/> instances. This class cannot be inherited.
	/// </summary>
	public sealed class NodeElementCollection : ConfigurationElementCollection
	{
		/// <summary>
		/// Creates a new <see cref="T:ConfigurationElement"/>.
		/// </summary>
		/// <returns>A new <see cref="T:ConfigurationElement"/>.</returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new NodeElement();
		}

		/// <summary>
		/// Gets the element key for a specified configuration element when overridden in a derived class.
		/// </summary>
		/// <param name="element">The <see cref="T:ConfigurationElement"/> to return the key for. </param>
		/// <returns>An <see cref="T:Object"/> that acts as the key for the specified <see cref="T:ConfigurationElement"/>.</returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((NodeElement)element).Address;
		}

		/// <summary>
		/// Helper method; converts the collection into an <see cref="T:IPEndPoint"/> collection for the interface implementation.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IPEndPoint> ToIPEndPoints()
		{
			return this.OfType<NodeElement>().Select(e => e.EndPoint);
		}
	}
}
