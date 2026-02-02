using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TMM
{
	public class VersionUI : MonoBehaviour
	{
		[SerializeField]
		TMP_Text textfield;

        private void Awake()
        {
			textfield.text = Application.version;
        }

        // Start is called before the first frame update
        void Start()
	    {
	        
	    }

	    // Update is called once per frame
	    void Update()
	    {
	        
	    }
	}
}
