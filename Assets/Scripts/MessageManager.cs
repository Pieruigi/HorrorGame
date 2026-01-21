using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMM.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TMM
{
	public class MessageManager : Singleton<MessageManager>
	{
		[SerializeField]
		List<string> messages = new List<string>();


		void OnEnable()
		{
			SceneManager.sceneLoaded += HandleOnSceneLoaded;
		}

        void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleOnSceneLoaded;
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
			var seq = DOTween.Sequence();
			seq.AppendInterval(2f);
			seq.OnComplete(() => { MessageUI.Instance.ShowMessage("F: flashlight on/off\nTAB: show wallet"); });
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
