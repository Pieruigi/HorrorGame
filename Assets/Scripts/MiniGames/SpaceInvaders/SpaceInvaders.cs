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
        List<Spaceship>[] spaceships;

        protected override void Awake()
        {
            base.Awake();

            stepOffset = enemyRoot.localPosition.x;
            Debug.Log("TEST - Test offset:"+stepOffset);
        }

        protected override void Start()
        {
            base.Start();

            spaceships = new List<Spaceship>[enemyRoot.childCount];

            // Init rows
            for (int i = 0; i < enemyRoot.childCount; i++)
            {
                spaceships[i] = new List<Spaceship>();
                Transform row = enemyRoot.GetChild(i);
                for(int j=0; j<row.childCount; j++)
                {
                    spaceships[i].Add(row.GetChild(j).GetComponent<Spaceship>());
                }
                       
                
            }

            // Get the move step as the horizontal distance between two spaceships
            stepDist = Mathf.Abs(spaceships[0][0].transform.localPosition.x - spaceships[0][1].transform.localPosition.x);

            Debug.Log($"TEST - Spaships.Length:{spaceships.Length}");
            foreach (List<Spaceship> spaceshipList in spaceships)
            {
                Debug.Log("TEST - Count:" + spaceshipList.Count);
            }

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
                    Debug.Log("TEST - Left Index:" + leftIndex);
                    Debug.Log("TEST - Right Index:" + rightIndex);

                    float minX = -leftIndex * stepDist + stepOffset;
                    float maxX = (steps - 1 - rightIndex) * stepDist + stepOffset;

                    var currentX = enemyRoot.transform.localPosition.x;
                    if(stepDir > 0)
                    {
                        if (maxX - currentX > 0.001f)
                        {
                            
                            // Move right
                            currentStep++;
                            var d = currentStep * stepDist + stepOffset;
                            Debug.Log("TEST - Moving - D:"+d);
                            enemyRoot.DOLocalMoveX(d, .2f).SetEase(Ease.OutQuint).OnComplete(() => 
                            {
                                var pos = enemyRoot.transform.localPosition;
                                pos.x = d;
                                enemyRoot.transform.localPosition = pos;

                               
                                if (maxX - d < 0.001f)
                                    stepDir = -1;
                            });
                        }
                    }
                    else
                    {
                        
                    }

                        Debug.Log("TEST - MinX:" + minX);
                    Debug.Log("TEST - MaxX:" + maxX);

                    yield return new WaitForSeconds(2f);
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
            
            for (int i = 0; i < spaceships.Length; i++)
            {
                for(int j=0;j<spaceships[i].Count;j++)
                {
                    if (!spaceships[i][j].Destroyed)
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

            for (int i = 0; i < spaceships.Length; i++)
            {
                for (int j = 0; j < spaceships[i].Count; j++)
                {
                    if (!spaceships[i][spaceships[i].Count - 1 - j].Destroyed)
                    {
                        if (index < 0 || spaceships[i].Count - 1 - j > index)
                            index = spaceships[i].Count - 1 - j;

                        break;
                    }
                }
            }

            return index;
        }
	}
}
