using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
	public class MailBox : MonoBehaviour
	{
		List<Letter> letters = new List<Letter>();

	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }

		// Update is called once per frame
		void Update()
		{

		}

		public List<Letter> CollectLetterAll()
		{
			var ret = letters;
			letters.Clear();
			return ret;
		}
		
		public void AddLetter(Letter letter)
        {
			letters.Add(letter);
        }
	}
}
