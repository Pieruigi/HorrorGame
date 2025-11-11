using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Windows;

namespace TMM
{
	/// <summary>
	/// White: no more illness
	/// Black: dead
	/// </summary>
	public enum Severity { Black, Red, Yellow, Green, White }

	public enum SubjectState { Bedbound, Patrol, Chase, Attack }
	
	public enum Leak {Head, Chest, Pelvis}

	public class Subject : MonoBehaviour
	{
		//public delegate void DestroyedDelegate(Subject subject);
		public UnityAction OnDestroyed;

		[SerializeField]
		List<Collider> leakTriggers;

		List<Leak> leaks = new List<Leak>();

		// string _name;
		// string surname;

		// int id;
		// public int Id
        // {
        //     get{ return id; }
        // }


//		int age;

		// Illness illness;
		// public Illness Illness
        // {
		// 	get { return illness; }
	    // }


		Severity severity;
		public Severity Severity
        {
			get { return severity; }
        }


		List<Application> applications = new List<Application>();

		bool firstDay = true;

		SubjectState state;

		NavMeshAgent agent;
		

        void Awake()
		{
			//illness = Illness.CreateRandomIllness();
			DisableLeakTriggerAll();
			severity = (Severity)Random.Range(1, 4);
			InitLeaks(severity);
			SetState(SubjectState.Bedbound);
        }

        // Start is called before the first frame update
        void Start()
	    {
	        
	    }

		// Update is called once per frame
		void Update()
		{
#if UNITY_EDITOR
			// if (Input.GetKeyDown(KeyCode.Alpha1))
			// 	Apply(Application.Capsule);

			// if (Input.GetKeyDown(KeyCode.Alpha2))
			// 	Apply(Application.Needle);

			// if (Input.GetKeyDown(KeyCode.Alpha3))
			// 	Apply(Application.Drug);

#endif

            
		}

		void OnEnable()
		{
			GameplayManager.OnDayStarted += HandleOnDayStarted;
			GameplayManager.OnNightStarted += HandleOnNightStarted;
		}

        void OnDisable()
        {
            GameplayManager.OnDayStarted -= HandleOnDayStarted;
			GameplayManager.OnNightStarted -= HandleOnNightStarted;
        }

        private void HandleOnDayStarted()
        {
            if(severity == Severity.Black)
			{
				// Die
				Die();
            }
        }

        private void HandleOnNightStarted()
		{
			// Check applications
			//CheckApplications();
        }

		// void CheckApplications()
		// {
		// 	Debug.Log($"TEST - Illness - Current severity:{severity}");
		// 	DebugApplications();
		// 	int ret = illness.CheckApplications(applications);
		// 	Debug.Log($"TEST - Illness - Applications result:{ret}");

		// 	severity = (Severity)Mathf.Clamp((int)severity + ret, (int)Severity.Black, (int)Severity.White);
		// 	Debug.Log($"TEST - Illness - Severity after treatment:{severity}");
		// 	applications.Clear();

		// 	// Check severity 
		// 	if(severity == Severity.Black)
		// 	{
		// 		// Subject become a psycho (no longer bedbound)
		// 		SetState(SubjectState.Patrol);
				
        //     }
        // }

		// void Apply(Application application)
		// {
		// 	if (applications.Count == 2) return;
		// 	if (applications.Contains(application)) return;

		// 	applications.Add(application);
		// }

		void Die()
		{
			Destroy(gameObject);
			OnDestroyed?.Invoke();
		}

		void SetState(SubjectState state)
		{
			switch (state)
			{
				case SubjectState.Bedbound:
					EnterBedboundState();
					break;
				case SubjectState.Patrol:
					EnterPatrolState();
					break;
			}
		}

		void EnterBedboundState()
        {
			// TODO: remove comments // agent.isStopped = true;
        }

		void EnterPatrolState()
		{

		}

		void InitLeaks(Severity severity)
		{

			this.severity = severity;

			if (severity == Severity.Red) // Three leaks
			{
				leaks = new List<Leak> { Leak.Head, Leak.Chest, Leak.Pelvis };
			}
			else if (severity == Severity.Green) // Just one
			{
				leaks = new List<Leak> { (Leak)Random.Range(0, 3) };
			}
			else // Two leaks
			{
				var all = new List<int>() { 0, 1, 2 };
				var l1 = all[Random.Range(0, all.Count)];
				all.Remove(l1);
				var l2 = all[Random.Range(0, all.Count)];
				leaks = new List<Leak>() { (Leak)l1, (Leak)l2 };
			}

			// Initialize triggers
			foreach (var leak in leaks)
			{
				leakTriggers[(int)leak].enabled = true;
			}


		}
		
		void DisableLeakTriggerAll()
        {
			foreach (var t in leakTriggers)
				t.enabled = false;
        }

		// void DebugApplications()
        // {
		// 	if (applications.Count == 0)
		// 		Debug.Log("TEST - Illness - Applications:[None,None]");
		// 	else if (applications.Count == 1)
		// 		Debug.Log($"TEST - Illness - Applications:[{applications[0]},None]");
		// 	else
		// 		Debug.Log($"TEST - Illness - Applications:[{applications[0]},{applications[1]}]");
        // }

	}
}
