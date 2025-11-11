using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace TMM
{
	public enum Application { None, Capsule, Needle, Drug }

	public enum Symptom { Head, Chest, Arms, Legs }

	

	[System.Serializable]	
	public class Illness
	{
		
		[SerializeField]
		string _name = "";

		[SerializeField]
		Symptom[] symptoms;

		public IList<Symptom> Symptoms
		{
			get { return symptoms.ToList().AsReadOnly(); }
		}

		

		[SerializeField]
		Application[] neededApplications;

	   
		// Init the new illness
		public Illness(int typeId)
		{
			switch (typeId)
			{
				case 0:
					_name = "Sudor pustulosus";
					symptoms = new Symptom[] { Symptom.Chest, Symptom.Arms };
					neededApplications = new Application[] { Application.Capsule, Application.Needle };
					break;
				case 1:
					_name = "Fetor mortis";
					symptoms = new Symptom[] { Symptom.Legs, Symptom.Arms };
					neededApplications = new Application[] { Application.Capsule, Application.Drug };
					break;
				case 2:
					_name = "Senilitas putrida";
					symptoms = new Symptom[] { Symptom.Head, Symptom.Chest };
					neededApplications = new Application[] { Application.Needle, Application.Drug };
					break;
				case 3:
					_name = "Halitus mortis";
					symptoms = new Symptom[] { Symptom.Head, Symptom.Legs };
					neededApplications = new Application[] { Application.Needle, Application.Capsule };
					break;
				case 4:
					_name = "Putridus pustulosus";
					symptoms = new Symptom[] { Symptom.Head, Symptom.Arms };
					neededApplications = new Application[] { Application.Drug, Application.Capsule };
					break;
				case 5:
					_name = "Fetores sudati";
					symptoms = new Symptom[] { Symptom.Chest, Symptom.Legs };
					neededApplications = new Application[] { Application.Drug, Application.Needle };
					break;
			}
		}


		public static Illness CreateRandomIllness()
		{
			Illness ret = new Illness(Random.Range(0, 6));
			return ret;
		}


		public int CheckApplications(List<Application> applications)
		{
			// No applications (00)
			if (applications.Count == 0)
				return GetApplicationRandomResult(0, 10, 90, 0);

			// Just the first application (V0, -0, X0)
			if (applications.Count == 1)
			{
				// V0: the first is right
				if (neededApplications[0] == applications[0])
					return GetApplicationRandomResult(40, 45, 15, 0);
				// -0: the first is in the wrong position
				if (applications[0] == neededApplications[1])
					return GetApplicationRandomResult(30, 50, 20, 0);
				// X0: the first is wrong
				return GetApplicationRandomResult(0, 0, 60, 40);
			}

			// Both applications applied (VV, --, VX or XV, -X or X-, )
            
			// VV: right applications
			if (neededApplications[0] == applications[0] && neededApplications[1] == applications[1])
				return 1;

			// --: right applications but in the wong order
			if (neededApplications[0] == applications[1] && neededApplications[1] == applications[0])
				return GetApplicationRandomResult(70, 20, 10, 0);

			// VX or XV: the first application is right, the second is missing or viceversa
			if ((neededApplications[0] == applications[0] && neededApplications[1] != applications[1]) ||
				(neededApplications[0] != applications[0] && neededApplications[1] == applications[1]))
				return GetApplicationRandomResult(0, 20, 55, 25);
				
			// -X or X-: the first is in the wrong position, the second is wrong or viceversa
			if ((neededApplications[0] == applications[1] /*&& neededApplications[1] != applications[0]*/) ||
				(/*neededApplications[0] != applications[1] &&*/ neededApplications[1] == applications[0]))
				return GetApplicationRandomResult(0, 10, 65, 25);
		
			return 0;
		}
		
		int GetApplicationRandomResult(int b, int s, int w, int w2)
		{
			Debug.Log($"TEST - Illness - B:{b}, S:{s}, W:{w}, W2:{w2}");
			if(b+s+w+w2 != 100)
            {
				Debug.LogError($"b+s+w+w2 != 100; {b},{s},{w},{w2}");
				return 0;
            }
			
			List<int> ret = new List<int>();
			for (int i = 0; i < b; i++)
				ret.Add(1);
			for (int i = 0; i < s; i++)
				ret.Add(0);
			for (int i = 0; i < w; i++)
				ret.Add(-1);
			for (int i = 0; i < w2; i++)
				ret.Add(-2);

			return ret[Random.Range(0, ret.Count)];
        }
        
		
	}
}
