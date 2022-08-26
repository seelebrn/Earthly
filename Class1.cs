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
            if (context.Asset is ScriptableObject mb) // also acts as a null check
            {
                Debug.Log("mb.name = " + mb.name);
                if (mb.name != "forbidden")
                {
                    FieldInfo[] fi = mb.GetType().GetFields();
                    foreach (FieldInfo f in fi)
                    {
                        Debug.Log("f.Name = " + f.Name);
                        var value = f.GetValue(mb);
                        Debug.Log("Value Type = " + value.GetType());
                        if(value.GetType() == typeof(string))
                        {
                            results.Add(value.ToString());
                        }
                        else { 
                        foreach (var x in value as IEnumerable<object>)
                        {
                            Debug.Log("x.Name = " + x.ToString());
                            FieldInfo[] sufi = x.GetType().GetFields();
                            foreach (FieldInfo suf in sufi)
                            {
                                if (suf.GetUnderlyingType() == typeof(string))
                                {
                                    results.Add(suf.GetValue(x).ToString());
                                }
                                else if (suf.GetUnderlyingType() == typeof(string[]))
                                {
                                    foreach (string s in suf.GetValue(x) as string[])
                                    {
                                        results.Add(s);
                                    }
                                }
                                else if (suf.GetUnderlyingType() == typeof(List<string>))
                                {
                                    foreach (string s in suf.GetValue(x) as List<string>)
                                    {
                                        results.Add(s);
                                    }
                                }


                                else
                                {
                                    Debug.Log("Missing Type = " + suf.GetUnderlyingType());
                                    if (suf.GetUnderlyingType() == typeof(Dialog[])
                                        || suf.GetUnderlyingType() == typeof(Pellet[])
                                        || suf.GetUnderlyingType() == typeof(AShuFa[])
                                        || suf.GetUnderlyingType() == typeof(ItemStored[])
                                        || suf.GetUnderlyingType() == typeof(GodNodeLinkClassList[])
                                        || suf.GetUnderlyingType() == typeof(Option[])
                                        || suf.GetUnderlyingType() == typeof(WeaponTreeNode[])
                                        || suf.GetUnderlyingType() == typeof(ExploreFloor[])
                                        || suf.GetUnderlyingType() == typeof(ExploreFloor[])
                                        || suf.GetUnderlyingType() == typeof(List<TalkInfo>)
                                        || suf.GetUnderlyingType() == typeof(List<ItemRequire>)
                                        || suf.GetUnderlyingType() == typeof(List<Require>)
                                        || suf.GetUnderlyingType() == typeof(List<TradeRequire>)
                                        || suf.GetUnderlyingType() == typeof(List<AXinFa>)
                                        || suf.GetUnderlyingType() == typeof(List<ConsumableEffect>)
                                        || suf.GetUnderlyingType() == typeof(List<Replys>)
                                        || suf.GetUnderlyingType() == typeof(List<Influence>)
                                        || suf.GetUnderlyingType() == typeof(List<DropItem>)
                                        || suf.GetUnderlyingType() == typeof(List<DropGroupInfo>)
                                        || suf.GetUnderlyingType() == typeof(List<ExtraStuck>)
                                        )
                                    {
                                        Debug.Log("Here are the dialogs !");
                                        foreach (var y in suf.GetValue(x) as IEnumerable<object>)
                                        {
                                            Debug.Log("y.Name = " + y.ToString());
                                            FieldInfo[] susufi = y.GetType().GetFields();
                                            foreach (FieldInfo susuf in susufi)
                                            {
                                                Debug.Log("SusufName = " + susuf.Name + " + SususfType = " + susuf.GetUnderlyingType());
                                                if (susuf.GetUnderlyingType() == typeof(string))
                                                {
                                                    results.Add(susuf.GetValue(y).ToString());
                                                }

                                            }
                                        }
                                    }
                                    else
                                    {
                                        Debug.Log("Missing Type sublevel = " + suf.GetUnderlyingType());
                                    }
                                }
                            }


                        }
                    }
                    }
                }

                context.Asset = mb; // only need to update the reference if you created a new texture
                context.Complete(
                    skipRemainingPostfixes: true);

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
