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

    // -----------------------------
    // PUBLIC METHODS
    // -----------------------------

    public void PlayLetterboxJumpScare()
    {
        //var fpc = FindFirstObjectByType<FirstPersonController>();
        //fpc.InputDisabled = true;

        PlayShake(
            duration: 1.7f,
            posStrength: 0.25f * .4f,
            rotStrength: 15f * .4f,
            vibratoPos: 30,
            vibratoRot: 20
            // onComplete: () =>
            // {
            //     fpc.InputDisabled = false;
            // }
        );
    }

    /// <summary>
    /// Uno shake leggerissimo per gli spari dell’arma giocosa.
    /// Pensato per frequenza 0.8 colpi/sec → deve essere rapido e subtle.
    /// </summary>
    public void PlayLightShootShake()
    {
        PlayShake(
            duration: 0.12f,            // molto breve
            posStrength: 0.03f,         // piccolissimo kick
            rotStrength: 2.5f,          // leggero recoil visivo
            vibratoPos: 8,
            vibratoRot: 10
        );
    }

    public void PlayJumpscareShake(float duration)
    {
        PlayShake(
            duration: duration,            // molto breve
            posStrength: 0.25f * .4f,
            rotStrength: 15f * .4f,
            vibratoPos: 30,
            vibratoRot: 20
        );
    }

    // -----------------------------
    // CORE SHAKE HANDLER
    // -----------------------------

    private void PlayShake(
        float duration,
        float posStrength,
        float rotStrength,
        int vibratoPos,
        int vibratoRot,
        System.Action onComplete = null)
    {
        // Ferma shake precedenti
        shakeTween?.Kill();
        rotTween?.Kill();

        // SHAKE POSITION
        shakeTween = transform.DOShakePosition(
            duration,
            strength: posStrength,
            vibrato: vibratoPos,
            randomness: 90,
            fadeOut: true
        ).SetUpdate(true);

        // SHAKE ROTATION
        rotTween = transform.DOShakeRotation(
            duration,
            strength: rotStrength,
            vibrato: vibratoRot,
            randomness: 90,
            fadeOut: true
        ).SetUpdate(true);

        rotTween.onComplete += () =>
        {
            transform.localPosition = originalPos;
            transform.localEulerAngles = originalRot;
            onComplete?.Invoke();
        };
    }
}


// using UnityEngine;
// using DG.Tweening;
// using StarterAssets;

// public class CameraShake : MonoBehaviour
// {
//     private Vector3 originalPos;
//     private Vector3 originalRot;
//     private Tween shakeTween;
//     private Tween rotTween;

//     void Awake()
//     {
//         originalPos = transform.localPosition;
//         originalRot = transform.localEulerAngles;
//     }

//     public void PlayLetterboxJumpScare()
// 	{
// 		var fpc = FindFirstObjectByType<FirstPersonController>();
// 		fpc.InputDisabled = true;

//         // Stop any previous shake
//         shakeTween?.Kill();
//         rotTween?.Kill();

//         float duration = 1.7f;
//         float positionStrength = 0.25f * .4f;  // Shake position power
//         float rotationStrength = 15f * .4f;    // Shake rotation power

//         // Shake position
//         shakeTween = transform.DOShakePosition(
//             duration,
//             strength: positionStrength,
//             vibrato: 30,
//             randomness: 90,
//             fadeOut: true
//         ).SetUpdate(true);

//         // Shake rotation
//         rotTween = transform.DOShakeRotation(
//             duration,
//             strength: rotationStrength,
//             vibrato: 20,
//             randomness: 90,
//             fadeOut: true
//         ).SetUpdate(true);

//         // Reset
//         rotTween.onComplete += () =>
//         {
//             transform.localPosition = originalPos;
// 			transform.localEulerAngles = originalRot;
// 			fpc.InputDisabled = false;
//         };
//     }
// }
