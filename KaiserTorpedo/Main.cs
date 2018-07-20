using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Harmony;
using UnityEngine;

namespace KaiserTorpedo
{
    public class Main
    {
        public static GameObject KaiserTorpedo;

        public static void Patch()
        {
            var harmony = HarmonyInstance.Create("com.ahk1221.kaisertorpedo");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            var assetBundle = AssetBundle.LoadFromFile(@"./QMods/KaiserTorpedo/torpedo.assets");
            KaiserTorpedo = assetBundle.LoadAsset<GameObject>("KaiserTorpedoMain");

            var rend = KaiserTorpedo.GetComponentInChildren<Renderer>();
            rend.material.shader = Shader.Find("MarmosetUBER");

            Console.WriteLine("[KaiserTorpedo] Patched succesfully!");
        }
    }

    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch("Update")]
    internal class Player_Update_Patch
    {
        static void Postfix()
        {
            if(Input.GetKeyDown(KeyCode.J))
            {
                var obj = GameObject.Instantiate(Main.KaiserTorpedo);
                obj.transform.position = Player.main.transform.position + (Vector3.forward * 2);
            }
        }
    }

    [HarmonyPatch(typeof(SeamothTorpedo))]
    [HarmonyPatch("Start")]
    internal class SeamothTorpedo_Start_Patch
    {
        static void Postfix(SeamothTorpedo __instance)
        {
            if(__instance.name.Contains("Gas"))
            {
                Console.WriteLine("GasTorpedo spawned! at " + __instance.transform.position);

                var mesh = __instance.gameObject.FindChild("mesh");

                if(mesh != null)
                {
                    GameObject.DestroyImmediate(mesh);
                }

                var newObject = GameObject.Instantiate(Main.KaiserTorpedo, __instance.transform);
            }
        }
    }
}
