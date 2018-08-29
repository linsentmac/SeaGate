//-----------------------------------------------------------------------
// <copyright>
//
// Copyright 2016 Lenovo Inc. All Rights Reserved.
//
// </copyright>
//-----------------------------------------------------------------------

using SimpleJson;
using System.Collections.Generic;

namespace LARSuite
{
    /// <summary>
    /// Json Utility.
    /// </summary>
    public class JsonObjectUtility {
        /// <summary>
        /// Build a <see cref="JsonObject"/> list from a Json string. 
        /// </summary>
        /// <param name="jsonString">Json content.</param>
        /// <returns>A JsonObject list.</returns>
        static public List<JsonObject> BuildJsonList(string jsonString) {
            List<JsonObject> jsons = new List<JsonObject>();
            if (jsonString != null && jsonString.Length > 0) {
                object jsonObject = (object)SimpleJson.SimpleJson.DeserializeObject(jsonString);
                if (jsonObject is JsonArray) {
                    foreach (JsonObject json in (JsonArray)jsonObject) {
                        jsons.Add(json);
                    }
                } else if (jsonObject is JsonObject) {
                    jsons.Add((JsonObject)jsonObject);
                }
            }

            return jsons;
        }

        /// <summary>
        /// Get string value from <see cref="JsonObject"/> with a given key. 
        /// </summary>
        /// <param name="jsonObject">A <see cref="JsonObject"/>.</param>
        /// <param name="key">The key value.</param>
        /// <param name="defaultValue">The default return value if the key not exist.</param>
        /// <returns>string value.</returns>
        static public string GetStringValue(JsonObject jsonObject, string key, string defaultValue) {
            object property;
            return jsonObject.TryGetValue(key, out property) ? (property != null ? property.ToString() : defaultValue) : defaultValue;
        }

        /// <summary>
        /// Get a <see cref="JsonObject"/> list from a JsonObject with a given key.
        /// Return an empty list if the key doesn't exist.
        /// </summary>
        /// <param name="jsonObject">A <see cref="JsonObject"/>.</param>
        /// <param name="key">The key.</param>
        /// <returns>A <see cref="JsonObject"/> List.</returns>
        static public List<JsonObject> GetJsonList(JsonObject jsonObject, string key) {
            List<JsonObject> jsons = new List<JsonObject>();

            object outObject;
            if(jsonObject.TryGetValue(key, out outObject)) {
                if (outObject is JsonArray) {
                    foreach (JsonObject json in (JsonArray)outObject) {
                        jsons.Add(json);
                    }
                } else if (jsonObject is JsonObject) {
                    jsons.Add(jsonObject);
                }
            }

            return jsons;
        }
    }
}
