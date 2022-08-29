using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using BepInEx;
using System.IO;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Reflection.Emit;
using Newtonsoft.Json;
using TMPro;
using XUnity.ResourceRedirector;
using UnityEditor;



namespace 天地归虚ENMod
{


    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class Main : BaseUnityPlugin
    {
        public static BepInEx.Logging.ManualLogSource log;
        public static List<string> results = new List<String>();
        public const string pluginGuid = "Cadenza.ENMOD.0.5";
        public const string pluginName = "ENMod";
        public const string pluginVersion = "0.5";
        public static Dictionary<string, string> translationDict;
        public static List<string> MissingLines = new List<string>();
        public static Dictionary<string, string> FileToDictionary(string dir)
        {
            Debug.Log(BepInEx.Paths.PluginPath);

            Dictionary<string, string> dict = new Dictionary<string, string>();

            IEnumerable<string> lines = File.ReadLines(Path.Combine(BepInEx.Paths.PluginPath, "Translations", dir));

            foreach (string line in lines)
            {

                var arr = line.Split('¤');
                if (arr[0] != arr[1])
                {
                    var pair = new KeyValuePair<string, string>(Regex.Replace(arr[0], @"\t|\n|\r", ""), arr[1]);

                    if (!dict.ContainsKey(pair.Key))
                        dict.Add(pair.Key, pair.Value);
                    else
                        Debug.Log($"Found a duplicated line while parsing {dir}: {pair.Key}");
                }
            }

            return dict;

        }


        private void Awake()
        {
            translationDict = FileToDictionary("KV.txt");
            var harmony = new Harmony("Cadenza.ENMOD.0.5");
            harmony.PatchAll();
        }


        

                private void Update()
        {
            Type[] ts = Assembly.GetAssembly(typeof(WholeObjects)).GetTypes();
            Type[] ts2 = Assembly.GetAssembly(typeof(TextMeshProUGUI)).GetTypes();



            if (Input.GetKeyUp(KeyCode.F9) == true)
            {
                List<GameObject> gameObjects = Resources.FindObjectsOfTypeAll<GameObject>().ToList();
                foreach (GameObject go in gameObjects)
                {
                    TextMeshProUGUI[] tmp = go.GetComponentsInChildren<TextMeshProUGUI>();
                    UnityEngine.UI.Text[] tmp2 = go.GetComponentsInChildren<UnityEngine.UI.Text>();
                    tmp.AddRangeToArray<TextMeshProUGUI>(go.GetComponents<TextMeshProUGUI>());
                    tmp.AddRangeToArray<TextMeshProUGUI>(go.GetComponentsInParent<TextMeshProUGUI>());
                    tmp2.AddRangeToArray<UnityEngine.UI.Text>(go.GetComponents<UnityEngine.UI.Text>());
                    tmp2.AddRangeToArray<UnityEngine.UI.Text>(go.GetComponentsInParent<UnityEngine.UI.Text>());

                    foreach (TextMeshProUGUI t in tmp)
                    {
                        if (!results.Contains(t.text.Replace("\n", "")))
                        {
                            results.Add(t.text.Replace("\n", ""));
                        }
                    }
                    foreach (UnityEngine.UI.Text t2 in tmp2)
                    {
                        {
                            if (!results.Contains(t2.text.Replace("\n", "")))
                            {
                                results.Add(t2.text.Replace("\n", ""));
                            }
                        }

                    }
                    UnityEngine.Object[] tmp3 = go.GetComponentsInChildren<UnityEngine.Object>();

                }
                foreach (Type t in ts)
                {
                    if (typeof(UnityEngine.Object).IsAssignableFrom(t))
                    {
                        var x0 = Resources.FindObjectsOfTypeAll(t).ToList();

                        foreach (var obj in x0)
                        {
                            try
                            {
                                FieldInfo[] fi = obj.GetType().GetFields();
                                foreach (FieldInfo f in fi)
                                {
                                    if (f.GetUnderlyingType() == typeof(string))
                                    {
                                        Debug.Log("Value = " + f.GetValue(obj).ToString().Replace("\n", ""));
                                        if (!results.Contains(f.GetValue(obj).ToString().Replace("\n", "")))
                                        {
                                            results.Add(f.GetValue(obj).ToString().Replace("\n", ""));

                                        }
                                    }
                                    else

                                    {
                                        if (typeof(System.Collections.IEnumerable).IsAssignableFrom(f.GetUnderlyingType()))
                                        {
                                            foreach (var x in f.GetValue(obj) as IEnumerable<object>)
                                            {
                                                Debug.Log(x.GetType());
                                                FieldInfo[] sufi = x.GetType().GetFields();
                                                foreach (FieldInfo suf in sufi)
                                                {
                                                    if (suf.GetUnderlyingType() == typeof(string))
                                                    {
                                                        Debug.Log("Value 2 = " + suf.GetValue(x).ToString().Replace("\n", ""));
                                                        if (!results.Contains(suf.GetValue(x).ToString().Replace("\n", "")))
                                                        {
                                                            results.Add(suf.GetValue(x).ToString().Replace("\n", ""));
                                                        }
                                                    }
                                                    if (suf.GetUnderlyingType() != typeof(string) && typeof(System.Collections.IEnumerable).IsAssignableFrom(suf.GetUnderlyingType()))
                                                    {
                                                        foreach (var y in suf.GetValue(x) as IEnumerable<object>)
                                                        {
                                                            FieldInfo[] susufi = y.GetType().GetFields();
                                                            foreach (FieldInfo susuf in susufi)
                                                            {
                                                                if (susuf.GetUnderlyingType() == typeof(string))
                                                                {
                                                                    Debug.Log("Value 3 = " + susuf.GetValue(y));
                                                                    if (!results.Contains(susuf.GetValue(y).ToString()))
                                                                    {
                                                                        results.Add(susuf.GetValue(y).ToString());
                                                                    }
                                                                }
                                                                if (susuf.GetUnderlyingType() != typeof(string) && typeof(System.Collections.IEnumerable).IsAssignableFrom(susuf.GetUnderlyingType()))
                                                                {
                                                                    Debug.Log("Nested Susuf = " + susuf.GetUnderlyingType());
                                                                    foreach (var z in susuf.GetValue(y) as IEnumerable<object>)
                                                                    {
                                                                        FieldInfo[] sususufi = z.GetType().GetFields();
                                                                        foreach (FieldInfo sususuf in sususufi)
                                                                        {
                                                                            if (sususuf.GetUnderlyingType() == typeof(string))
                                                                            {
                                                                                Debug.Log("Value 4 = " + sususuf.GetValue(z));
                                                                                if (!results.Contains(sususuf.GetValue(z).ToString()))
                                                                                {
                                                                                    results.Add(sususuf.GetValue(z).ToString());
                                                                                }
                                                                            }
                                                                            if (sususuf.GetUnderlyingType() != typeof(string) && typeof(System.Collections.IEnumerable).IsAssignableFrom(sususuf.GetUnderlyingType()))
                                                                            {
                                                                                Debug.Log("Nested sususuf = " + sususuf.GetUnderlyingType());

                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            catch
                            { }
                        }
                    }
                }
            }
            if (Input.GetKeyUp(KeyCode.F10) == true)
            {
                var unique = results.Distinct();
                TextWriter tw = new StreamWriter(Path.Combine(BepInEx.Paths.PluginPath, "results.txt"));

                foreach (string s in unique)
                {
                    if (Helpers.IsChinese(s))
                    {
                        tw.WriteLine(s);
                    }
                }
                tw.Close();
                var unique2 = MissingLines.Distinct();
                TextWriter tw2 = new StreamWriter(Path.Combine(BepInEx.Paths.PluginPath, "missing.txt"));

                foreach (string s2 in unique2)
                {
                    if (Helpers.IsChinese(s2))
                    {
                        tw2.WriteLine(s2);
                    }
                }
                tw2.Close();
            }
        }
    }


    [HarmonyPatch(typeof(TextMeshProUGUI), "OnEnable")]
    static class Patch00
    {
    /*    static AccessTools.FieldRef<TextMeshProUGUI, TMP_TextInfo> m_textInfoRef =
       AccessTools.FieldRefAccess<TextMeshProUGUI, TMP_TextInfo>("m_textInfo");
        static void Postfix(TextMeshProUGUI __instance)
        {

            var m_textInfo = m_textInfoRef(__instance);
            if (Main.translationDict.ContainsKey(m_textInfo.textComponent.text))
            {
                Debug.Log("Prefix String Case TMP __instance.text = " + m_textInfo.textComponent.text);

                m_textInfo.textComponent.text = Main.translationDict[m_textInfo.textComponent.text];

                //Traverse.Create(__instance).Field("text").SetValue(Main.translationDict[__instance.text]);

                Debug.Log("Prefix Replaced string = " + m_textInfo.textComponent.text);
            }
            else
            {
                Debug.Log("Prefix Missed line TMP = " + m_textInfo.textComponent.text);
                Main.MissingLines.Add(m_textInfo.textComponent.text);
                //Main.MissingLines.Add(__instance.textInfo.textComponent.text);
            }
        }*/
        static void Postfix(TextMeshProUGUI __instance)
        {

            if(Main.translationDict.ContainsKey(__instance.text.ToString()))
            {
                Debug.Log("String Case TMP __instance.text = " + __instance.text);
                Debug.Log("String Case TMP __instance.text2 = " + __instance.textInfo.textComponent.text);

                __instance.SetText(Main.translationDict[__instance.text]);

                //Traverse.Create(__instance).Field("text").SetValue(Main.translationDict[__instance.text]);

                Debug.Log("Replaced string = " + __instance.text);
            }
            else
            {
                Debug.Log("Missed line TMP = " + __instance.text);
                Main.MissingLines.Add(__instance.text);
                //Main.MissingLines.Add(__instance.textInfo.textComponent.text);
            }
        }
    }
    [HarmonyPatch(typeof(UnityEngine.UI.Text), "OnEnable")]
    static class Patch01
    {
        static void Postfix(UnityEngine.UI.Text __instance)
        {

            Debug.Log("String Case UI.Text = " + __instance.text);
            if (Main.translationDict.ContainsKey(__instance.text))
            {
                //Debug.Log("Found string = " + __instance.textInfo.textComponent.text);
                __instance.text = Main.translationDict[__instance.text];
                //Debug.Log("Replaced string = " + __instance.textInfo.textComponent.text);
            }

            else
            {
                Debug.Log("Missed line UEUIT = " + __instance.text);
                Main.MissingLines.Add(__instance.text);
            }
        }
    }
    [HarmonyPatch(typeof(TMP_TextInfo), "Clear")]
    static class Patch02
    {
        static void Prefix(TMP_TextInfo __instance)
        {

            Debug.Log("String Case TMP_TextInfo = " + __instance.textComponent.text);
            if (Main.translationDict.ContainsKey(__instance.textComponent.text))
            {
                Debug.Log("TMP_TextInfo Found string = " + __instance.textComponent.text);
                __instance.textComponent.text = Main.translationDict[__instance.textComponent.text];
                Traverse.Create(__instance.textComponent.text).Property("text").SetValue(__instance.textComponent.text);
                Debug.Log("TMP_TextInfo Replaced string = " + __instance.textComponent.text);
            }

            else
            {
                Debug.Log("Missing String Case TMP_TextInfo = " + __instance.textComponent.text);
                Main.MissingLines.Add(__instance.textComponent.text);
            }
        }
        static void Postfix(TMP_TextInfo __instance)
        {
               
            Debug.Log("String Case TMP_TextInfo = " + __instance.textComponent.text);
            if (Main.translationDict.ContainsKey(__instance.textComponent.text))
            {
                Debug.Log("TMP_TextInfo Found string = " + __instance.textComponent.text);
                __instance.textComponent.text = Main.translationDict[__instance.textComponent.text];
                Traverse.Create(__instance.textComponent.text).Property("text").SetValue(__instance.textComponent.text);
                Debug.Log("TMP_TextInfo Replaced string = " + __instance.textComponent.text);
            }

            else
            {
                Debug.Log("Missing String Case TMP_TextInfo = " + __instance.textComponent.text);
                Main.MissingLines.Add(__instance.textComponent.text);
            }
        }

    }
    public static class Helpers
    {
        public static readonly Regex cjkCharRegex = new Regex(@"\p{IsCJKUnifiedIdeographs}");
        public static bool IsChinese(string s)
        {
            return cjkCharRegex.IsMatch(s);
        }
    }
}
