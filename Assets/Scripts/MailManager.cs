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
			MailManager.Instance.ReportMailCollected(this);
			
		}
		
		public void SetDelivered()
        {
			delivered = true;
			MailManager.Instance.ReportMailDelivered(this);
        }
	}




	public class MailManager : Singleton<MailManager>
	{
	

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

		// void OnEnable()
		// {
		// 	GameplayManager.OnWorkShiftStarted += HandleOnNextShiftStarted;
			
		// }

		// void OnDisable()
		// {
		// 	GameplayManager.OnWorkShiftStarted -= HandleOnNextShiftStarted;
			
		// }

      

        /// <summary>
        /// We must create letters and fill mailboxes or the player bag depending whether we are playing day or night shift
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public void InitShift(int day, bool isNightShift)
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
			Debug.Log("Init Nightshift");
		}
		
		/// <summary>
        /// Create letters and fill mailboxes
        /// </summary>
        /// <param name="day"></param>
		void InitDayShift(int day)
		{
			mails.Clear();

			int mailCount = 5; // Depending on the day???
			
			for (int i = 0; i < mailCount; i++)
				CreateMail();

			MailboxManager.Instance.Init(mails);
			
        }

	
		public void CreateMail()
		{

			// Get all the addresses from the address manager which are not used yet
			var letterboxes = LetterboxManager.Instance.Letterboxes.Where(l => !l.Used).ToList();
			//var addresses = AddressManager.Instance.Addresses.Where(a => !mails.Exists(l => l.Address == a)).ToList();

			// Get a random address from the filtred list
			var letterbox = letterboxes[Random.Range(0, letterboxes.Count)];

			letterbox.Used = true;

			// Create a new letter
			var letter = new Mail(letterbox.Address);

			// Add the letter to the list
			mails.Add(letter);

		}

		public bool MailDeliveredAll()
		{
			return !mails.Exists(l => !l.Delivered);
		}

		public bool MailCollectedAll()
		{
			return !mails.Exists(l => !l.Collected);
		}

		public void ReportMailCollected(Mail mail)
		{
			if (MailCollectedAll())
				GameplayManager.Instance.ReportMailCollectedAll();
		}
		
		public void ReportMailDelivered(Mail mail)
        {
			if (MailDeliveredAll())
				GameplayManager.Instance.ReportMailDeliveredAll();
        }

		
	}
}
