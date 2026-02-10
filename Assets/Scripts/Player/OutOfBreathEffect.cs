using DG.Tweening;
using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using TMM.UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TMM
{
	public class OutOfBreathEffect : MonoBehaviour
	{
        [SerializeField]
        AudioSource breathAudioSource;

        [SerializeField]
        List<GameObject> syringes;

        [SerializeField]
        AudioSource slipAudioSource;

        
        Vignette vignette;

        Tween tween;

        float maxIntensity = .75f;

        StaminaUI staminaUI;

        List<Vector3> originalPositions = new List<Vector3>();
        List<Quaternion> originalRotations = new List<Quaternion>();

        private void Awake()
        {
            foreach(var s in syringes)
            {
                originalPositions.Add(s.transform.localPosition);
                originalRotations.Add(s.transform.localRotation);
                s.gameObject.SetActive(false);
            }
        }

        // Start is called before the first frame update
        void Start()
	    {
            FindFirstObjectByType<Volume>().profile.TryGet<Vignette>(out vignette);
            if(GetComponent<OutOfBreathDebuff>().Value)
            {
                vignette.intensity.value = maxIntensity;
            }
            else
            {
                vignette.intensity.value = 0f;
            };

            
        }

	    // Update is called once per frame
	    void Update()
	    {
	        
	    }

        private void OnEnable()
        {
            TimedBuffDebuff.OnApplied += OnBuffApplied;
            TimedBuffDebuff.OnExpired += OnBuffExpired;
            FirstPersonController.OnOutOfBreath += HandleOnOutOfBreath;
            MazeBuilder.OnMazeCreated += HandleOnMazeCreated;
        }

        private void OnDisable()
        {
            TimedBuffDebuff.OnApplied -= OnBuffApplied;
            TimedBuffDebuff.OnExpired -= OnBuffExpired;
            FirstPersonController.OnOutOfBreath -= HandleOnOutOfBreath;
            MazeBuilder.OnMazeCreated -= HandleOnMazeCreated;
        }

        private void HandleOnMazeCreated()
        {
            staminaUI = FindFirstObjectByType<StaminaUI>();
        }

        private void HandleOnOutOfBreath()
        {
            staminaUI.Shake();
        }

        private void OnBuffApplied(TimedBuffDebuff arg0)
        {
            if(arg0.GetType() != typeof(OutOfBreathDebuff)) return;

            // Apply effect
            tween?.Kill();
            tween = DOTween.To(() => vignette.intensity.value, y => vignette.intensity.value = y, maxIntensity, 0.5f).OnComplete(() => 
            {
                tween = DOTween.To(() => vignette.intensity.value, y => vignette.intensity.value = y, maxIntensity*.5f, 1.272f*.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
            });

            // Play sound
            breathAudioSource.Play();

            // Activate syringes
            StartCoroutine(ActivateSyringes());
        }

        private void OnBuffExpired(TimedBuffDebuff arg0)
        {
            if (arg0.GetType() != typeof(OutOfBreathDebuff)) return;

            // Remove effect
            tween?.Kill();
            DOTween.To(() => vignette.intensity.value, y => vignette.intensity.value = y, 0f, 0.5f);

            // Stop sound
            breathAudioSource.Stop();
        }

        IEnumerator ActivateSyringes()
        { 
            foreach(var s in syringes)
            {
                s.transform.parent = Camera.main.transform;
                s.transform.localPosition = originalPositions[syringes.IndexOf(s)] + Vector3.up * .5f;
                s.transform.localRotation = originalRotations[syringes.IndexOf(s)];

                s.transform.DOLocalMove(originalPositions[syringes.IndexOf(s)], .25f).SetEase(Ease.OutBack).OnComplete(() => 
                {
                    
                });
                s.transform.DOShakeRotation(.5f, new Vector3(15f, 15f, 15f), 10, 90f).OnComplete(() => 
                {
                    s.transform.DOLocalMove(originalPositions[syringes.IndexOf(s)] - Vector3.up*.5f, .5f).SetEase(Ease.InSine);
                });

                s.SetActive(true);

            }

            slipAudioSource.Play();

            yield return new WaitForSeconds(1.1f);


            foreach (var s in syringes)
            {

                s.SetActive(false);
            }
        }
    }
}
