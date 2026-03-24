using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TMM
{
	public class StageUI : MonoBehaviour
	{
		[SerializeField]
		CanvasGroup canvasGroup;

		[SerializeField]
		TMP_Text stageField;

		[SerializeField]
		TMP_Text modeField;

        void Awake()
        {
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
			if ("GameScene".Equals(arg0.name))
			{
				Open();
			}
		}

		void Open()
		{
			string mode = "Normal";
			switch (GameManager.Instance.Level)
			{
				case 1:
					mode = "Hard";
					break;
				case 2:
					mode = "Nightmare";
					break;
			}
			stageField.text = $"Stage {GameManager.Instance.GameStage}";

			modeField.text = mode;

			var seq = DOTween.Sequence();
			seq.Append(canvasGroup.DOFade(1, .1f));
			seq.AppendInterval(2f);
			seq.Append(canvasGroup.DOFade(0, .1f));
		}
		
		
    }
}
