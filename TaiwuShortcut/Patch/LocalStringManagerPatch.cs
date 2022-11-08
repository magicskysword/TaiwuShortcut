using System.Collections.Generic;
using HarmonyLib;

namespace TaiwuShortcut.Patch
{
    public class LocalStringManagerPatch
    {
        private static Dictionary<ushort, string> s_tmpLanguages = new Dictionary<ushort, string>();
        private static ushort s_tmpIndex = 0;
        
        [HarmonyPatch(typeof(LocalStringManager), "Get", typeof(ushort))]
        [HarmonyPrefix]
        public static bool LanguageManagerPatch(ref string __result, ushort id)
        {
           if(s_tmpLanguages.TryGetValue(id, out var value))
           {
               __result = value;
               return false;
           }
           
           return true;
        }

        private static void ReCalculateIndex()
        {
            var array = Traverse.Create(typeof(LocalStringManager))
                .Field("_localUILanguageArray")
                .GetValue<string[]>();
            
            if(array != null)
            {
                s_tmpIndex = (ushort)(array.Length + 500);
            }
            else
            {
                s_tmpIndex = 50000;
            }
        }
        
        public static void Clear()
        {
            s_tmpLanguages.Clear();
            s_tmpIndex = 0;
            ReCalculateIndex();
        }

        public static ushort Add(string text)
        {
            if(s_tmpIndex == 0)
                ReCalculateIndex();

            var index = s_tmpIndex;
            s_tmpLanguages.Add(index, text);
            s_tmpIndex += 1;
            return index;
        }
    }
}