using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
	public class MapUI : MonoBehaviour
	{
		[SerializeField]
		GameObject dayBlock;

		[SerializeField]
		GameObject nightBlock;

        void Awake()
        {
            dayBlock.SetActive(false);
			nightBlock.SetActive(false);
        }

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
			dayBlock.SetActive(false);
			nightBlock.SetActive(false);
            if (GameplayManager.Instance)
            {
				if (GameplayManager.Instance.NightShift)
					nightBlock.SetActive(true);
				else
					dayBlock.SetActive(true);    
            }
			
		}

        void OnDisable()
        {
			dayBlock.SetActive(false);
			nightBlock.SetActive(false);
        }
    }
}
