using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
	public class BreakoutBrick : MonoBehaviour
	{
		[SerializeField]
		GameObject mesh;

		[SerializeField]
		ParticleSystem destroyParticle;

		Breakout minigame;

        private void Awake()
        {
            minigame = transform.root.GetComponentInChildren<Breakout>();
			minigame.ReportBrickAdded(this);
        }

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

			minigame.ReportBrickHit(this);

			Explode();
						

        }

		void Explode()
		{
			destroyParticle.Play();
			mesh.SetActive(false);
			GetComponent<Collider>().enabled = false;
		}
    }
}
