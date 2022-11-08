using Newtonsoft.Json;
using UnityEngine;

namespace TaiwuShortcut
{
    public class KeySetting
    {
        [JsonProperty]
        public string KeyName { get; set; }
        [JsonProperty]
        public KeyCode KeyCode { get; set; } = KeyCode.None;
        [JsonProperty]
        public KeyCode FnKeyCode { get; set; } = KeyCode.None;
    }
}