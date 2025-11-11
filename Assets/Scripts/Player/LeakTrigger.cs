using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
	public class LeakTrigger : MonoBehaviour
	{
		[SerializeField]
		Leak leak;

		public Leak Leak
		{
			get { return leak; }
		}

		[SerializeField]
		GameObject medicinePrefab;

		[SerializeField]
		Transform medicineSpawnPoint;

		GameObject medicine;

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

		public void ShowMedicine()
		{
			medicine = Instantiate(medicinePrefab, medicineSpawnPoint.position, medicineSpawnPoint.rotation);
			
        }

		public void HideMedicine()
        {
			Destroy(medicine);
        }
    }
}
