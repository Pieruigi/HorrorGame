using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace TMM
{
	public class SpaceInvaders : MiniGame
	{
		[SerializeField]
		Transform enemyRoot;

        [SerializeField]
        PlayerSpaceship playerSpaceship;

        float stepDist = 0;

        int steps = 8;
        int currentStep = 0;
        float stepOffset;

        int stepDir = 1;


        [SerializeField]
        List<Spaceship>[] spaceshipRows;

        int jumpscareScore = -1;

        protected override void Awake()
        {
            base.Awake();

            stepOffset = enemyRoot.localPosition.x;
           
        }

        protected override void Start()
        {
            base.Start();

            spaceshipRows = new List<Spaceship>[enemyRoot.childCount];

            // Init rows
            for (int i = 0; i < enemyRoot.childCount; i++)
            {
                spaceshipRows[i] = new List<Spaceship>();
                Transform row = enemyRoot.GetChild(i);
                for(int j=0; j<row.childCount; j++)
                {
                    spaceshipRows[i].Add(row.GetChild(j).GetComponent<Spaceship>());
                }
                       
                
            }

            // Get the move step as the horizontal distance between two spaceships
            stepDist = Mathf.Abs(spaceshipRows[0][0].transform.localPosition.x - spaceshipRows[0][1].transform.localPosition.x);

         

        }

        protected override void Update()
        {
            base.Update();

           
        }

        public override void DoChildActivation()
        {
            base.DoChildActivation();

            playerSpaceship.Activate();

            // Start moving
            StartCoroutine(DoMove());

            IEnumerator DoMove()
            {
                while (true)
                {
                    int leftIndex = GetLeftIndex();
                    int rightIndex = GetRightIndex();
                  
                    float minX = -leftIndex * stepDist + stepOffset;
                    float maxX = (steps - 1 - rightIndex) * stepDist + stepOffset;

                    var currentX = enemyRoot.transform.localPosition.x;
                    if(stepDir > 0)
                    {
                        if (maxX - currentX > 0.001f)
                        {
                            
                            // Move right
                            currentStep++;
                            //var d = currentStep * stepDist + stepOffset;
                            //Debug.Log("TEST - Moving - D:"+d);
                            //enemyRoot.DOLocalMoveX(d, .2f).SetEase(Ease.OutQuint).OnComplete(() => 
                            //{
                            //    var pos = enemyRoot.transform.localPosition;
                            //    pos.x = d;
                            //    enemyRoot.transform.localPosition = pos;

                               
                            //    if (maxX - d < 0.001f)
                            //        stepDir = -1;
                            //});
                        }
                    }
                    else
                    {
                        if(currentX - minX > 0.001f)
                        {
                            // Move left 
                            currentStep--;
                            //var d = currentStep * stepDist + stepOffset;
                            //enemyRoot.DOLocalMoveX(d, .2f).SetEase(Ease.OutQuint).OnComplete(() =>
                            //{
                            //    var pos = enemyRoot.transform.localPosition;
                            //    pos.x = d;
                            //    enemyRoot.transform.localPosition = pos;


                            //    if (d - minX< 0.001f)
                            //        stepDir = 1;
                            //});
                        }
                    }

                    var d = currentStep * stepDist * .5f + stepOffset;
                  
                    enemyRoot.DOLocalMoveX(d, .1f).SetEase(Ease.OutQuint).OnComplete(() =>
                    {
                        var pos = enemyRoot.transform.localPosition;
                        pos.x = d;
                        enemyRoot.transform.localPosition = pos;


                        if (stepDir > 0 && maxX - d < 0.001f)
                            stepDir = -1;
                        else if (stepDir < 0 && d - minX < 0.001f)
                            stepDir = 1;
                    });

              

                    yield return new WaitForSeconds(.5f);
                }
            }
        }

        public override void DoChildDeactivation()
        {
            base.DoChildDeactivation();

            playerSpaceship.Deactivate();

            // Stop moving
            StopAllCoroutines();
        }

        int GetLeftIndex()
        {
            int index = -1;
            
            for (int i = 0; i < spaceshipRows.Length; i++)
            {
                for(int j=0;j<spaceshipRows[i].Count;j++)
                {
                    if (!spaceshipRows[i][j].Destroyed)
                    {
                        if(index < 0 || j < index)
                            index = j;

                        break;
                    }
                }
            }

            return index;
        }

        int GetRightIndex() 
        {
            int index = -1;

            for (int i = 0; i < spaceshipRows.Length; i++)
            {
                for (int j = 0; j < spaceshipRows[i].Count; j++)
                {
                    if (!spaceshipRows[i][spaceshipRows[i].Count - 1 - j].Destroyed)
                    {
                        if (index < 0 || spaceshipRows[i].Count - 1 - j > index)
                            index = spaceshipRows[i].Count - 1 - j;

                        break;
                    }
                }
            }

            return index;
        }

        public void ReportSpaceshipDestroyed()
        {
            // Check if the game has been beaten
            foreach(var spaceshipRow in spaceshipRows)
            {
                foreach (var spaceship in spaceshipRow)
                {
                    if (!spaceship.Destroyed)
                        return;
                }
            }

            if(jumpscareScore > 0)
            {
                jumpscareScore--;
                if (jumpscareScore == 0)
                    MiniJumpscare.Play();
            }

            ReportBeaten();
        }

        public override void InitMiniJumpscare(MiniJumpscare miniJumpscare)
        {

            base.InitMiniJumpscare(miniJumpscare);

            // Play jumpscare at a specific score
            jumpscareScore = Random.Range(6, 16);
        }
    }
}
