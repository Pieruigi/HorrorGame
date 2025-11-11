using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace TMM
{
	public class SubjectInfo : MonoBehaviour
	{
		[SerializeField]
		TMP_Text numField;

		[SerializeField]
		List<GameObject> symptomFlags;

		[SerializeField]
		List<Color> colors;

		[SerializeField]
		GameObject root;

		string nameFormatString = "SUBJECT {0:D4}";

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

		// public void Init(Subject subject)
		// {
		// 	root.SetActive(true);
		// 	var id = subject.Id;
		// 	var illness = subject.Illness;

		// 	// Set number and color
		// 	numField.text = string.Format(nameFormatString, id);
		// 	numField.color = colors[(int)subject.Severity];

		// 	foreach (var s in symptomFlags)
		// 		s.SetActive(false);

		// 	for (int i = 0; i < illness.Symptoms.Count; i++)
		// 		symptomFlags[(int)illness.Symptoms[i]].SetActive(true);
		// }
		
		public void Clear()
        {
            root.SetActive(false);
        }
		
	}
}
