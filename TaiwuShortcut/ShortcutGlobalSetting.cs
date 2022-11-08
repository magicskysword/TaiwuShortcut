using System.Collections.Generic;
using Newtonsoft.Json;

namespace TaiwuShortcut
{
    public class ShortcutGlobalSetting
    {
        [JsonProperty]
        public Dictionary<string, KeySetting> KeySettings { get; set; } = new Dictionary<string, KeySetting>();
        
    }
}