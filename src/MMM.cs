using System;
using System.IO;
using System.Reflection;
using System.Xml;
using HarmonyLib;
using UnityEngine;

public class MajorasMaskMoon : IModApi
{
    public static string modsFolderPath;
    private static bool isInstantiated = false;
    GameObject moonManagerObject;
    public static int bloodMoonInterval = 7;
    bool scaleMoon = true;

    public void InitMod(Mod _modInstance)
    {
        Log.Out(" Loading Patch: " + base.GetType().ToString());
        modsFolderPath = _modInstance.Path;
        ReadXML();
        //ModEvents.GameStartDone.RegisterHandler(GameStart);
        if (scaleMoon)
        {
            if (!isInstantiated)
            {
                Log.Out("[Majoras Mask Moon] Instantiating MonoBehaviour");
                moonManagerObject = new GameObject("MajorasMaskMoonManager");
                moonManagerObject.AddComponent<MonoMMM>();
                isInstantiated = true;
            }
        }
        ModEvents.WorldShuttingDown.RegisterHandler(WorldShuttingDown);
        Harmony harmony = new Harmony(base.GetType().ToString());
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }

    public void WorldShuttingDown()
    {
        if (isInstantiated)
        {
            Log.Out("[Majoras Mask Moon] Deleting MonoBehaviour");
            UnityEngine.Object.Destroy(moonManagerObject);
            isInstantiated = false;
        }
    }

    void ReadXML()
    {
        using (XmlReader xmlReader = XmlReader.Create(modsFolderPath + "\\settings.xml"))
        {
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    if (xmlReader.Name.ToString() == "scaleMoon")
                    {
                        string temp = xmlReader.ReadElementContentAsString().ToLower();
                        if (!bool.TryParse(temp, out scaleMoon))
                        {
                            Log.Warning($"[Majoras Mask Moon] Failed to read settings.xml setting scaleMoon. Using default of {scaleMoon}");
                            Log.Warning($"[Majoras Mask Moon] Failed settings.xml read should be reviewed and mod reinstalled if needed");
                        }
                    }
                    if (scaleMoon)
                    {
                        if (xmlReader.Name.ToString() == "bloodMoonInterval")
                        {
                            string temp = xmlReader.ReadElementContentAsString();
                            if (!int.TryParse(temp, out bloodMoonInterval))
                            {
                                Log.Warning($"[Majoras Mask Moon] Failed to read settings.xml setting bloodMoonInterval. Using default of {bloodMoonInterval}");
                                Log.Warning($"[Majoras Mask Moon] Failed settings.xml read should be reviewed and mod reinstalled if needed");
                            }
                        }
                    }
                }
            }
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
        string ambientMoonTexture = "#@modfolder(MajorasMaskMoon):Resources/mm_moon.unity3d?mm_moon_1024.png";
        Texture moonTexture = DataLoader.LoadAsset<Texture>(ambientMoonTexture);
        MeshRenderer renderer;

        if (moonTransform.TryGetComponent<MeshRenderer>(out renderer))
        {
            if (renderer.material != null && moonTexture != null)
            {
                // Set moon texture
                Log.Out("[Majoras Mask Moon] Replacing Moon Texture");
                renderer.material.SetTexture(propColorMap, moonTexture);
            }
        }
    }
}

/*
2025-02-18T16:59:52 58.120 INF [MMM] Found MeshRenderer on: MoonSprite
2025-02-18T16:59:52 58.123 INF [MMM] Property on MoonSprite: _ColorMap
2025-02-18T16:59:52 58.125 INF [MMM] Found MeshRenderer on: AtmosphereSphere
2025-02-18T16:59:52 58.128 INF [MMM] Property on AtmosphereSphere: _OuterSpaceCube
2025-02-18T16:59:52 58.130 INF [MMM] Found MeshRenderer on: CloudsSphere
2025-02-18T16:59:52 58.132 INF [MMM] Property on CloudsSphere: _CloudBGTex
2025-02-18T16:59:52 58.135 INF [MMM] Property on CloudsSphere: _CloudBlendTex
2025-02-18T16:59:52 58.137 INF [MMM] Property on CloudsSphere: _CloudMainTex
2025-02-18T16:59:52 58.140 INF [MMM] Property on CloudsSphere: _DebugTex
*/

/*
[HarmonyPatch(typeof(SkyManager), "Init")]
public class SetSkyTexture_Patch
{
    public static void Postfix()
    {
        int propColorMap = Shader.PropertyToID("_OuterSpaceCube");
        string skySprite = "AtmosphereSphere";
        Transform skyTransform = SkyManager.skyManager.transform.FindInChildren(skySprite);


        string galaxyCubeMap = "#@modfolder(MajorasMaskMoon):Resources/galaxy_cubemap.unity3d?galaxy";
        Cubemap skyTexture = DataLoader.LoadAsset<Cubemap>(galaxyCubeMap);
        //Texture skyTexture = DataLoader.LoadAsset<Texture>(ambientSkyTexture);


        //Texture2D atmosphere2DTexture = DataLoader.LoadAsset<Texture2D>(ambientSkyTexture);
        //Cubemap atmosphereCubemap = CreateCubemapFrom2DTexture(galaxyCubeMap);



        MeshRenderer renderer;

        if (skyTransform.TryGetComponent<MeshRenderer>(out renderer))
        {
            if (renderer.material != null && galaxyCubeMap != null)
            {


                // Set moon texture
                Log.Out("[MMM] Replacing Moon Texture with MMM");
                renderer.material.SetTexture(propColorMap, skyTexture);
            }
        }
    }
}
*/

/*
[HarmonyPatch(typeof(SkyManager), "Init")]
public class SetSkyboxTexture_Patch
{
    public static void Postfix()
    {
        int propColorMap = Shader.PropertyToID("_ColorMap");

        // Example: Load moon texture
        string moonSpriteTexture = "#@modfolder(MajorasMaskMoon):Resources/mm_moon.unity3d?mm_moon_512.png";
        Texture moonTexture = DataLoader.LoadAsset<Texture>(moonSpriteTexture);

        // Loop through all transforms under SkyManager to find any MeshRenderer with a texture
        Transform skyManagerTransform = SkyManager.skyManager.transform;
        foreach (Transform childTransform in skyManagerTransform)
        {
            // Check if the child has a MeshRenderer component
            MeshRenderer renderer = childTransform.GetComponent<MeshRenderer>();
            if (renderer != null && renderer.material != null)
            {
                // Debug log to see all available transforms with MeshRenderer components
                Log.Out("[MMM] Found MeshRenderer on: " + childTransform.name);
                foreach (var property in renderer.material.GetTexturePropertyNames())
                {
                    Log.Out("[MMM] Property on " + childTransform.name + ": " + property);
                }

                // If it's the night sky, replace it too
                if (childTransform.name.Contains("NightSky") || childTransform.name.Contains("Skybox") || childTransform.name.Contains("sky") || childTransform.name.Contains("galaxy"))
                {
                    Log.Out("[MMM] ------------possible replacement ---------.");
                    
                }
            }
        }
    }
}
*/
