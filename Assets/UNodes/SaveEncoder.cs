using System;
using System.Collections.Generic;
using UnityEngine;

namespace UNodes.Project
{
    public class SaveEncoder : BaseSaveEncoder
    {
        [Serializable] private class DictionaryWrapper
        {
            public List<string> keys = new();
            public List<string> values = new();
        }


        public override Dictionary<string, string> Decode(string saveDataString)
        {
            if (string.IsNullOrEmpty(saveDataString)) 
                return new Dictionary<string, string>();

            return JsonToDictionary(saveDataString);
        }


        public override string Encode(Dictionary<string, string> saveData)
            => DictionaryToJson(saveData);




        private string DictionaryToJson(Dictionary<string, string> dictionary)
        {
            var wrapper = new DictionaryWrapper();

            foreach (var pair in dictionary)
            {
                wrapper.keys.Add(pair.Key);
                wrapper.values.Add(pair.Value);
            }

            return JsonUtility.ToJson(wrapper);
        }

        private Dictionary<string, string> JsonToDictionary(string json)
        {
            var wrapper = JsonUtility.FromJson<DictionaryWrapper>(json);
            var dictionary = new Dictionary<string, string>();

            if (wrapper == null || wrapper.keys == null || wrapper.values == null)
                return dictionary;

            for (int i = 0; i < Math.Min(wrapper.keys.Count, wrapper.values.Count); i++)
                dictionary[wrapper.keys[i]] = wrapper.values[i];

            return dictionary;
        }

        
    }
}
