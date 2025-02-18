using UnityEngine;

public class MonoMMM : MonoBehaviour
{
    private float timeSinceLastUpdate = 0f; // Timer for update intervals
    private const float updateInterval = 5f; // Update interval in seconds

    void Update()
    {
        timeSinceLastUpdate += Time.deltaTime;

        if (timeSinceLastUpdate >= updateInterval)
        {
            timeSinceLastUpdate = 0f; // Reset the timer
            UpdateMoonScale();
        }
    }

    void UpdateMoonScale()
    {
        ulong worldTime = GameManager.Instance.World.worldTime;
        string moonSprite = "MoonSprite";

        // 9am, so biggest mooon during horde  night, and then when hidden in the horizon, reset to smallest size
        ulong adjustedWorldTime = worldTime >= 9000UL ? worldTime - 9000UL : worldTime + 16000UL;

        var dayNum = (int)(adjustedWorldTime / 24000UL + 1UL);
        int dayCycle = (dayNum - 1) % 7 + 1;
        float progressInDay = (adjustedWorldTime % 24000) / 24000f;

        float minSize = 1.0f;
        float maxSize = 14.0f;
        float lerpProgress = (dayCycle - 1) / 7.0f + (progressInDay / 10.0f);
        float newScale = Mathf.Lerp(minSize, maxSize, lerpProgress);

        // Apply new scale to the moon
        Transform moonTransform = SkyManager.skyManager.transform.FindInChildren(moonSprite);
        if (moonTransform != null)
        {
            Transform moonParent = moonTransform.parent;
            moonParent.localScale = new Vector3(newScale, newScale, newScale);
        }
    }
}