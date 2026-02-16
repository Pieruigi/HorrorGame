using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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

        [SerializeField]
        Hammer hammer;

        [SerializeField]
        TMP_Text counterUI;
        
        int jumpscareScore = -1;

        float showTime = .5f;

        float hiddenTime = 1f;

        int scoreCount = 20;

        bool isClownVisible = false;

        protected override void Awake()
        {
            base.Awake();

            counterUI.text = scoreCount.ToString("00");
        }

        protected override void Update()
        {
            base.Update();

            if (IsActive)
            {

            }
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
            base.InitMiniJumpscare(miniJumpscare);

            // Play jumpscare at a specific score
            jumpscareScore = Random.Range(3, scoreCount-5);
        }

        public override void DoChildActivation()
        {
            base.DoChildActivation();

            // Activate hammer
            hammer.Activate();

            StartCoroutine(DoShowClowns());

            IEnumerator DoShowClowns()
            {
                isClownVisible = false;
                yield return new WaitForSeconds(1f);

                Transform lastSpawnPoint = null;

                while(scoreCount > 0)
                {
                    GameObject clown = GetRandomClown();
                    lastSpawnPoint = GetRandomSpawnPoint(lastSpawnPoint);

                    // Move clown in position
                    clown.transform.position = lastSpawnPoint.position;

                    // Move the clown up
                    isClownVisible = true;
                    clown.transform.GetChild(0).DOLocalMoveY(-0.72f, .2f).SetEase(Ease.OutBack);

                    yield return new WaitForSeconds(showTime);

                    // Move clown down
                    if (isClownVisible)
                    {
                        
                        clown.transform.GetChild(0).DOLocalMoveY(-1f, .2f).SetEase(Ease.OutBack).OnComplete(() => 
                        {
                            if (isClownVisible)
                            {
                                isClownVisible = false;
                                clown.GetComponentInChildren<HammerClown>().ReportNotHit();
                            }
                            
                        });
                    }
                    

                    yield return new WaitForSeconds(Random.Range(hiddenTime*.8f, hiddenTime*1.2f));
                }
            }
        }

        public override void DoChildDeactivation()
        {
            base.DoChildDeactivation();

            isClownVisible = false;

            foreach (var clown in clowns)
            {
                var m = clown.transform.GetChild(0);
                var pos = m.localPosition;
                pos.y = -1f;
                m.localPosition = pos;
            }

            // Deactivate hammer
            hammer.Deactivate();

            StopAllCoroutines();
        }

        public void ReportHit(HammerClown clown)
        {
         
            if (!isClownVisible) return;

            isClownVisible = false;

            clown.transform.DOKill();

            clown.transform.DOLocalMoveY(-1f, .05f);

            scoreCount--;

            counterUI.text = scoreCount.ToString("00");

            if (jumpscareScore > 0)
            {
                jumpscareScore--;
                if (jumpscareScore == 0)
                    MiniJumpscare.Play();
            }

            if (scoreCount == 0)
                ReportBeaten();
        }

        
    }
}
