
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace TMM
{
	public class JumpscareManager : MonoBehaviour
	{

		

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

			// Prima devo eliminare tutti quelli che non hanno almeno un tile libero in una delle 4 posizioni (tipo quelli interni ai corridoi che si possono attraversare solo n-s e e-o)


			// Set random jumpscares
			int count = Random.Range(1, 6);

			for (int i = 0; i < count; i++)
			{
				jsl.RemoveAt(Random.Range(0, jsl.Count));
			}

			// Destroy all remaining jumpscares
			foreach (var js in jsl)
				Destroy(js.gameObject);
		}

		
		
    }
}
