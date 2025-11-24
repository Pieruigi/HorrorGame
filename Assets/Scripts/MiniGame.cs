using DG.Tweening;
using StarterAssets;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;


namespace TMM
{
	public abstract class MiniGame : MonoBehaviour
	{
		public delegate void MiniGameBeatenDelegate(MiniGame miniGame);
		public static MiniGameBeatenDelegate OnMiniGameBeaten;

		[SerializeField]
		Transform playerTarget;

		float attempts = 10;

		FirstPersonController player;

		//Transform cameraRoot;

		bool activated = false;

		float moveTime = .25f;

		protected abstract void DoUpdate();

		protected virtual void Awake()
        {
            
        }

	    // Start is called before the first frame update
	    protected virtual void Start()
	    {
			player = FindFirstObjectByType<FirstPersonController>();
			//cameraRoot = player.GetComponent<CameraShake>().transform;
	    }

		// Update is called once per frame
		protected void Update()
		{
#if UNITY_EDITOR
			if (Input.GetKeyDown(KeyCode.Z))
			{
				if (!activated)
					Activate();
				else
					Deactivate();
			}
#endif
			if (activated)
				DoUpdate();

		}

		public virtual void Activate()
		{
			if (activated || attempts <= 0) return;

			activated = true;

			// Stop player from moving
			player.InputDisabled = true;

			// Move the controller to the target position
			player.transform.DOMove(playerTarget.position, moveTime);
			player.transform.DORotateQuaternion(playerTarget.rotation, moveTime);
		}

		public virtual void Deactivate()
		{
			if (!activated) return;

			activated = false;
			// Player input enabke
			player.InputDisabled = false;
		}

		protected void DecreaseAttempts()
		{
			attempts--;
			if (attempts < 0) attempts = 0;

			// if (activated)
			// 	Deactivate();
		}

		protected bool CheckAttempts()
		{
			return attempts > 0;
		}
		
	}
}
