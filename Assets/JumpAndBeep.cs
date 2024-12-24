using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveAndBeepController : MonoBehaviour
{
    public AudioSource audioSource; // The beep sound
    public float waveAmount = 10f; // Degrees to rotate
    public float waveSpeed = 5f; // Speed of the wave

    private bool isWaving = false;

    IEnumerator PerformWave()
    {
        isWaving = true;

        if (audioSource != null)
        {
            audioSource.Play(); // Play the beep sound
        }

        float elapsedTime = 0f;
        float duration = 1f; // Total wave duration
        while (elapsedTime < duration)
        {
            float angle = Mathf.Sin(elapsedTime * waveSpeed) * (waveAmount / 2); // Smaller wave
            transform.localRotation = Quaternion.Euler(0, angle, 0); // Apply the rotation
            elapsedTime += Time.deltaTime; // Increment elapsed time
            yield return null; // Wait for the next frame
        }

        transform.localRotation = Quaternion.identity; // Reset rotation to original position
        isWaving = false; // Reset waving state
    }
}
