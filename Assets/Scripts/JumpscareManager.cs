
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Rendering;

namespace TMM
{
	public class JumpscareManager : Singleton<JumpscareManager>
	{

		AudioSource audioSource;

		protected override void Awake()
		{
			base.Awake();

			audioSource = GetComponent<AudioSource>();
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
			MazeBuilder.OnMazeCreated += HandleOnMazeCrated;
		}

        void OnDisable()
        {
            MazeBuilder.OnMazeCreated -= HandleOnMazeCrated;
        }

		private void HandleOnMazeCrated()
		{
			// Get all jumpscare objects
			List<Jumpscare> jsl = FindObjectsByType<Jumpscare>(FindObjectsSortMode.None).ToList();

			// Remove all tiles with only North-South or East-West connections
			List<Jumpscare> toRemove = new List<Jumpscare>();
			foreach (var js in jsl)
			{
				if(js.Validate())
					toRemove.Add(js);
			}
			// Remove tiles
			foreach(var r in toRemove)
			{
				jsl.Remove(r);
				Destroy(r.gameObject);
			}


			// Set random jumpscares
			int count = Random.Range(0, 2);
			//count = 4;

			for (int i = 0; i < count && jsl.Count > 0; i++)
			{
				jsl.RemoveAt(Random.Range(0, jsl.Count));
			}

			// Destroy all remaining jumpscares
			foreach (var js in jsl)
				Destroy(js.gameObject);
		}

		public void PlayAudio()
		{
			audioSource.Play();
		}
		
    }
}
