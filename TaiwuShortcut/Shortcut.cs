using UnityEngine;

namespace TaiwuShortcut
{
    public class Shortcut
    {
        public Shortcut(string key, string name, KeyCode keyCode, KeyCode fnKeyCode = KeyCode.None, bool canSet = true)
        {
            Key = key;
            Name = name;
            KeyCode = keyCode;
            FnKeyCode = fnKeyCode;
            CanSet = canSet;
        }
        
        public string Key { get; set; }
        public string Name { get; set; }
        public KeyCode KeyCode { get; set; }
        public KeyCode FnKeyCode { get; set; }
        public bool CanSet { get; set; }
        public bool IsShow { get; set; } = true;
    }
}