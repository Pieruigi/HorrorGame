using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM.UI
{
	public class MenuUI : MonoBehaviour
	{
		[SerializeField]
		List<GameObject> panels;

		[SerializeField]
		bool hideOnStart = false;

		bool isOpen = false;

        private void Awake()
        {
            
        }

        // Start is called before the first frame update
        void Start()
	    {
            
			if(hideOnStart)
				HideAll();
			else
				ShowPanel(panels[0]);
        }

	    // Update is called once per frame
	    void Update()
	    {
			if (hideOnStart)
			{
				if(Input.GetKeyDown(KeyCode.Escape))
				{
					if (isOpen)
					{
                        HideAll();
						GameManager.Instance.UnpauseGame();
                    }
					else
					{
						GameManager.Instance.PauseGame();
                        ShowPanel(panels[0]);
                    }
						
                }
			}
	    }

		public void HideAll()
		{
			foreach(var panel in panels)
			{
				panel.SetActive(false);
            }
			isOpen = false;
        }

		public void ShowPanel(GameObject panel)
		{
			HideAll();
            panel.SetActive(true);
			isOpen = true;
        }
	}
}
