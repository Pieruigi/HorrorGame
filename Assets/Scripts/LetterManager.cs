using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace TMM
{
	[System.Serializable]
	public class Letter
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
            get{ return collected; }
        }

		[SerializeField]
		bool delivered = false;
		public bool Delivered
        {
            get{ return delivered; }
        }

		public Letter(Address address)
		{
			this.address = address;
		}
	}




	public class LetterManager : MonoBehaviour
	{

		[SerializeField]
		List<Letter> letters = new List<Letter>();

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
			int letterCount = 10; // Depending on the day???
			int mailBoxCount = MailBoxManager.Instance.MailBoxes.Count;

			for (int i = 0; i < letterCount; i++)
			{
				var letter = CreateLetter();
				letters.Add(letter);
				MailBoxManager.Instance.MailBoxes[i % mailBoxCount].AddLetter(letter);
			}
			
							

        }

		public void AddLetter(Letter letter)
		{
			letters.Add(letter);
		}

		public void RemoveLetter(Letter letter)
		{
			letters.Remove(letter);
		}

		public Letter CreateLetter()
		{
			// Get all the addresses from the address manager which are not used yet
			var addresses = AddressManager.Instance.Addresses.Where(a => !letters.Exists(l => l.Address == a)).ToList();

			// Get a random address from the filtred list
			var address = addresses[Random.Range(0, addresses.Count)];

			// Create a new letter
			var letter = new Letter(address);

			// Add the letter to the list
			letters.Add(letter);

			// Return
			return letter;
		}

		public bool LetterDeliveredAll()
		{
			return !letters.Exists(l => !l.Delivered);
		}
		
		public bool LetterCollectedAll()
        {
			return !letters.Exists(l => !l.Collected);
        }
	}
}
