using System.Collections;
using System.Collections.Generic;
using TMM.Interfaces;
using UnityEngine;

namespace TMM
{
	public class Bullet : MonoBehaviour
	{
	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }

		// Update is called once per frame
		void Update()
		{

		}

        void OnCollisionEnter(Collision collision)
		{
			
			//GetComponent<Collider>().enabled = false;
			// ITarget iT = collision.gameObject.GetComponent<ITarget>();
			// if (iT != null)
			// {
			// 	Debug.Log("HIT - Bullet - IT");
			// 	iT.Hit(gameObject);

			// }
            // else
            // {
            //     Debug.Log("HIT - Bullet - " + collision.gameObject);
            // }
        	Destroy(gameObject);    
			
        }
    }
}
