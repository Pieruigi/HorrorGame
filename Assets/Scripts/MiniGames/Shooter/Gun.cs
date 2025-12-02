using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
		Renderer lightRenderer;

		[SerializeField]
		Material greenMaterial;

		[SerializeField]
		Material redMaterial;

		[SerializeField]
		float rate = 1;

		float cooldown = 0;

		bool active = false;

		Transform parentDefault;

		GameObject gunRoot;

        void Awake()
        {
			parentDefault = transform.parent;
			gunRoot = transform.GetChild(0).gameObject;
			gunRoot.SetActive(false);
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
            {
				cooldown -= Time.deltaTime;
				if (cooldown <= 0)
					lightRenderer.material = greenMaterial;
            }
				



			if (Input.GetMouseButton(0))
			{
				if (cooldown <= 0)
				{
					cooldown = 1/rate;
					// Shoot
					var bullet = Instantiate(bulletPrefab);
					bullet.GetComponent<Rigidbody>().position = bulletTarget.position;
					bullet.transform.rotation = bulletTarget.rotation;
					bullet.GetComponent<Rigidbody>().AddForce(bulletTarget.forward * 20, ForceMode.VelocityChange);

					// Set red light
					lightRenderer.material = redMaterial;
					//Physics.IgnoreCollision(GetComponent<Collider>(), bullet.GetComponent<Collider>(), true);
				}
			}
		}
		
		public void Activate(bool value)
        {
			active = value;
			cooldown = 0.25f;
			lightRenderer.material = redMaterial;

			float moveTime = .25f;

			if (!value)
			{
				transform.DOLocalMoveZ(0, moveTime).OnComplete(() =>
                {
                	transform.parent = parentDefault;
					transform.localPosition = Vector3.zero;
					transform.localRotation = Quaternion.identity;
					gunRoot.SetActive(false);
                });

				
			}
            else
			{
				transform.parent = Camera.main.transform;
				transform.localPosition = Vector3.down * .136f;
				transform.localRotation = Quaternion.identity;
				gunRoot.SetActive(true);

				transform.DOLocalMoveZ(.321f, moveTime);
				//transform.localPosition = Vector3.down * .136f + Vector3.forward * .321f;
				
            }
        }
	}
}
