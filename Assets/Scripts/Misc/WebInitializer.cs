
using System.Collections;
using System.Collections.Generic;
using RetroShadersPro.URP;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace TMM
{
	public class WebInitializer : MonoBehaviour
	{
		[SerializeField]
		RawImage rawImage;

		[SerializeField]
		Volume volume;

		CRTSettings crt;

		
		// Start is called before the first frame update
		void Start()
		{
#if UNITY_WEBGL
			var camera = Camera.main;
			camera.targetTexture = rawImage.texture as RenderTexture;

			if (volume.profile.TryGet(out crt))
				crt.active = false;
#else
			Destroy(gameObject);
#endif
		}

		// Update is called once per frame
		void Update()
		{

		}
	}
}
