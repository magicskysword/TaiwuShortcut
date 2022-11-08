using System.Collections.Generic;

namespace TaiwuShortcut
{
    public class ShortcutGroup
    {
        public static readonly ShortcutGroup Common = new ShortcutGroup("Common", 0);
        public static readonly ShortcutGroup Combat = new ShortcutGroup("Combat", 10);
        public static readonly ShortcutGroup Map = new ShortcutGroup("Map", 20);
        public static readonly ShortcutGroup EventWindow = new ShortcutGroup("EventWindow", 30);
        
        public string Name { get; }
        public int Order { get; }
        public bool IsShow { get; set; } = true;

        public ShortcutGroup(string name, int order = 100)
        {
            Name = name;
            Order = order;
        }
    }
}