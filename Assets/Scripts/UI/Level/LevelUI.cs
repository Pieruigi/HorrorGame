using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TMM.UI
{
	public class LevelUI : MonoBehaviour
	{
		[SerializeField]
		List<Toggle> toggles;

		[SerializeField]
		TMP_Text choiceField;

		bool initialized = false;

        private void Awake()
        {
			foreach (Toggle toggle in toggles)
			{
				toggle.onValueChanged.AddListener((v) => { HandleOnValueChanged(toggle, v); });
			}
        }

        // Start is called before the first frame update
        void Start()
	    {
			InitToggles();
			
			
	    }

	    // Update is called once per frame
	    void Update()
	    {
	        
	    }

		void InitToggles()
		{
            if (SteamStatsManager.Instance.GetGameLevel(out var level))
            {
                for (int i = 0; i < toggles.Count; i++)
                {
                    if (i <= level)
					{
                        toggles[i].GetComponent<LevelToggleUI>().Unlock();
						toggles[i].interactable = true;
                    }
                        
                }

            }

			for (int i = 0; i < toggles.Count; i++) 
			{
				if (i == GameManager.Instance.Level)
					toggles[i].isOn = true;
				else 
					toggles[i].isOn = false;
			}

			InitChoiceField();

			initialized = true;
        }

		void HandleOnValueChanged(Toggle toggle, bool isOn)
		{
			if (!isOn) return;

			var index = toggles.IndexOf(toggle);
			GameManager.Instance.Level = index;

            InitChoiceField();
        }

		void InitChoiceField()
		{
			string name = "NORMAL";

			switch (GameManager.Instance.Level)
			{
				case 1:
					name = "HARD";
					break;
				case 2:
					name = "NIGHTMARE";
					break;
			}

			choiceField.text = name;
		}

	}
}
