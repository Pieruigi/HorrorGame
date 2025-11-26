using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
	public class ExitDoor : MonoBehaviour
	{
		[SerializeField]
		Light _light;

		[SerializeField]
		Color closedColor;

		[SerializeField]
		Color openColor;

		[SerializeField]
		GameObject door;

		[SerializeField]
		Renderer lightRenderer;

		[SerializeField]
		Material closedMaterial;

		[SerializeField]
		Material openMaterial;

		int materialId = 3;

        void Awake()
        {
			var mats = lightRenderer.materials;
			if (mats[materialId] != closedMaterial)
			{
				mats[materialId] = closedMaterial;
				lightRenderer.materials = mats;
			}
			_light.color = closedColor;
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
			MiniGame.OnMiniGameBeaten += HandleOnMiniGameBeaten;
		}

        void OnDisable()
        {
            MiniGame.OnMiniGameBeaten -= HandleOnMiniGameBeaten;
        }

        private void HandleOnMiniGameBeaten(MiniGame miniGame)
        {
			var mats = lightRenderer.materials;
			mats[materialId] = openMaterial;
			lightRenderer.materials = mats;
			_light.color = openColor;
			var pos = door.transform.position;
			pos.y = 2.3f;
			door.transform.position = pos;

        }
    }
}
