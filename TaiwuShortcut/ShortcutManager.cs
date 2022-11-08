using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FrameWork;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TaiwuShortcut.Behaviour;
using UnityEngine;

namespace TaiwuShortcut
{
    public static class ShortcutManager
    {
        private static Dictionary<ShortcutGroup, List<Shortcut>> s_shortcuts =
            new Dictionary<ShortcutGroup, List<Shortcut>>();

        private static Dictionary<UIElement, List<Action<UIElement>>> s_onUIUpdates =
            new Dictionary<UIElement, List<Action<UIElement>>>();

        private static Dictionary<UIElement, List<ValueTuple<Shortcut, Action<UIElement>>>> s_onUIShortcuts =
            new Dictionary<UIElement, List<ValueTuple<Shortcut, Action<UIElement>>>>();

        private static ShortcutGlobalSetting s_globalSetting = new ShortcutGlobalSetting();
        private static string CustomSettingFile => Path.Combine(Game.GetArchiveDirPath(false), "modShortcutSetting.json");

        internal static void Clear()
        {
            s_shortcuts.Clear();
            s_onUIUpdates.Clear();
            s_onUIShortcuts.Clear();
            s_globalSetting = new ShortcutGlobalSetting();
        }
        
        internal static void Init()
        {
            Clear();
            AddShortcutGroup(ShortcutGroup.Common);
            AddShortcutGroup(ShortcutGroup.Combat);
            AddShortcutGroup(ShortcutGroup.Map);
            AddShortcutGroup(ShortcutGroup.EventWindow);
            LoadSetting();
        }
        
        internal static void OnUIElementShow(ArgumentBox argbox)
        {
            if (!argbox.Get("Element", out UIElement element))
                return;

            var ext = element.UiBase.GetComponent<UIExtensiveBehaviour>();
            if (ext == null)
            {
                ext = element.UiBase.gameObject.AddComponent<UIExtensiveBehaviour>();
            }
            
            ext.Clear();
            ext.BindElement = element;
            if (s_onUIShortcuts.TryGetValue(element, out var keyList))
            {
                ext.BindShortcutAction.AddRange(keyList);
            }
            if (s_onUIUpdates.TryGetValue(element, out var updateList))
            {
                foreach (var action in updateList)
                {
                    ext.RegisterUpdate(action);
                }
            }
        }

