using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
	public class BreakoutBottom : MonoBehaviour
	{
		Breakout minigame;

        private void Awake()
        {
            minigame = transform.root.GetComponentInChildren<Breakout>();
        }

        // Start is called before the first frame update
        void Start()
	    {
	        
	    }

	    // Update is called once per frame
	    void Update()
	    {
	        
	    }

        private void OnTriggerEnter(Collider other)
        {
            BreakoutBall ball = other.GetComponent<BreakoutBall>();

			if (ball == null) return;

			StartCoroutine(ResetBallDelayed(ball, .5f));
        }

		IEnumerator ResetBallDelayed(BreakoutBall ball, float delay)
		{
			minigame.ReportBallDestroyed();

			// Hide ball
			ball.DestroyBall();
			

			yield return new WaitForSeconds(delay);

			// Reset ball
			ball.Show();
		}

		
		public void Reset()
		{
			StopAllCoroutines();
		}
    }
}
