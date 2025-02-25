using System.Collections.Generic;
using UnityEngine;

public class MonoMMM : MonoBehaviour
{
    private float timeSinceLastUpdate = 0f; // Timer for update intervals
    private float updateInterval = 5f; // Update interval in seconds

    void Update()
    {
        timeSinceLastUpdate += Time.deltaTime;

        if (timeSinceLastUpdate >= updateInterval)
        {
            //var world = GameManager.Instance.WorldInfo;
            // var a = world.getgam
            //var a = GamePrefs.Instance;
            //var b = a.GetInt(EnumGamePrefs.BloodMoonFrequency);
            //var c = GamePrefs.GetInt(EnumGamePrefs.BloodMoonFrequency);
            //int bloodMoonInterval = GamePrefs.GetInt(EnumGamePrefs.BloodMoonFrequency);
            //int bloodMoonInterval2 = (int)GameInfoInt.BloodMoonFrequency;
            //var e = GameStats.GetInt(EnumGameStats.BloodMoonDay).ToString();
            //var f = ConsoleCmdGetGamePrefs.GetCommands();
            //SortedList<string, string> sortedList = new SortedList<string, string>();
            /*
            foreach (EnumGamePrefs enumGamePrefs in EnumUtils.Values<EnumGamePrefs>())
            {
                if (enumGamePrefs.ToStringCached<EnumGamePrefs>() == "BloodMoonFrequency")
                {
                    Log.Out($"[mmm] ---- found it {GamePrefs.GetObject(enumGamePrefs)}----");
                }
            }*/
                //var d = (int)EnumGamePrefs.GetInt(BloodMoonFrequency);
                //var e = GamePrefs.
                //Log.Out($"MMM -  blood moon internval {bloodMoonInterval}");
            //Log.Out($"MMM -  blood moon internval {bloodMoonInterval2}");
            //Log.Out($"MMM -  blood moon internval {d}");
            timeSinceLastUpdate = 0f; // Reset the timer
            /*
             * This worked, but seems unnesseccary, and if folks are using differnt daylight hours, this would look jank.
            ulong worldTime = GameManager.Instance.World.worldTime;
            if (worldTime >= 16000UL || worldTime <= 10000UL)
            {
                // night time, more frequent update
                updateInterval = 5f;
            }
            else
            {
                updateInterval = 60f;
            }
            */
            UpdateMoonScale();
        }
    }


    void UpdateMoonScale()
    {
        string moonSprite = "MoonSprite";
        ulong worldTime = GameManager.Instance.World.worldTime;
        // 9am, so biggest mooon during horde  night, and then when hidden in the horizon, reset to smallest size
        ulong adjustedWorldTime = worldTime >= 9000UL ? worldTime - 9000UL : worldTime + 16000UL;
        var dayNum = (int)(adjustedWorldTime / 24000UL + 1UL);
        int dayCycle = (dayNum - 1) % MajorasMaskMoon.bloodMoonInterval + 1;
        float progressInDay = (adjustedWorldTime % 24000) / 24000f;

        float minSize = 1.0f;
        float maxSize = 14.0f;
        float lerpProgress = (dayCycle - 1) / (float)MajorasMaskMoon.bloodMoonInterval + (progressInDay / 10.0f);
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