        internal static void SaveSetting()
        {
            try
            {
                var parent = Path.GetDirectoryName(CustomSettingFile);
                Directory.CreateDirectory(parent);
                File.WriteAllText(CustomSettingFile, JObject.FromObject(s_globalSetting)
                    .ToString(Formatting.Indented));
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        internal static void LoadSetting()
        {
            try
            {
                if (File.Exists(CustomSettingFile))
                {
                    s_globalSetting = JObject.Parse(File.ReadAllText(CustomSettingFile))
                        .ToObject<ShortcutGlobalSetting>();
                }
                else
                {
                    s_globalSetting = new ShortcutGlobalSetting();
                }
            }
            catch (Exception e)
            {
                s_globalSetting = new ShortcutGlobalSetting();
                Debug.LogError(e);
            }
        }
        
        internal static void SaveKeySetting(Shortcut shortcut, HotKeyCommand hotKey)
        {
            var keySetting = new KeySetting();
            var keyCodes = hotKey.GetKeyCode();
            if (keyCodes.Length == 2)
            {
                keySetting.FnKeyCode = keyCodes[0];
                keySetting.KeyCode = keyCodes[1];
            }
            else if (keyCodes.Length == 1)
            {
                keySetting.FnKeyCode = KeyCode.None;
                keySetting.KeyCode = keyCodes[0];
            }

            s_globalSetting.KeySettings[shortcut.Key] = keySetting;
        }
        
        internal static void LoadKeySetting(Shortcut shortcut, HotKeyCommand hotKey)
        {
            if (s_globalSetting.KeySettings.TryGetValue(shortcut.Key, out var setting))
            {
                hotKey.SetCustomKey(setting.KeyCode, setting.FnKeyCode);
            }
        }

        /// <summary>
        /// 添加快捷键
        /// </summary>
        /// <param name="key"></param>
        /// <param name="name">快捷键名称</param>
        /// <param name="group">快捷键组</param>
        /// <param name="keyCode">快捷键按键</param>
        /// <param name="fnKeyCode">快捷键辅助按键</param>
        /// <param name="canSet">是否允许设置快捷键</param>
        public static Shortcut AddShortcut(ShortcutGroup group, string key,string name, KeyCode keyCode, 
            KeyCode fnKeyCode = KeyCode.None, bool canSet = true)
        {
            var shortcut = new Shortcut(key, name, keyCode, fnKeyCode, canSet);
            AddShortcut(group, shortcut);
            return shortcut;
        }

        /// <summary>
        /// 添加快捷键
        /// </summary>
        /// <param name="group"></param>
        /// <param name="shortcut"></param>
        public static void AddShortcut(ShortcutGroup group, Shortcut shortcut)
        {
            AddShortcutGroup(group);
            s_shortcuts[group].Add(shortcut);
        }

        /// <summary>
        /// 移除快捷键
        /// </summary>
        /// <param name="group"></param>
        /// <param name="shortcut"></param>
        public static void RemoveShortcut(ShortcutGroup group, Shortcut shortcut)
        {
            if (s_shortcuts.ContainsKey(group))
            {
                s_shortcuts[group].Remove(shortcut);
            }
        }
        
        /// <summary>
        /// 添加快捷键组
        /// </summary>
        /// <param name="group"></param>
        public static void AddShortcutGroup(ShortcutGroup group)
        {
            if (!s_shortcuts.TryGetValue(group, out var list))
            {
                list = new List<Shortcut>();
                s_shortcuts.Add(group, list);
            }
        }

        /// <summary>
        /// 移除快捷键组
        /// </summary>
        /// <param name="group"></param>
        public static void RemoveShortcutGroup(ShortcutGroup group)
        {
            if (s_shortcuts.ContainsKey(group))
                s_shortcuts.Remove(group);
        }

        /// <summary>
        /// 添加UI快捷键检测
        /// </summary>
        /// <param name="uiElement"></param>
        /// <param name="shortcut"></param>
        /// <param name="action"></param>
        public static void AddUIShortcutAction(UIElement uiElement, Shortcut shortcut, Action<UIElement> action)
        {
            if (!s_onUIShortcuts.TryGetValue(uiElement, out var list))
            {
                list = new List<ValueTuple<Shortcut, Action<UIElement>>>();
                s_onUIShortcuts[uiElement] = list;
            }
            
            list.Add(new ValueTuple<Shortcut, Action<UIElement>>(shortcut, action));
        }

        /// <summary>
        /// 添加UI每帧行为
        /// </summary>
        /// <param name="uiElement"></param>
        /// <param name="action"></param>
        public static void AddUIUpdateAction(UIElement uiElement, Action<UIElement> action)
        {
            if (!s_onUIUpdates.TryGetValue(uiElement, out var list))
            {
                list = new List<Action<UIElement>>();
                s_onUIUpdates[uiElement] = list;
            }
            
            list.Add(action);
        }
        
        /// <summary>
        /// 移除UI快捷键检测
        /// </summary>
        /// <param name="uiElement"></param>
        /// <param name="shortcut"></param>
        /// <param name="action"></param>
        public static void RemoveUIShortcutAction(UIElement uiElement, Shortcut shortcut, Action<UIElement> action)
        {
            if (s_onUIShortcuts.TryGetValue(uiElement, out var list))
            {
                var index = list.IndexOf((shortcut, action));
                if (index >= 0)
                {
                    list.RemoveAt(index);
                }
            }
        }
        
        /// <summary>
        /// 移除UI每帧行为
        /// </summary>
        /// <param name="uiElement"></param>
        /// <param name="action"></param>
        public static void RemoveUIUpdateAction(UIElement uiElement, Action<UIElement> action)
        {
            if (s_onUIUpdates.TryGetValue(uiElement, out var list))
            {
                var index = list.IndexOf(action);
                if (index >= 0)
                {
                    list.RemoveAt(index);
                }
            }
        }

        /// <summary>
        /// 创建并添加快捷键组
        /// </summary>
        /// <param name="name"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static ShortcutGroup CreateAndAddShortcutGroup(string name, int order)
        {
            var group = new ShortcutGroup(name, order);
            AddShortcutGroup(group);
            return group;
        }

        public static IEnumerable<ShortcutGroup> GetGroupsInOrderRange(int includeMinValue, int excludeMaxValue)
        {
            return s_shortcuts
                .Select(s => s.Key)
                .Where(g => g.Order >= includeMinValue 
                            && g.Order < excludeMaxValue 
                            && !IsSystemGroup(g)
                            && g.IsShow)
                .OrderBy(g => g.Order);
        }

        public static bool IsSystemGroup(ShortcutGroup group)
        {
            return group == ShortcutGroup.Common
                   || group == ShortcutGroup.Combat
                   || group == ShortcutGroup.Map
                   || group == ShortcutGroup.EventWindow;
        }

        public static IEnumerable<Shortcut> GetAllShortcut(ShortcutGroup group)
        {
            if (s_shortcuts.ContainsKey(group))
            {
                return s_shortcuts[group];
            }

            return Array.Empty<Shortcut>();
        }

        public static bool Check(Shortcut shortcut, UIElement element, bool holdCheck = false,
            bool downCheck = false, bool isIgnoreBlockHotKey = false)
        {
            if (!element.ForceListenCommand && !UIManager.Instance.IsFocusElement(element))
                return false;
            return Check(shortcut, holdCheck, downCheck, isIgnoreBlockHotKey);
        }

        public static bool Check(Shortcut shortcut, bool holdCheck = false,
            bool downCheck = false, bool isIgnoreBlockHotKey = false)
        {
            if (!isIgnoreBlockHotKey && CommandKitBase.GetDisable() || UIManager.Instance.BlockHotKey)
                return false;
            if (shortcut.FnKeyCode == KeyCode.None)
            {
                if (holdCheck)
                    return Input.GetKey(shortcut.KeyCode);
                return downCheck ? Input.GetKeyDown(shortcut.KeyCode) : Input.GetKeyUp(shortcut.KeyCode);
            }
            bool isFnKeyClick = false;
            switch (shortcut.FnKeyCode)
            {
                case KeyCode.RightShift:
                case KeyCode.LeftShift:
                    isFnKeyClick = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                    break;
                case KeyCode.RightControl:
                case KeyCode.LeftControl:
                    isFnKeyClick = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
                    break;
                case KeyCode.RightAlt:
                case KeyCode.LeftAlt:
                    isFnKeyClick = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
                    break;
                case KeyCode.RightCommand:
                case KeyCode.LeftCommand:
                    isFnKeyClick = Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand);
                    break;
            }
            if (holdCheck)
                return isFnKeyClick && Input.GetKey(shortcut.KeyCode);
            return isFnKeyClick && (downCheck ? Input.GetKeyDown(shortcut.KeyCode) : Input.GetKeyUp(shortcut.KeyCode));
        }
    }
}