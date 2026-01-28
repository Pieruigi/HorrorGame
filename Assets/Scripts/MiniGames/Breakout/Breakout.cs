using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
	public class Breakout : MiniGame
	{
        [SerializeField]
        GameObject ship;

        [SerializeField]
        GameObject ball;

        [SerializeField]
        BreakoutBottom bottom;

        Vector3 shipPositionDefault;

        float shipSpeed = 2;
        
        float shipOffsetMax = 0.58f;

        BreakoutBall boBall;

        int shipDirection = 0;
        public int ShipDirection => shipDirection;

        int brickCount = 0;

        protected override void Awake()
        {
            base.Awake();

            shipPositionDefault = ship.transform.localPosition;
           
            boBall = ball.GetComponent<BreakoutBall>(); 
            //ball.SetActive(false);
        }

        protected override void Update()
        {
            base.Update();

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.X))
                Time.timeScale = 0;

#endif

            if (IsActive)
            {
                if (!Paused)
                {
                    // Move player ship
                    if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                    {
                        shipDirection = Input.GetKey(KeyCode.A) ? -1 : 1;
                        // Move the player ship
                        var shipPos = ship.transform.localPosition;
                        shipPos.x += shipDirection * shipSpeed * Time.deltaTime;

                        if (Mathf.Abs(shipPos.x) > shipOffsetMax)
                            shipPos.x = shipDirection * shipOffsetMax; // Check boundaries

                        ship.transform.localPosition = shipPos; // Set position
                    }
                    else
                    {
                        shipDirection = 0;
                    }
                }
               



            }
        }

        public override void DoChildActivation()
        {
            base.DoChildActivation();

            //ball.SetActive(true);
            StopAllCoroutines();

            StartCoroutine(LaunchBallDelayed(.5f));
            
        }

        IEnumerator LaunchBallDelayed(float delay)
        {
            yield return new WaitForSeconds(delay);
            boBall.Show();
        }

        public override void DoChildDeactivation()
        {
            base.DoChildDeactivation();

            StopAllCoroutines();

            bottom.Reset();

            StartCoroutine(ResetOnExit());
        }

        IEnumerator ResetOnExit()
        {
            yield return new WaitForSeconds(.25f);

            ship.transform.localPosition = shipPositionDefault;
        
            boBall.Hide();

        }

        public void ReportBrickHit(BreakoutBrick brick)
        {
            brickCount--;
            if (brickCount <= 0)
                ReportBeaten();
        }

        public void ReportBrickAdded(BreakoutBrick brick)
        {
            brickCount++;
        }

        public void ReportBallDestroyed()
        {
            StartCoroutine(ResetShipDelayed(.5f));
            
        }

        IEnumerator ResetShipDelayed(float delay)
        {
            yield return new WaitForSeconds(delay);
            ship.transform.localPosition = shipPositionDefault;
        }
	}
}
