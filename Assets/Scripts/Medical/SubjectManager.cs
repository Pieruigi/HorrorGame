using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
	public class SubjectManager : Singleton<SubjectManager>
	{
		[SerializeField]
		GameObject subjectPrefab;

		List<Subject> subjects = new List<Subject>();

		int counter = 0;

	    // Start is called before the first frame update
	    void Start()
		{


			// Create a new subject
			SpawnSubject();
	    }

		// Update is called once per frame
		void Update()
		{

		}



		void SpawnSubject()
		{
			

			// Get free bed 
			var bed = BedManager.Instance.GetRandomFreeBed();
			if(!bed)
            {
				Debug.LogWarning("No free beds");
				return;
            }

			GameObject go = GameObject.Instantiate(subjectPrefab);

			
			var subject = go.GetComponent<Subject>();
			subject.Init(++counter);
			subject.transform.position = bed.SubjectSpawnPoint.position;
			subject.transform.rotation = bed.SubjectSpawnPoint.rotation;
			bed.SetBusy(subject);
			subject.OnDestroyed += () => { subjects.Remove(subject); };
			subjects.Add(subject);
		}
		
		
	}
}
