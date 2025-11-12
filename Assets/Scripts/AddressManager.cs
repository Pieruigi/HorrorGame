using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace TMM
{

	[System.Serializable]
	public class Address
	{
		[SerializeField]
		string street;

		[SerializeField]
		int number;

		public Address(string street, int number)
        {
			this.street = street;
			this.number = number;
        }
	}

	public class AddressManager : Singleton<AddressManager>
	{
		List<Address> addresses = new List<Address>()
		{
			new Address("Via A", 1),
			new Address("Via A", 2),
			new Address("Via B", 1),
			new Address("Via B", 2),
			new Address("Via B", 3),
			new Address("Via C", 1),
			new Address("Via C", 2),
			new Address("Via D", 1),
			new Address("Via D", 2),
			new Address("Via D", 3),
			new Address("Via D", 4),
		};

		public IList<Address> Addresses
        {
            get{ return addresses.AsReadOnly(); }
        }

		// Start is called before the first frame update
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{

		}
	}
}
