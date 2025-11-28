using System.Collections;
using System.Collections.Generic;
using System.Threading;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace TMM.UI
{
	public class MiniGameTimer : MonoBehaviour
	{
		[SerializeField]
		Color normalColor;

		[SerializeField]
		Color lowColor;

		MiniGame miniGame;

		TMP_Text textField;

		bool lowFx = false;




		void Awake()
        {
			miniGame = GetComponentInParent<MiniGame>();
			textField = GetComponent<TMP_Text>();
			textField.color = normalColor;
        }

        // Start is called before the first frame update
        void Start()
	    {
	        
	    }

		// Update is called once per frame
		void Update()
		{

		}

		void LateUpdate()
		{
			float timeLeft = miniGame.TimeLeft;

			if (timeLeft > 0)
			{
				textField.text = timeLeft.ToString("0");

				if (timeLeft < 4)
				{
					if (!lowFx)
					{
						lowFx = true;
						StartLowFx();
					}
				}
                else
                {
                    if (lowFx)
                    {
						lowFx = false;
						StopLowFx();
                    }
                }
			}
			else
			{
				textField.text = "ERR";
			}
		}

		void StartLowFx()
		{
			textField.DOKill();
			textField.color = lowColor;
			//textField.DOFade()
		}
		
		void StopLowFx()
        {
			textField.DOKill();
			textField.color = normalColor;
        }
    }
}
