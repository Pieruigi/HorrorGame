using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
	public class HammerClown : MonoBehaviour
	{
        [SerializeField]
        AudioSource hitSource;

        [SerializeField]
        List<AudioClip> hitClips;

		HammerGame miniGame;

	    // Start is called before the first frame update
	    void Start()
	    {

#if UNITY_EDITOR
            miniGame = FindFirstObjectByType<HammerGame>();
#endif
        }

	    // Update is called once per frame
	    void Update()
	    {
	        
	    }

        private void OnEnable()
        {
			MazeBuilder.OnMazeCreated += HandleOnMazeCreated;
        }

        private void OnDisable()
        {
            MazeBuilder.OnMazeCreated -= HandleOnMazeCreated;
        }

        private void HandleOnMazeCreated()
        {
            miniGame = FindFirstObjectByType<HammerGame>(); 
        }

        private void OnCollisionEnter(Collision collision)
        {
			
            miniGame.ReportHit(this);

            // Play sound
            hitSource.clip = hitClips[0];// [Random.Range(0, hitClips.Count)];
            hitSource.Play();
        }

        public void ReportNotHit()
        {
            hitSource.clip = hitClips[1];// [Random.Range(0, hitClips.Count)];
            hitSource.Play();
        }
    }
}
