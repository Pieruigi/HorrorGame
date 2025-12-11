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
			stageField.text = $"Stage {GameManager.Instance.GameStage}";

			var seq = DOTween.Sequence();
			seq.Append(canvasGroup.DOFade(1, .1f));
			seq.AppendInterval(2f);
			seq.Append(canvasGroup.DOFade(0, .1f));
		}
		
		
    }
}
