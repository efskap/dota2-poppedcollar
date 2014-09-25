using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;
namespace InvokerWPF
{
    class Config
    {
        private static Dictionary<string, object> config = new Dictionary<string, object>();
        public static string filename = "config.json";
        public static void Load()
        {
            try
            {
                config = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(filename));
            }
            catch (Exception)
            {
                config = new Dictionary<string, object>();
                Save();
            }
        }
        public static void Save()
        {
            File.WriteAllText(filename, JsonConvert.SerializeObject(config));
        }
        public static void Set(string key, object o, bool save = true)
        {
            if (!config.ContainsKey(key))
                config.Add(key, o);
            else
                config[key] = o;

            if (save)
                Save();

        }
        public static T Get<T>(string key, T defaultObject)
        {
            if (!config.ContainsKey(key))
            {
                Set(key, defaultObject);
                return (T)defaultObject;

            }
            else
                return (T)config[key];

        }
    }
}
