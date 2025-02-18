using System.Reflection;
using HarmonyLib;
using UnityEngine;

public class MajorasMaskMoon : IModApi
{
    private static bool isInstantiated = false;
    GameObject moonManagerObject;

    public void InitMod(Mod _modInstance)
    {
        Log.Out(" Loading Patch: " + base.GetType().ToString());
        ModEvents.GameStartDone.RegisterHandler(GameStart);
        ModEvents.WorldShuttingDown.RegisterHandler(WorldShuttingDown);
        Harmony harmony = new Harmony(base.GetType().ToString());
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }
    public void GameStart()
    {
        if (!isInstantiated)
        {
            Log.Out("[MMM] Instantiating MonoBehaviour");
            moonManagerObject = new GameObject("MajorasMaskMoonManager");
            moonManagerObject.AddComponent<MonoMMM>();
            isInstantiated = true;
        }
    }
    public void WorldShuttingDown()
    {
        if (isInstantiated)
        {
            Log.Out("[MMM] Deleting MonoBehaviour");
            UnityEngine.Object.Destroy(moonManagerObject);
            isInstantiated = false;
        }
    }
}


[HarmonyPatch(typeof(SkyManager), "Init")]
public class SetMoonTexture_Patch
{
    public static void Postfix()
    {
        int propColorMap = Shader.PropertyToID("_ColorMap");
        string moonSprite = "MoonSprite";
        Transform moonTransform = SkyManager.skyManager.transform.FindInChildren(moonSprite);
        string ambientMoonTexture = "#@modfolder(MajorasMaskMoon):Resources/mm_moon.unity3d?mm_moon_512.png";
        Texture moonTexture = DataLoader.LoadAsset<Texture>(ambientMoonTexture);
        MeshRenderer renderer;

        if (moonTransform.TryGetComponent<MeshRenderer>(out renderer))
        {
            if (renderer.material != null && moonTexture != null)
            {
                // Set moon texture
                Log.Out("[MMM] Replacing Moon Texture with MMM");
                renderer.material.SetTexture(propColorMap, moonTexture);
            }
        }
    }
}

