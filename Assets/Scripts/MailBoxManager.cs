using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace TMM
{
	public class MailboxManager : Singleton<MailboxManager>
	{

		List<Mailbox> mailboxes;
		public IList<Mailbox> Mailboxes
        {
            get{ return mailboxes.AsReadOnly(); }
        }

	    // Start is called before the first frame update
	    void Start()
		{
			// Get all mailboxes in scene
			mailboxes = FindObjectsByType<Mailbox>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList(); 
	    }

		// Update is called once per frame
		void Update()
		{

		}

		
		public void Init(List<Mail> mails)
        {
            for(int i=0; i<mails.Count; i++)
            {
                mailboxes[i % mailboxes.Count].AddMail(mails[i]);
            }
        }
        
    }
}
