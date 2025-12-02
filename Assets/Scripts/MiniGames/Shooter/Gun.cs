using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
	public class Gun : MonoBehaviour
	{
		[SerializeField]
		GameObject bulletPrefab;

		[SerializeField]
		Transform bulletTarget;

		[SerializeField]
		float rate = 1;

		float cooldown = 0;

		bool active = false;

		Transform parentDefault;

        void Awake()
        {
			parentDefault = transform.parent;
        }

        // Start is called before the first frame update
        void Start()
	    {
	        
	    }

		// Update is called once per frame
		void Update()
		{
			if (!active) return;

			if (cooldown > 0)
				cooldown -= Time.deltaTime;

			if (Input.GetMouseButton(0))
			{
				if (cooldown <= 0)
				{
					cooldown = rate;
					// Shoot
					var bullet = Instantiate(bulletPrefab);
					bullet.GetComponent<Rigidbody>().position = bulletTarget.position;
					bullet.transform.rotation = bulletTarget.rotation;
					bullet.GetComponent<Rigidbody>().AddForce(bulletTarget.forward * 20, ForceMode.VelocityChange);
					//Physics.IgnoreCollision(GetComponent<Collider>(), bullet.GetComponent<Collider>(), true);
				}
			}
		}
		
		public void Activate(bool value)
        {
			active = value;
			cooldown = 0.25f;

			if (!value)
			{
				transform.parent = parentDefault;
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
			}
            else
            {
				transform.parent = Camera.main.transform;
				transform.localPosition = Vector3.down * .136f + Vector3.forward * .321f;
				transform.localRotation = Quaternion.identity;
            }
        }
	}
}
