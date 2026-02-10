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

        int steps = 7;
        int currentStep = 0;

        int moveDir = -1;


        [SerializeField]
        List<Spaceship>[] spaceships;

        protected override void Awake()
        {
            base.Awake();
			
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
