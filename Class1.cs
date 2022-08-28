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



namespace 天地归虚ENMod
{


    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class Main : BaseUnityPlugin
    {
        public static BepInEx.Logging.ManualLogSource log;
        public static List<string> results = new List<String>();
        public static List<string> masterlist = new List<string>()
            {
"ANPCCharaTemplate",
"Weapon",
"PelletGroup",
"Pellet",
"PelletFormulaBook",
"PelletFormula",
"Materials",
"Item",
"Stove",
"Fire",
"MagicPaper",
"WeaponFormula",
"ZhuangShiPin",
"StovePos",
"StovePosBook",
"TbsFramework.Skill.SkillInfo",
"EffectInfo",
"LifeSkill",
"TbsFramework.Skill.ParticleInfo",
"TbsFramework.Skill.WallInfo",
"BattleSceneInfo",
"BossInfo",
"WeatherInfo",
"ArmatureMod",
"EntryInfo",
"BuffInfo",
"TradeEntryInfo",
"FbEntryInfo",
"ZhenFa",
"XinFa",
"ShuFa",
"AbstractXinFaBook",
"AbstractShuFaBook",
"ExploreItem",
"WeaponBattleInfo",
"MonsterGroupInfo",
"MonsterTermInfo",
"DropItemGroup",
"DropTerm",
"DropTermManager",
"ResistInfo",
"AreaLeveInfo",
"GodClass",
"GodNode",
"PlantClass",
"WeaponTreeNode",
"TechTreeNode",
"ATechTree",
"Consumable",
"PaiXiTemple",
"DicSenseInfo",
"UnityEngine.UI.Text",
"TMPro.TextMeshProUGUI",
"Flowchart"
            };
        public const string pluginGuid = "Cadenza.ENMOD.0.5";
        public const string pluginName = "ENMod";
        public const string pluginVersion = "0.5";
        public static Dictionary<string, string> translationDict;
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


        public static System.Collections.IDictionary d;

        void Awake()
        {
            ResourceRedirection.RegisterAssetLoadedHook(
                behaviour: HookBehaviour.OneCallbackPerResourceLoaded,
                priority: 0,
                action: AssetLoaded);
        }

        private void Update()
        {
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
            }
        }
        public void AssetLoaded(AssetLoadedContext context)
        {
            if (context.Asset is GameObject go) // also acts as a null check
            {
                

                Component[] mb = go.GetComponentsInChildren(typeof(MonoBehaviour));
                foreach (Component c in mb)
                {

                    if (typeof(MonoBehaviour).IsAssignableFrom(c.GetType()))
                    {
                        PropertyInfo[] pi = c.GetType().GetProperties();
                        foreach (PropertyInfo p in pi)
                        {
                            if (p.PropertyType == typeof(string))
                            { 
                            Debug.Log("Value Property = " + p.GetValue(c));
                                if (!results.Contains(p.GetValue(c)))
                                {
                                    results.Add(p.GetValue(c).ToString());
                                }
                            }
                            if(p.PropertyType == typeof(UnityEngine.UI.Text))
                            {
                                UnityEngine.UI.Text x = p.GetValue(c) as UnityEngine.UI.Text;
                                results.Add(x.text);
                            }
                            if (p.PropertyType == typeof(TextMeshProUGUI))
                            {
                                TextMeshProUGUI x = p.GetValue(c) as TextMeshProUGUI;
                                results.Add(x.text);
                            }
                        }

                    }
                }

                

                context.Asset = go; // only need to update the reference if you created a new texture
                context.Complete(
                    skipRemainingPostfixes: true);

            }
            /*
            if (context.Asset is ScriptableObject so) // also acts as a null check
            {

                MemberInfo[] mi = so.GetType().GetMembers();
                {
                    foreach(MemberInfo m in mi)
                    {
                        if(m.MemberType.ToString() == "Field")
                        {
                            FieldInfo f = (FieldInfo)m;
                            Debug.Log("Field name = " + f.Name);
                            Debug.Log("Field type = " + f.GetUnderlyingType());
                            if(f.GetUnderlyingType() == typeof(string))
                            {
                                Debug.Log("Value = " + f.GetValue(so));
                                if (!results.Contains(f.GetValue(so).ToString()))
                                {
                                    results.Add(f.GetValue(so).ToString());
                                }
                            }
                            else
                            {
                                if (typeof(System.Collections.IEnumerable).IsAssignableFrom(f.GetUnderlyingType()))
                                {
                                    foreach (var x in f.GetValue(so) as IEnumerable<object>)
                                    {
                                        Debug.Log(x.GetType());
                                        FieldInfo[] sufi = x.GetType().GetFields();
                                        foreach (FieldInfo suf in sufi)
                                        {
                                            if (suf.GetUnderlyingType() == typeof(string))
                                            { Debug.Log("Value 2 = " + suf.GetValue(x));
                                                if(!results.Contains(suf.GetValue(x).ToString()))
                                                {
                                                    results.Add(suf.GetValue(x).ToString());
                                                }

                                            }
                                            if (suf.GetUnderlyingType() != typeof(string) && typeof(System.Collections.IEnumerable).IsAssignableFrom(suf.GetUnderlyingType()))
                                            {
                                                foreach(var y in suf.GetValue(x) as IEnumerable<object>)
                                                {
                                                    FieldInfo[] susufi = y.GetType().GetFields();
                                                    foreach(FieldInfo susuf in susufi)
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
                        if(m.MemberType.ToString() == "Property")
                        {
                            PropertyInfo f = (PropertyInfo)m;
                            Debug.Log("Property name = " + f.Name);

                            if (f.GetUnderlyingType() == typeof(string))
                            { 
                                Debug.Log("Value = " + f.GetValue(so)); 
                            }
                         
                        }


                    }
                }



                context.Asset = so; // only need to update the reference if you created a new texture
                context.Complete(
                    skipRemainingPostfixes: true);

            }*/
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
