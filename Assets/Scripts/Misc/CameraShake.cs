using UnityEngine;
using DG.Tweening;
using StarterAssets;

public class CameraShake : MonoBehaviour
{
    private Vector3 originalPos;
    private Vector3 originalRot;
    private Tween shakeTween;
    private Tween rotTween;

    void Awake()
    {
        originalPos = transform.localPosition;
        originalRot = transform.localEulerAngles;
    }

    public void PlayLetterboxJumpScare()
	{
		var fpc = FindFirstObjectByType<FirstPersonController>();
		fpc.InputDisabled = true;

        // Stop any previous shake
        shakeTween?.Kill();
        rotTween?.Kill();

        float duration = 1.7f;
        float positionStrength = 0.25f * .6f;  // Shake position power
        float rotationStrength = 15f * .6f;    // Shake rotation power

        // Shake position
        shakeTween = transform.DOShakePosition(
            duration,
            strength: positionStrength,
            vibrato: 30,
            randomness: 90,
            fadeOut: true
        ).SetUpdate(true);

        // Shake rotation
        rotTween = transform.DOShakeRotation(
            duration,
            strength: rotationStrength,
            vibrato: 20,
            randomness: 90,
            fadeOut: true
        ).SetUpdate(true);

        // Reset
        rotTween.onComplete += () =>
        {
            transform.localPosition = originalPos;
			transform.localEulerAngles = originalRot;
			fpc.InputDisabled = false;
        };
    }
}
