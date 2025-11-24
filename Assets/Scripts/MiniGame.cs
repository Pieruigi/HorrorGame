using System.Collections;
using System.Collections.Generic;
using PSXShadersPro.URP.Demo;
using StarterAssets;
using UnityEngine;
using UnityEngine.Diagnostics;

namespace TMM
{
	public class MiniGame : MonoBehaviour
	{
		public delegate void MiniGameBeatenDelegate(MiniGame miniGame);
		public static MiniGameBeatenDelegate OnMiniGameBeaten;

		[SerializeField]
		Transform playerTarget;

		float attempts;

		FirstPersonController player;

		Transform cameraRoot;

	    // Start is called before the first frame update
	    void Start()
	    {
			player = FindFirstObjectByType<FirstPersonController>();
			cameraRoot = player.GetComponent<CameraShake>().transform;
	    }

		// Update is called once per frame
		void Update()
		{

		}

		public void Activate()
        {
			if (attempts <= 0) return;

			// Stop player from moving
			player.InputDisabled = true;
			
        }
		
		
	}
}
