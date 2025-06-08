using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Infrastructure.Extensions;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class CreateJsonLevelsFromWordList : EditorUtility
{
    private static Dictionary<string, string> WordPaths = new ()
    {
        {"Ru", "Assets/Content/Words/russian_nouns_400_2.txt"},
        {"En", "Assets/Content/Words/en_nouns_2.txt"},
        {"AllWords", "Assets/Content/Words/allWords.txt"},
        {"AllTowns", "Assets/Content/Words/allTowns.txt"},
        {"NeedWords", "Assets/Content/Words/needWords.txt"},
        {"NeedWords6", "Assets/Content/Words/needWords6.txt"}
    };

    [MenuItem("Definitions/Get words")]
    public static void GetWords()
    {
        var all = AssetDatabase.LoadAssetAtPath<TextAsset>(WordPaths["AllWords"]).text.Split('\n');
        var nouns = new List<string>();
       
        for (var i = 0 ; i < all.Length; i++)
        {
            var row = all[i];
            if (row.Contains("noun") == false)
                continue;

            var word = row.Split(' ')[2].Trim();
            if (word.Length < 5 || word.Length > 7)
                continue;

            nouns.Add(word);
        }
        for (var i = 0 ; i < all.Length; i++)
        {
            var row = all[i];
            if (row.Contains("noun"))
                continue;

            var word = row.Split(' ')[2].Trim();
            if (word.Length < 5 || word.Length > 7)
                continue;

            if (nouns.Contains(word))
                nouns.Remove(word);
        }
        var allTowns = AssetDatabase.LoadAssetAtPath<TextAsset>(WordPaths["AllTowns"]).text.Split('\n');
        for (var i = 0 ; i < allTowns.Length; i++)
        {
            var town = allTowns[i];
            if (town.Length < 5 || town.Length > 7)
                continue;

            var lowerTown = town.ToLower();
            if (nouns.Contains(lowerTown))
                nouns.Remove(lowerTown);
        }

        var sb = new StringBuilder();
        for (var i = 0 ; i < nouns.Count; i++)
        {
            var word = nouns[i];
            sb.Append($"{word}\n");
        }
        File.WriteAllText(WordPaths["NeedWords"], sb.ToString());

        sb.Clear();
        for (var i = 0 ; i < nouns.Count; i++)
        {
            var word = nouns[i];
            if (word.Length == 6)
                sb.Append($"{word}\n");
        }
        File.WriteAllText(WordPaths["NeedWords6"], sb.ToString());
    }

    [MenuItem("Definitions/Create levels")]
    public static void CreateLevels()
    {
        var objects = Selection.objects;
        foreach (var obj in objects)
        {
            if (obj is TextAsset textAsset)
            {
                CreateLevels(textAsset.name, path: AssetDatabase.GetAssetPath(textAsset));
            }
        }
    }

    private static void CreateLevels(string locale, string path)
    {
        var words = AssetDatabase.LoadAssetAtPath<TextAsset>(path).text.Split('\n');

        var sb = new StringBuilder();
        for (var i = 0 ; i < words.Length; i++)
        {
            var word = words[i].Trim();
            sb.Append($"\"{word.ToUpper()}\": [{GetClusters(word)}]\n");
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

            File.WriteAllText($@"Assets/Content/Definitions/Levels/{locale}_{i / 4 + 1}.json", sb.ToString());
        }
    }

    [MenuItem("Definitions/Create levels from 101 english words")]
    public static void CreateLevelsFrom101En()
    {
        var words = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Content/Words/nounlist_adding.txt").text.Split('\n');
        
        var sb = new StringBuilder();
        for (var i = 0 ; i < words.Length; i++)
        {
            var word = words[i].Trim();
            sb.Append($"\"{word.ToUpper()}\": [{GetClusters(word)}]\n");
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

            File.WriteAllText($@"Assets/Content/Definitions/Levels/En_{100 + i / 4 + 1}.json", sb.ToString());
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
            {
                clusterLength = length;
            }
            else if (length == 5 || length == 7)
            {
                clusterLength = Random.Range(2, 4);
            }
            else
            {
                clusterLength = 2;
            }

            length -= clusterLength;
            result += length > 0 ? $"{clusterLength}, " : $"{clusterLength}";
        }
        return result;
    }

    [MenuItem("Definitions/RemoveUseless")]
    public static void RemoveUseless()
    {
        foreach (var pair in WordPaths)
        {
            RemoveUseless(pair.Key, pair.Value);
        }
    }

    private static void RemoveUseless(string locale, string path)
    {
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
