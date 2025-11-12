using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Events;

namespace TMM
{
	[System.Serializable]
	public class Mail
	{
		public delegate void CollectedDelegate(Mail mail);
		public static CollectedDelegate OnCollected;


		public delegate void DeliveredDelegate(Mail mail);
		public static DeliveredDelegate OnDelivered;


		[SerializeField]
		Address address;

		public Address Address
        {
            get{ return address; }
        }

		// True if you get the letter from the boxmail, otherwise false
		[SerializeField]
		bool collected = false;
		public bool Collected
        {
			get { return collected; }
			//set{ collected = value; }
        }

		[SerializeField]
		bool delivered = false;
		public bool Delivered
        {
			get { return delivered; }
			//set{ delivered = value; }
        }

		public Mail(Address address)
		{
			this.address = address;
		}

		public void SetCollected()
		{
			collected = true;
			OnCollected?.Invoke(this);
		}
		
		public void SetDelivered()
        {
			delivered = true;
			OnDelivered?.Invoke(this);
        }
	}




	public class MailManager : Singleton<MailManager>
	{
		public static UnityAction OnMailCollectedAll;
		public static UnityAction OnMailDeliveredAll;

		[SerializeField]
		List<Mail> mails = new List<Mail>();

		public IList<Mail> Mails
		{
			get { return mails.AsReadOnly(); }
		}

	

	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }

		// Update is called once per frame
		void Update()
		{

		}

		void OnEnable()
		{
			GameplayManager.OnWorkShiftStarted += HandleOnNextShiftStarted;
			
		}

		void OnDisable()
		{
			GameplayManager.OnWorkShiftStarted -= HandleOnNextShiftStarted;
			
		}

      

        /// <summary>
        /// We must create letters and fill mailboxes or the player bag depending whether we are playing day or night shift
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        private void HandleOnNextShiftStarted(int day, bool isNightShift)
		{
			
			if (isNightShift)
			{
				// Init night shift
			}
            else
			{
				// Init day shift
				InitDayShift(day);
            }
		}

		void InitNightShift()
		{

		}
		
		/// <summary>
        /// Create letters and fill mailboxes
        /// </summary>
        /// <param name="day"></param>
		void InitDayShift(int day)
		{
			int letterCount = 5; // Depending on the day???
			int mailBoxCount = MailboxManager.Instance.Mailboxes.Count;

			for (int i = 0; i < letterCount; i++)
			{
				var letter = CreateMail();
				MailboxManager.Instance.Mailboxes[i % mailBoxCount].AddMail(letter);
			}
			
							

        }

	
		public Mail CreateMail()
		{
			// Get all the addresses from the address manager which are not used yet
			var addresses = AddressManager.Instance.Addresses.Where(a => !mails.Exists(l => l.Address == a)).ToList();

			// Get a random address from the filtred list
			var address = addresses[Random.Range(0, addresses.Count)];

			// Create a new letter
			var letter = new Mail(address);

			// Add the letter to the list
			mails.Add(letter);

			// Return
			return letter;
		}

		public bool MailDeliveredAll()
		{
			return !mails.Exists(l => !l.Delivered);
		}

		public bool MailCollectedAll()
		{
			return !mails.Exists(l => !l.Collected);
		}

		
	}
}
