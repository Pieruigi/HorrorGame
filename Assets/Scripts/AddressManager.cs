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
		List<Address> addresses = new List<Address>();

		public IList<Address> Addresses
        {
            get{ return addresses.AsReadOnly(); }
        }

		// Start is called before the first frame update
		void Start()
		{
			// Get all the letterboxes
			var letterboxes = FindObjectsByType<Letterbox>(FindObjectsInactive.Include, FindObjectsSortMode.None);

			// Read all the addresses
			foreach (var letterbox in letterboxes)
			{
				letterbox.Reset();
                addresses.Add(letterbox.Address);
            }
				
		}

		// Update is called once per frame
		void Update()
		{
			
		}
	}
}
