using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using StarterAssets;
using TMM.UI;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TMM
{
	public class MessageManager : Singleton<MessageManager>
	{
		[SerializeField]
		List<string> messages = new List<string>();

		bool walletUpdated = false;

		bool sprintAndjump = false;


        private void Start()
        {
			
        }

        private void Update()
        {
		
        }

        void OnEnable()
		{
			SceneManager.sceneLoaded += HandleOnSceneLoaded;
			MiniGame.OnMiniGameBeaten += HandleOnMiniGameBeaten;
			Wallet.OnBalanceUpdated += HandleOnWalletUpdated;
			MazeBuilder.OnMazeCreated += HandleOnMazeCreated;
		}

        void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleOnSceneLoaded;
            MiniGame.OnMiniGameBeaten -= HandleOnMiniGameBeaten;
            Wallet.OnBalanceUpdated -= HandleOnWalletUpdated;
            MazeBuilder.OnMazeCreated -= HandleOnMazeCreated;
        }

        private void HandleOnMazeCreated()
        {
            if(GameManager.Instance.GameStage > 1)
			{
				walletUpdated = true;
				sprintAndjump = true;
			}
			else
			{
				StartCoroutine(CheckSprintAndJump());
			}
		}

		IEnumerator CheckSprintAndJump()
		{
			if (sprintAndjump) yield break;

			// Get player 
			FirstPersonController player = FindFirstObjectByType<FirstPersonController>();

			while (!sprintAndjump)
			{
                // Raycast
				var origin = player.transform.position;
				origin.y = 0.5f;
				var direction = player.transform.forward;

				if (Physics.Raycast(origin, direction, out var hit, 2.5f))
				{
					var ft = hit.collider.transform.parent.GetComponent<FloorTrigger>();
					if (ft != null && !ft.Triggered) 
					{
						sprintAndjump = true;
                        if (!IsMessageVisible())
                            MessageUI.Instance.ShowMessage("Sprint (LSHIFT) and jump (SPACE)");
                    }
				}
                

				yield return new WaitForSeconds(.2f);
            }

		}

		private void HandleOnWalletUpdated(int amount)
        {
			if (walletUpdated) return;

			walletUpdated = true;

			StartCoroutine(Do());

			IEnumerator Do()
			{
				yield return new WaitForSeconds(1f);

				if(!IsMessageVisible())
                    MessageUI.Instance.ShowMessage("TAB: show wallet");
            }

				
        }

        private void HandleOnMiniGameBeaten(MiniGame miniGame)
        {
			StartCoroutine(DoShowMessage());

			// Local function 
			IEnumerator DoShowMessage()
			{
				yield return new WaitForSeconds(2f);
                ShowCustomMessage(7, false);
            } 
			
        }

        private void HandleOnSceneLoaded(Scene arg0, LoadSceneMode arg1)
		{
            
			if (arg0.name == "GameScene")
			{
				if (GameManager.Instance.GameStage == 1)
				{
					SendFlashlightMessage();
				}
			}
		}

		void SendFlashlightMessage()
		{
			//return;
			var seq = DOTween.Sequence();
			seq.AppendInterval(2f);
			seq.AppendCallback(() => { MessageUI.Instance.ShowMessage("Find and beat the minigame"); });
            seq.AppendInterval(3f);
            seq.OnComplete(() => { MessageUI.Instance.ShowMessage("F: flashlight on/off"); });
		}

		public void ShowCustomMessage(int messageId, bool keepOn = false)
		{
			Debug.Log("Show custom message:" + messageId);
			if (messageId < 0) return;
			MessageUI.Instance.ShowMessage(messages[messageId], keepOn);
		}

		public void HideMessage()
		{
			MessageUI.Instance.HideMessage();
		}
		
		public bool IsMessageVisible()
		{
			return MessageUI.Instance.Visible;
		}
    }
}
