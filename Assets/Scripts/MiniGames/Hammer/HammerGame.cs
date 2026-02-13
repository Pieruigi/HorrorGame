using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace TMM
{
	public class HammerGame : MiniGame
	{
        [SerializeField]
        List<Transform> spawnPoints;

        [SerializeField]
        List<GameObject> clowns;
        
        int jumpscareScore = -1;

        float showTime = .5f;

        float hiddenTime = 1f;

        int scoreCount = 50;

    
        protected override void Update()
        {
            base.Update();

            if (IsActive)
            {

            }
        }

        public void ReportHit()
        {
            scoreCount--;

            if(jumpscareScore > 0)
            {
                jumpscareScore--;
                if (jumpscareScore == 0)
                    MiniJumpscare.Play();
            }

            if(scoreCount == 0)
                ReportBeaten();
        }

        GameObject GetRandomClown()
        {
            return clowns[Random.Range(0, clowns.Count)];
        }

        Transform GetRandomSpawnPoint(Transform exclude)
        {
            var availables = spawnPoints.Where(s=>s != exclude).ToList();

            return availables[Random.Range(0, availables.Count)];
        }

        public override void InitMiniJumpscare(MiniJumpscare miniJumpscare)
        {
            Debug.Log("TEST - Minijumpscare initialization");

            base.InitMiniJumpscare(miniJumpscare);

            // Play jumpscare at a specific score
            jumpscareScore = Random.Range(3, scoreCount-5);
        }

        public override void DoChildActivation()
        {
            base.DoChildActivation();

            StartCoroutine(DoShowClowns());

            IEnumerator DoShowClowns()
            {
                yield return new WaitForSeconds(1f);

                Transform lastSpawnPoint = null;

                while(scoreCount > 0)
                {
                    GameObject clown = GetRandomClown();
                    lastSpawnPoint = GetRandomSpawnPoint(lastSpawnPoint);

                    Debug.Log("TEST - Show clown:" + clown.name);
                    Debug.Log("TEST - Spawn point:" + lastSpawnPoint.parent.name);
                    // Move clown in position
                    clown.transform.position = lastSpawnPoint.position;

                    // Move the clown up
                    clown.transform.GetChild(0).DOLocalMoveY(-0.72f, .2f).SetEase(Ease.OutBack);

                    yield return new WaitForSeconds(showTime);

                    // Move clown down
                    clown.transform.GetChild(0).DOLocalMoveY(-1f, .2f).SetEase(Ease.OutBack);
                    Debug.Log("TEST - Hide clown:" + clown.name);

                    yield return new WaitForSeconds(hiddenTime);
                }
            }
        }

        public override void DoChildDeactivation()
        {
            base.DoChildDeactivation();

            StopAllCoroutines();
        }

        
    }
}
