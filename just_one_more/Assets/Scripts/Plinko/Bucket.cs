using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// Plinko bucket that registers ball scores with multiplier value and plays animation
public class Bucket : MonoBehaviour
{
    [SerializeField] private float multiplier = 1.0f;
    [SerializeField] private ParticleSystem particlePrefab;
    [SerializeField] private AudioClip particleSound;
    private float dropDistance = 15f;
    private float dropDuration = 0.15f;
    private float returnDuration = 0.25f;
    private float shakeAngle = 20f;
    private int shakeCount = 3;
    public float getMultiplier() => multiplier;
    private ParticleSystem particleInstance;

    // Called when ball enters this bucket - triggers animation, particles, and score registration
    public void OnBallEntered()
    {
        StartCoroutine(ShakeBounceAnimation());
        SpawnParticles();
        PlinkoManager plinkoManager = FindAnyObjectByType<PlinkoManager>();
        if (plinkoManager != null)
        {
            plinkoManager.OnBallLanded(multiplier);
        }
        else
        {
            Debug.LogError("plinkoManager not found in scene!");
        }
    }

    // Spawn colored particle effect at bucket position with sound
    private void SpawnParticles()
    {
        particleInstance = Instantiate(particlePrefab, transform.position, Quaternion.identity);
        var main = particleInstance.main;
        main.startColor = GetMultiplierColor();
        particleInstance.Play();
        float randPitch = Random.Range(0.5f, 1.5f);
        SoundController.Instance.PlaySound(particleSound, 0.5f, randPitch);
    }

    private Color GetMultiplierColor()
    {
        Image image = GetComponent<Image>();
        if (image != null)
        {
            return image.color;
        }

        return Color.white;
    }

    // Drop bucket down with shake, then bounce back up - two-phase animation
    private IEnumerator ShakeBounceAnimation()
    {
        Vector3 originalPos = transform.localPosition;
        Quaternion originalRot = transform.localRotation;
        
        // Phase 1: Drop down with shake
        float elapsed = 0f;
        while (elapsed < dropDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / dropDuration;
            
            float eased = 1f - Mathf.Pow(1f - t, 2f);

            Vector3 dropPos = originalPos - Vector3.up * (dropDistance * eased);
            transform.localPosition = dropPos;

            float shakeProgress = t * shakeCount;
            float currentShakeAngle = Mathf.Sin(shakeProgress * Mathf.PI * 2f) * shakeAngle * (1f - t);
            
            transform.localRotation = originalRot * Quaternion.Euler(0, 0, currentShakeAngle);
            
            yield return null;
        }
        
        // Phase 2: Return to original position
        elapsed = 0f;
        while (elapsed < returnDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / returnDuration;
            
            float posEased = Mathf.Sin(t * Mathf.PI * 0.5f);
            Vector3 returnPos = Vector3.Lerp(
                originalPos - Vector3.up * dropDistance,
                originalPos,
                posEased
            );
            transform.localPosition = returnPos;
            
            float shakeProgress = t * shakeCount;
            float currentShakeAngle = Mathf.Sin(shakeProgress * Mathf.PI * 2f) * shakeAngle * (1f - t);
            
            transform.localRotation = originalRot * Quaternion.Euler(0, 0, currentShakeAngle);
            
            yield return null;
        }
        
        transform.localPosition = originalPos;
        transform.localRotation = originalRot;
    }
}
