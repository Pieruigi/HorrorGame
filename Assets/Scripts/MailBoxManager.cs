using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace TMM
{
	public class MailBoxManager : Singleton<MailBoxManager>
	{

		List<MailBox> mailBoxes;
		public IList<MailBox> MailBoxes
        {
            get{ return mailBoxes.AsReadOnly(); }
        }

	    // Start is called before the first frame update
	    void Start()
		{
			// Get all mailboxes in scene
			mailBoxes = FindObjectsByType<MailBox>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList(); 
	    }

		// Update is called once per frame
		void Update()
		{

		}
		
		
	}
}
