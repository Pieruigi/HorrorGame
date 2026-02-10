using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TMM
{
	public class Spaceship : MonoBehaviour
	{
		bool destroyed = false;
		public bool Destroyed { get { return destroyed; } }

		Collider _collider;

		List<Renderer> renderers;

        private void Awake()
        {
            _collider = GetComponent<Collider>();	
        }

        // Start is called before the first frame update
        void Start()
	    {
	        renderers = GetComponentsInChildren<Renderer>().ToList();
	    }

	    // Update is called once per frame
	    void Update()
	    {
	        
	    }

        private void OnCollisionEnter(Collision collision)
        {
            if(destroyed) return;

			SpaceBullet bullet = collision.collider.GetComponent<SpaceBullet>();

			if(bullet == null) return;

			destroyed = true;
			_collider.enabled = false;

			foreach(Renderer r in renderers)
				r.enabled = false;
        }
    }
}
