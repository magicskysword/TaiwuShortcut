using UnityEngine.UI;

namespace TaiwuShortcut
{
    public static class ShortcutExtension
    {
        public static bool Check(this Shortcut shortcut,UIElement element, bool holdCheck = false,
            bool downCheck = false, bool isIgnoreBlockHotKey = false)
        {
            return ShortcutManager.Check(shortcut, element, holdCheck, downCheck, isIgnoreBlockHotKey);
        }

        public static bool Check(this Shortcut shortcut,bool holdCheck = false, bool downCheck = false, bool isIgnoreBlockHotKey = false)
        {
            return ShortcutManager.Check(shortcut, holdCheck, downCheck, isIgnoreBlockHotKey);
        }

        public static bool CanClick(this Button button)
        {
            return button != null && button.gameObject.activeInHierarchy && button.interactable;
        }
        
        public static bool CanToggle(this Toggle tgl)
        {
            return tgl != null && tgl.gameObject.activeInHierarchy && tgl.interactable;
        }
    }
}