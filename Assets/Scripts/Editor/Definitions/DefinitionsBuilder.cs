#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using GameLogic.Model.Definitions;

namespace EditorDefinitions
{
    public class DefinitionsBuilder : EditorUtility 
    {
        private static string DefinitionsFilePath => $"{Application.dataPath}/Definitions.json";

        [MenuItem("Definitions/Build")]
        public static void BuildDefinitions()
        {
            var gameDefs = new GameDefs();
            var gameDefsType = typeof(GameDefs);
            var fields = gameDefsType.GetFields();
            var properties = gameDefsType.GetProperties();

            var definitionPaths = AssetDatabase.GetAllAssetPaths().ToList();
            definitionPaths.RemoveAll(p => !p.StartsWith("Assets/Content/Definitions/"));

            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None,
                Formatting = Formatting.Indented
            };

            var loadedDefs = new List<BaseDef>();
            var dictFieldTypes = new Dictionary<string, (FieldInfo, Type)>();
            var propertyTypes = new Dictionary<string, (PropertyInfo, Type)>();
            foreach (var fieldInfo in fields)
            {
                if (fieldInfo.FieldType.IsGenericType == false) 
                    continue;

                var args = fieldInfo.FieldType.GetGenericArguments();
                dictFieldTypes.Add(fieldInfo.Name, (fieldInfo, args[1]));
            }
            foreach (var propertyInfo in properties)
            {
                propertyTypes.Add(propertyInfo.Name, (propertyInfo, propertyInfo.PropertyType));
            }

            foreach (var definitionPath in definitionPaths)
            {
                var pathParts = definitionPath.Split('/');
                var fileName = pathParts[^1].Split('.')[0];
                var dirName = pathParts[^2];

                if (dictFieldTypes.TryGetValue(dirName, out var dictField))
                {
                    var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(definitionPath);
                    if (JsonConvert.DeserializeObject(textAsset.text, dictField.Item2, settings) is not BaseDef loadedDef)
                        throw new Exception($"Failed to load {fileName} from {definitionPath} - it is not BaseDef type");

                    loadedDef.Id = fileName;

                    if (dictField.Item1.GetValue(gameDefs) is not IDictionary dictFieldValue)
                        throw new Exception($"Failed to get {fileName} from {definitionPath}");

                    dictFieldValue.Add(fileName, loadedDef);
                    loadedDefs.Add(loadedDef);
                }
                else if (propertyTypes.TryGetValue(fileName, out var property))
                {
                    var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(definitionPath);
                    if (JsonConvert.DeserializeObject(textAsset.text, property.Item2, settings) is not BaseDef loadedDef)
                        throw new Exception($"Failed to load {fileName} from {definitionPath} - it is not BaseDef type");

                    loadedDef.Id = fileName;
                    property.Item1.SetValue(gameDefs, loadedDef);
                    loadedDefs.Add(loadedDef);
                }
            }

            var validator = new GameDefsValidator(gameDefs);
            validator.Validate();

            var json = JsonConvert.SerializeObject(gameDefs, Formatting.Indented, settings);
            Debug.Log($"{json}");

            File.WriteAllText(DefinitionsFilePath, json);
            AssetDatabase.Refresh();
        }

    }
}
#endif