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

		[SerializeField]
		AudioSource beepAudioSource;

		[SerializeField]
		AudioSource errorAudioSource;
		

		MiniGame miniGame;

		TMP_Text textField;

		bool lowFx = false;

		float lastTimeLeft = 0;

		




		void Awake()
        {
			miniGame = GetComponentInParent<MiniGame>();
			textField = GetComponent<TMP_Text>();
			textField.color = normalColor;

			Debug.Log("FLOOR . " + Mathf.Ceil(5.03f));
			Debug.Log("FLOOR . " + Mathf.Ceil(4.96f));
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
				textField.text = Mathf.CeilToInt(timeLeft).ToString("000");

				if (timeLeft < 5)
				{
					if (!lowFx)
					{
						lowFx = true;
						StartLowFx();
					}

					if(Mathf.Ceil(timeLeft) < Mathf.Ceil(lastTimeLeft))
                    {
						beepAudioSource.Play();
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
				textField.text = $"ERR {Mathf.CeilToInt(miniGame.GetCooldownLeft()).ToString("00")}";

				if (lastTimeLeft > 0)
					errorAudioSource.Play();
			}

			lastTimeLeft = timeLeft;
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
