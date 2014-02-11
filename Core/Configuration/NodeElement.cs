using System;
using System.Configuration;

namespace Enyim.Caching.Configuration
{
	public sealed class NodeElement : ConfigurationElement
	{
		private System.Net.IPEndPoint endpoint;

		/// <summary>
		/// Gets or sets the ip address of the node.
		/// </summary>
		[ConfigurationProperty("address", IsRequired = true, IsKey = true, DefaultValue = "")]
		[ConfigurationValidator(typeof(AddressValidator))]
		public string Address
		{
			get { return (string)base["address"]; }
			set { base["address"] = value; }
		}

		/// <summary>
		/// Gets the <see cref="T:IPEndPoint"/> representation of this instance.
		/// </summary>
		public System.Net.IPEndPoint EndPoint
		{
			get { return this.endpoint ?? (this.endpoint = ConfigurationHelper.ResolveToEndPoint(this.Address)); }
		}

		#region [ AddressValidator             ]

		private class AddressValidator : ConfigurationValidatorBase
		{
			public override bool CanValidate(Type type)
			{
				return (type == typeof(string)) || base.CanValidate(type);
			}

			public override void Validate(object value)
			{
				var address = Convert.ToString(value);

				if (!String.IsNullOrEmpty(address) && address.LastIndexOf(':') < 1)
					throw new ConfigurationErrorsException("Invalid address specified: " + value);
			}
		}

		#endregion
	}
}
