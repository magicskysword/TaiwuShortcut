using System;
using System.Collections.Generic;
using FrameWork;
using HarmonyLib;
using UnityEngine;

namespace TaiwuShortcut.Patch
{
    public class UISystemSettingPatch
    {
        private static Dictionary<Shortcut, HotKeyCommand> s_commands = new Dictionary<Shortcut, HotKeyCommand>();

        [HarmonyPatch(typeof(UI_SystemSetting), "InitHotKeysDataList")]
        [HarmonyPriority(-1)]
        [HarmonyPrefix]
        public static bool InitHotKeyPrefix(UI_SystemSetting __instance)
        {
            if (AccessTools.Field(typeof(UI_SystemSetting), "_hotKeysDataList") == null)
                return true;

            var settings = Traverse.Create(__instance).Field<List<(sbyte, ushort, HotKeyCommand)>>("_hotKeysDataList");
            LocalStringManagerPatch.Clear();

            settings.Value = new List<(sbyte, ushort, HotKeyCommand)>();
            s_commands.Clear();
            
            var groups = ShortcutManager.GetGroupsInOrderRange(int.MinValue, ShortcutGroup.Common.Order);
            foreach (var group in groups)
            {
                AddGroup(settings.Value, group);
            }

            for (int cmdIndex = 0; cmdIndex < CommandKitBase.CommandKitArray.Length; ++cmdIndex)
            {
                CommandKitBase commandKit = CommandKitBase.CommandKitArray[cmdIndex];
                if (commandKit.ShowInSettings)
                {
                    settings.Value.Add((1, commandKit.GroupDescLanguageId, null));
                    for (int keyIndex = 0; keyIndex < commandKit.GroupCommand.Length; ++keyIndex)
                    {
                        HotKeyCommand hotKeyCommand = commandKit.GroupCommand[keyIndex];
                        settings.Value.Add((2, commandKit.Id, hotKeyCommand));
                    }
                }

                if (commandKit == CommandKitBase.CommonCommandKit)
                {
                    AddGroupContent(settings.Value, ShortcutGroup.Common);
                    groups = ShortcutManager.GetGroupsInOrderRange(ShortcutGroup.Common.Order,
                        ShortcutGroup.Combat.Order);
                    foreach (var group in groups)
                    {
                        AddGroup(settings.Value, group);
                    }
                }
                else if (commandKit == CommandKitBase.CombatCommandKit)
                {
                    AddGroupContent(settings.Value, ShortcutGroup.Combat);
                    groups = ShortcutManager.GetGroupsInOrderRange(ShortcutGroup.Combat.Order,
                        ShortcutGroup.Map.Order);
                    foreach (var group in groups)
                    {
                        AddGroup(settings.Value, group);
                    }
                }
                else if (commandKit == CommandKitBase.MapCommandKit)
                {
                    AddGroupContent(settings.Value, ShortcutGroup.Map);
                    groups = ShortcutManager.GetGroupsInOrderRange(ShortcutGroup.Map.Order,
                        ShortcutGroup.EventWindow.Order);
                    foreach (var group in groups)
                    {
                        AddGroup(settings.Value, group);
                    }
                }
                else if (commandKit == CommandKitBase.EventWindowKit)
                {
                    AddGroupContent(settings.Value, ShortcutGroup.EventWindow);
                    groups = ShortcutManager.GetGroupsInOrderRange(ShortcutGroup.EventWindow.Order,
                        int.MaxValue);
                    foreach (var group in groups)
                    {
                        AddGroup(settings.Value, group);
                    }
                }
            }
            __instance.CGet<GameObject>("HeadLine").SetActive(false);
            __instance.CGet<Refers>("ContentLine").gameObject.SetActive(false);

            return false;
        }
        
        public static void OnUIElementHide(ArgumentBox argbox)
        {
            if (!argbox.Get("Element", out UIElement element))
                return;

            if (element.Name != "UI_SystemSetting")
                return;

            var uiInstance = element.UiBaseAs<UI_SystemSetting>();
            if (uiInstance == null)
                return;

            foreach (var pair in s_commands)
            {
                ShortcutManager.SaveKeySetting(pair.Key, pair.Value);
            }
            ShortcutManager.SaveSetting();
        }

        private static void AddGroup(List<(sbyte, ushort, HotKeyCommand)> settings, ShortcutGroup group)
        {
            AddGroupTitle(settings, group);
            AddGroupContent(settings, group);
        }

        private static void AddGroupTitle(List<(sbyte, ushort, HotKeyCommand)> settings, ShortcutGroup group)
        {
            settings.Add((1, LocalStringManagerPatch.Add(group.Name), null));
        }

        private static void AddGroupContent(List<(sbyte, ushort, HotKeyCommand)> settings, ShortcutGroup group)
        {
            var shortcuts = ShortcutManager.GetAllShortcut(group);
            ushort index = 1;
            foreach (var shortcut in shortcuts)
            {
                if (!shortcut.IsShow)
                {
                    continue;
                }
                
                if (s_commands.ContainsKey(shortcut))
                {
                    Debug.LogWarning($"重复的快捷键Key : {shortcut.Key}，名称 : {shortcut.Name}");
                    continue;
                }
                var hotKeyCommand = CreateHotKeyCommand(index++ ,shortcut);
                s_commands.Add(shortcut, hotKeyCommand);
                settings.Add((2, hotKeyCommand.Id, hotKeyCommand));
            }
        }

        private static HotKeyCommand CreateHotKeyCommand(ushort id, Shortcut shortcut)
        {
            var hotKey = new HotKeyCommand((byte) id, LocalStringManagerPatch.Add(shortcut.Name), 
                shortcut.KeyCode, shortcut.FnKeyCode, shortcut.CanSet);
            ShortcutManager.LoadKeySetting(shortcut, hotKey);
            return hotKey;
        }
    }
}