using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class Bucket : MonoBehaviour
{
    [SerializeField] private float multiplier = 1.0f;
    [SerializeField] private ParticleSystem particlePrefab;
    private float dropDistance = 15f;
    private float dropDuration = 0.15f;
    private float returnDuration = 0.25f;
    private float shakeAngle = 20f;
    private int shakeCount = 3;
    public float getMultiplier() => multiplier;
    private ParticleSystem particleInstance;

    public void OnBallEntered()
    {
        StartCoroutine(ShakeBounceAnimation());
        SpawnParticles();
        // Send the bucket's multiplier to the plinkoManager when a ball enters
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

    private void SpawnParticles()
    {
        particleInstance = Instantiate(particlePrefab, transform.position, Quaternion.identity);
        var main = particleInstance.main;
        main.startColor = GetMultiplierColor();
        particleInstance.Play();
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

    private IEnumerator ShakeBounceAnimation()
    {
        Vector3 originalPos = transform.localPosition;
        Quaternion originalRot = transform.localRotation;
        
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
