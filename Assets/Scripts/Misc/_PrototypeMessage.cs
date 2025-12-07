using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
	public class _PrototypeMessage : Singleton<_PrototypeMessage>
	{
		[SerializeField]
		GameObject panel;

        protected override void Awake()
        {
			base.Awake();

			panel.SetActive(false);
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
			panel.SetActive(true);
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}
		
		public void Close()
        {
			panel.SetActive(false);
			Time.timeScale = 1;
        }
	}
}
