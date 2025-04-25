using System.IO;
using System.Linq;
using System.Text;
using Infrastructure.Extensions;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Editor
{
    public class CreateJsonLevelsFromWordList : EditorUtility  
    {
        
        [MenuItem("Definitions/Create levels")]
        public static void CreateLevels()
        {
            var path = "Assets/Content/Words/russian_nouns_400_2.txt";
            //var path = "Assets/Content/Words/en_nouns_2.txt";
            var enNouns = AssetDatabase.LoadAssetAtPath<TextAsset>(path).text.Split('\n');

            var sb = new StringBuilder();
            for (var i = 0 ; i < enNouns.Length; i++)
            {
                sb.Append($"\"{enNouns[i].ToUpper()}\": [{GetClusters(enNouns[i])}]\n");
            }

            var allWordClusters = sb.ToString().Split('\n');
            for (int i = 0; i < allWordClusters.Length; i += 4)
            {
                sb.Clear();
                sb.Append("{\n");
                sb.Append($"\t\"Words\":\n");
                sb.Append("\t{\n");
                for (var j = 0; j < 4 && i + j < allWordClusters.Length; j++)
                {
                    var end = j == 3 ? "" : ",";
                    sb.Append($"\t\t{allWordClusters[i + j]}{end}\n");
                }
                sb.Append("\t}\n");
                sb.Append("}\n");

                File.WriteAllText($@"Assets/Content/Definitions/Levels/Ru_{i / 4 + 1}.json", sb.ToString());
                //File.WriteAllText($@"Assets/Content/Definitions/Levels/En_{i / 4 + 1}.json", sb.ToString());
            }
        }

        private static string GetClusters(string word)
        {
            var result = string.Empty;
            var length = word.Length;
            while (length > 0)
            {
                var clusterLength = 0;
                if (length == 3 || length == 2)
                    clusterLength = length;
                else if (length == 4)
                    clusterLength = Random.Range(1, 3) > 1 ? 4 : 2;
                else if (length > 4)
                    clusterLength = Random.Range(2, 4);

                length -= clusterLength;
                result += length > 0 ? $"{clusterLength}, " : $"{clusterLength}";
            }
            return result;
        }

        [MenuItem("Definitions/RemoveUseless")]
        public static void RemoveUseless()
        {
            var path = "Assets/Content/Words/russian_nouns_400_2.txt";
            //var path = "Assets/Content/Words/en_nouns_2.txt";
            var enNouns = AssetDatabase.LoadAssetAtPath<TextAsset>(path).text.Split('\n').Distinct().ToList();

            var withoutUseless = enNouns.ToList();
            withoutUseless.Shuffle();

            var sb = new StringBuilder();
            for (var i = withoutUseless.Count() - 1; i >= 0; i--)
            {
                var word = withoutUseless[i];
                if (word.Length <= 4 || word.Length > 7)
                    continue;

                if (word.Contains("ий") 
                    || word.Contains("ый") 
                    || word.Contains("ая") 
                    || word.Contains("яя") 
                    || word.Contains("ей"))
                    continue;

                sb.Append($"{withoutUseless[i].TrimEnd()}\n");
            }

            File.WriteAllText(path, sb.ToString());
        }
    }
}