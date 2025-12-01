using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using StarterAssets;
using UnityEngine;
using UnityEngine.UI;

namespace TMM.UI
{
	public class StaminaUI : MonoBehaviour
	{
		[SerializeField]
		GameObject panel;

		[SerializeField]
		Image fillImage;

		float maxWidth;
		CanvasGroup canvasGroup;
		FirstPersonController player;

		float lastStamina;

		float elapsed = 0;

		float showTime = .25f;

		bool visible = false;

        void Awake()
        {
			maxWidth = fillImage.rectTransform.rect.width;
			canvasGroup = panel.GetComponent<CanvasGroup>();
			player = FindFirstObjectByType<FirstPersonController>();
			lastStamina = player.Stamina;
			canvasGroup.alpha = 0;
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
          

			if (player.Stamina != lastStamina)
			{
				elapsed = 0;
				if (!visible)
				{
					visible = true;
					canvasGroup.alpha = 1;
				}
			}
            else
			{
                if (visible)
                {
					elapsed += Time.deltaTime;
					if(player.Stamina == player.MaxStamina && elapsed > showTime)
					{
						visible = false;
						canvasGroup.alpha = 0;
					}	    
                }
				
            }

			if (visible)
			{
				var size = fillImage.rectTransform.sizeDelta;
				fillImage.rectTransform.sizeDelta = new Vector2(maxWidth * player.Stamina / player.MaxStamina, size.y);

				
				// if (lastStamina > 0 && player.Stamina == 0)
				// 	canvasGroup.DOFade(0, .1f).SetLoops(-1);
				// else if (lastStamina == 0 && player.Stamina > 0)
                // {
				// 	canvasGroup.DOKill();
				// 	canvasGroup.alpha = 1;
                // }
					
			}
			
			

			lastStamina = player.Stamina;


        }
    }
}
