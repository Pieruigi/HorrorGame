using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
	public class InteractionEffect : MonoBehaviour
	{
		int materialId = 0;

		Renderer _renderer;

		Color color;

		

        void Awake()
        {
			_renderer = GetComponent<Renderer>();
			//color = _renderer.material.color;
			
        }

        // Start is called before the first frame update
        void Start()
	    {
	        
	    }

		// Update is called once per frame
		void Update()
		{

		}

		public void EnableInteractionEffect(bool value)
		{
			Debug.Log("TEST - Set interaction effetct :" + value);
			color = _renderer.material.GetColor("_BaseColor");
			if (value)
			    _renderer.material.SetVector("_BaseColor", (Vector4)color*3);
            else
				_renderer.material.SetColor("_BaseColor", color/3f);
		}
		
	}
}
