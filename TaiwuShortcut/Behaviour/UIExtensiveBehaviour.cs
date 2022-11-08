using System;
using System.Collections.Generic;
using UnityEngine;

namespace TaiwuShortcut.Behaviour
{
    public class UIExtensiveBehaviour : MonoBehaviour
    {
        public UIElement BindElement;

        public List<(Shortcut, Action<UIElement>)> BindShortcutAction =
            new List<(Shortcut, Action<UIElement>)>(); 
        public event Action<UIElement> OnUpdate;

        public void Clear()
        {
            BindShortcutAction.Clear();
            OnUpdate = null;
        }

        public void RegisterUpdate(Action<UIElement> action)
        {
            OnUpdate += action;
        }

        private void Update()
        {
            foreach (var tuple in BindShortcutAction)
            {
                if (tuple.Item1.Check(BindElement, false, false, false))
                {
                    tuple.Item2.Invoke(BindElement);
                }
            }

            OnUpdate?.Invoke(BindElement);
        }
    }
}