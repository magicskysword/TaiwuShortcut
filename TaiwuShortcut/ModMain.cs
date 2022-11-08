using FrameWork;
using HarmonyLib;
using TaiwuModdingLib.Core.Plugin;
using TaiwuShortcut.Patch;
using UnityEngine;
using UnityEngine.UI;

namespace TaiwuShortcut
{
    [PluginConfig("快捷键框架 & 优化", "剑圣(skyswordkill)", "1.0.0")]
    public class ModMain : TaiwuRemakePlugin
    {
        private Harmony Harmony { get; set; }
        
        private ShortcutOptimize ShortcutOptimize { get; set; }

        private bool enableShortcutOptimize;
        
        public override void Initialize()
        {
            Harmony = new Harmony("skyswordkill.taiwu.shortcut");
            Harmony.PatchAll(typeof(UISystemSettingPatch));
            Harmony.PatchAll(typeof(LocalStringManagerPatch));
            ShortcutManager.Init();

            GEvent.Add(UiEvents.OnUIElementHide, UISystemSettingPatch.OnUIElementHide);
            GEvent.Add(UiEvents.OnUIElementShow, ShortcutManager.OnUIElementShow);

            ShortcutOptimize = new ShortcutOptimize();
            ShortcutOptimize.Init();
        }

        public override void OnModSettingUpdate()
        {
            ModManager.GetSetting(ModIdStr, "Bool_EnableShortcutOptimize", ref enableShortcutOptimize);
            ShortcutOptimize.SetEnable(enableShortcutOptimize);
        }

        public override void Dispose()
        {
            ShortcutOptimize.Dispose();
            ShortcutOptimize = null;
            
            Harmony.UnpatchSelf();
            Harmony = null;
            
            GEvent.Remove(UiEvents.OnUIElementHide, UISystemSettingPatch.OnUIElementHide);
            GEvent.Remove(UiEvents.OnUIElementShow, ShortcutManager.OnUIElementShow);
            ShortcutManager.Clear();
        }
    }
}