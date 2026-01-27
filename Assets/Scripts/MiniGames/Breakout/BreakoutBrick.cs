using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
	public class BreakoutBrick : MonoBehaviour
	{
		[SerializeField]
		GameObject mesh;
	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }

	    // Update is called once per frame
	    void Update()
	    {
	        
	    }

        private void OnCollisionEnter(Collision collision)
        {
            BreakoutBall ball = collision.collider.GetComponent<BreakoutBall>();
			if (ball == null) return;

			Explode();
        }

		void Explode()
		{
			mesh.SetActive(false);
			GetComponent<Collider>().enabled = false;
		}
    }
}
