using UnityEngine;
using Newtonsoft.Json;

public static class DataService
{
    /// <summary>
    /// This function returns object class by type from prefs.
    /// </summary>
    /// <param name="key"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T LoadObjectWithKey<T>(string key) where T : new()
    {
        T cachedClass = JsonConvert.DeserializeObject<T>(PlayerPrefs.GetString(key), GetJsonConverterSettings());;

        if (cachedClass == null)
        {
            cachedClass = new T();
            Debug.Log($"Data Not Found. Type : {typeof(T)}");
        }
        else
        {
            Debug.Log($"Data Found. Type : {typeof(T)} JSON Data : {PlayerPrefs.GetString(key)}");
        }
        
        return cachedClass;
    }

    /// <summary>
    /// This function returns target object as json.
    /// </summary>
    /// <param name="targetObject"></param>
    /// <returns></returns>
    public static string ObjectToJson(object targetObject)
    {
        return JsonConvert.SerializeObject(targetObject, GetJsonConverterSettings());
    }

    /// <summary>
    /// This function returns json converter settings.
    /// </summary>
    /// <returns></returns>
    private static JsonSerializerSettings GetJsonConverterSettings()
    {
        return new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };
    }
}
