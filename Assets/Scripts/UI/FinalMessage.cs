using UnityEngine;
using TMPro;
using DG.Tweening; // Required for DOTween
using System.Collections;
using UnityEngine.SceneManagement; // Added in case you want to reload the scene

public class FinalMessage : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI finalText;
    [SerializeField] private CanvasGroup finalCanvasGroup;

    [Header("Impact Settings (SBAAMMM)")]
    [SerializeField] private float punchDuration = 0.2f;
    [SerializeField] private Vector3 punchScale = new Vector3(1.5f, 1.5f, 1.5f);
    [SerializeField] private float punchElasticity = 0.3f;

    [Header("Shake Settings")]
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeStrength = 20f;
    [SerializeField] private int shakeVibrato = 30;

    [Header("Flow Settings")]
    [SerializeField] private float displayDuration = 3.5f;
    [SerializeField] private int sceneToLoadIndex = 0;

    void Awake()
    {
        // Initial setup: hide the message
        if (finalCanvasGroup != null)
        {
            finalCanvasGroup.alpha = 0;
            finalCanvasGroup.gameObject.SetActive(false);
        }
    }

    // Call this method when the lever is pulled
    public void PlayFinalSequence()
    {
        if (finalText == null || finalCanvasGroup == null)
        {
            Debug.LogError("FinalMessage: Missing UI references!");
            return;
        }

        // 1. Activate UI
        finalCanvasGroup.gameObject.SetActive(true);
        finalCanvasGroup.alpha = 1;
        finalText.text = "You'll never be free";

        // Ensure scale is reset before starting the animation
        finalText.transform.localScale = Vector3.one;

        // 2. THE IMPACT (SBAAMMM)
        // PunchScale creates the "hit" effect, growing and bouncing back
        finalText.transform.DOPunchScale(punchScale, punchDuration, 10, punchElasticity);

        // Violent shake for the horror atmosphere
        finalText.transform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato);

        // 3. Handle the loop restart
        StartCoroutine(WaitAndRestart());
    }

    private IEnumerator WaitAndRestart()
    {
        // Keep the text on screen for a while
        yield return new WaitForSeconds(displayDuration);

        // Optional: Fade out effect before reloading
        finalCanvasGroup.DOFade(0, 1.5f);
        //yield return new WaitForSeconds(1.5f);

        // 4. Reload the scene/maze
        //Debug.Log("Restarting the nightmare...");
        //SceneManager.LoadScene(sceneToLoadIndex);
    }
}