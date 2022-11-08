using System;
using System.Collections.Generic;

namespace TaiwuShortcut
{
    public abstract class ModShortcutCollection
    {
        public virtual bool Enable { get; private set; } = false;

        public List<(ShortcutGroup, Shortcut)> Shortcuts = new List<(ShortcutGroup, Shortcut)>();
        public List<(UIElement, Shortcut, Action<UIElement>)> UIShortcutActions =
            new List<(UIElement, Shortcut, Action<UIElement>)>();
        public List<(UIElement, Action<UIElement>)> UIUpdateActions
            = new List<(UIElement, Action<UIElement>)>();

        public void Init()
        {
            OnInit();
            SetEnable(true);
        }

        public void Dispose()
        {
            SetEnable(false);
        }

        public void RegisterShortcut(ShortcutGroup group, Shortcut shortcut)
        {
            Shortcuts.Add((group, shortcut));
        }
        
        public void RegisterUIShortcut(UIElement element, Shortcut shortcut, Action<UIElement> action)
        {
            UIShortcutActions.Add((element, shortcut, action));
        }
        
        public void RegisterUIUpdate(UIElement element, Action<UIElement> action)
        {
            UIUpdateActions.Add((element, action));
        }

        public virtual void SetEnable(bool enable)
        {
            if (Enable && !enable)
            {
                OnDisable();
            }
            else if (!Enable && enable)
            {
                OnEnable();
            }
            
            Enable = enable;
        }
        
        public abstract void OnInit();

        protected virtual void OnEnable()
        {
            foreach (var tuple in Shortcuts)
            {
                ShortcutManager.AddShortcut(tuple.Item1, tuple.Item2);
            }
            
            foreach (var tuple in UIShortcutActions)
            {
                ShortcutManager.AddUIShortcutAction(tuple.Item1, tuple.Item2, tuple.Item3);
            }
            
            foreach (var tuple in UIUpdateActions)
            {
                ShortcutManager.AddUIUpdateAction(tuple.Item1, tuple.Item2);
            }
        }
        
        protected virtual void OnDisable()
        {
            foreach (var tuple in Shortcuts)
            {
                ShortcutManager.RemoveShortcut(tuple.Item1, tuple.Item2);
            }
            
            foreach (var tuple in UIShortcutActions)
            {
                ShortcutManager.RemoveUIShortcutAction(tuple.Item1, tuple.Item2, tuple.Item3);
            }
            
            foreach (var tuple in UIUpdateActions)
            {
                ShortcutManager.RemoveUIUpdateAction(tuple.Item1, tuple.Item2);
            }
        }
    }
}