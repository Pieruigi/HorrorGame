using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
	public class BreakoutBottom : MonoBehaviour
	{

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

			Debug.Log("TEST - Enter :" + ball);

			if (ball == null) return;

			StartCoroutine(ResetBallDelayed(ball, 1f));
        }

		IEnumerator ResetBallDelayed(BreakoutBall ball, float delay)
		{
			// Hide ball
			ball.Hide();
			// Play fx
			PlayFx();

			yield return new WaitForSeconds(delay);

			// Reset ball
			ball.Show();
		}

		void PlayFx()
		{

		}
    }
}
