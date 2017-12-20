using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Settings.Tests.Models
{
	public class TestUser
	{
		public string Username { get; set; }
		public int Id { get; set; }
		public string Email { get; set; }

		public Address Address { get; set; }
	}

	public class Address
	{
		public string Street { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string Zip { get; set; }
	}
}
