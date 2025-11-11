using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

namespace TMM
{
	public class Bed : MonoBehaviour
	{
		[SerializeField]
		Transform subjectSpawnPoint;

		// [SerializeField]
		// SubjectInfo subjectInfo;

		public Transform SubjectSpawnPoint
		{
			get { return subjectSpawnPoint; }
		}

		Subject subject = null;

		public bool IsFree
        {
            get{ return !subject; }
        }

		void Awake()
        {
		
        }

        // Start is called before the first frame update
        void Start()
	    {
	        
	    }

		// Update is called once per frame
		void Update()
		{

		}

		public void SetBusy(Subject subject)
		{
			this.subject = subject;
			subject.OnDestroyed += () => { SetFree(); };
			//subjectInfo.Init(subject);
		}
		
		public void SetFree()
		{
			subject = null;
			//subjectInfo.Clear();
        }
		

    }
}
