using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM.UI
{
	public class DotCanvas : Singleton<DotCanvas>
	{
		[SerializeField]
		CanvasGroup canvasGroup;

        protected override void Awake()
		{
			base.Awake();
			canvasGroup.alpha = 0;

			// Set camera
			GetComponent<Canvas>().worldCamera = Camera.main;
        }

        // Start is called before the first frame update
        void Start()
		{
			
	    }

		// Update is called once per frame
		void Update()
		{

		}

		public void Show()
		{
			canvasGroup.alpha = 1;
		}
		
		public void Hide()
        {
			canvasGroup.alpha = 0;
        }
	}
}
