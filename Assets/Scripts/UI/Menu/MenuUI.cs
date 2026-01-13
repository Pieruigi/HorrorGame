using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM.UI
{
	public class MenuUI : MonoBehaviour
	{
		[SerializeField]
		List<GameObject> panels;

        private void Awake()
        {
            
        }

        // Start is called before the first frame update
        void Start()
	    {
            ShowPanel(panels[0]);
        }

	    // Update is called once per frame
	    void Update()
	    {
	        
	    }

		void HideAll()
		{
			foreach(var panel in panels)
			{
				panel.SetActive(false);
            }
        }

		public void ShowPanel(GameObject panel)
		{
			HideAll();
            panel.SetActive(true);
        }
	}
}
