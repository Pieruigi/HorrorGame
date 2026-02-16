using UnityEngine;

public class MenuCameraSway : MonoBehaviour
{
    [Header("Sway Settings")]
    [Tooltip("Maximum rotation angle in degrees.")]
    [SerializeField] private float rotationRange = 5f;

    [Tooltip("How fast the camera oscillates.")]
    [SerializeField] private float speed = 0.5f;

    private Quaternion initialRotation;

    void Start()
    {
        // Store the starting rotation to oscillate around it
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        // Calculate the Y-axis offset using Sine wave (returns a value between -1 and 1)
        float swayY = Mathf.Sin(Time.time * speed) * rotationRange;
        
        // Apply the rotation relative to the initial starting pose
        // We multiply the quaternions to combine the rotations correctly
        transform.localRotation = initialRotation * Quaternion.Euler(0, swayY, 0);
    }
}