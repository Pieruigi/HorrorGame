using System.Collections;
using System.Collections.Generic;
using System.Threading;
using DG.Tweening;
using TMM.Interfaces;
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

		[SerializeField]
		AudioSource alarmAudioSource;

		MiniGame miniGame;

		TMP_Text textField;

		bool lowFx = false;

		float lastTimeLeft = 0;


		float noiseRangeOnError = 40;

		float noiseRange = 0;



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
				// Check and reset alarm if needed
				if(noiseRange > 0)
                {
					noiseRange = 0;
					//alarmAudioSource.Stop();
					AlarmManager.Instance.ReportTriggerDeactivated(miniGame.gameObject);
                }

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
                {
					errorAudioSource.Play();
					StartCoroutine(MakeNoise());
                }
					
			}

			lastTimeLeft = timeLeft;
		}

		IEnumerator MakeNoise()
		{
			yield return new WaitForSeconds(1f);
			if(miniGame.TimeLeft <= 0)
            {
				Debug.Log("Timer noise");
				noiseRange = noiseRangeOnError;
				//alarmAudioSource.Play();
				AlarmManager.Instance.ReportTriggerActivated(miniGame.gameObject);
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
