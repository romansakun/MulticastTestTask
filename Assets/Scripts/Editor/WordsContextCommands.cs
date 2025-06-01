using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class WordsContextCommands
{
    [MenuItem("CONTEXT/TextAsset/SortRussiansByFrequency")]
    static void SortRussiansByFrequency(MenuCommand command)
    {
        TextAsset selectedFile = command.context as TextAsset;
        if (selectedFile == null) 
            return;

        var frequencies =  File.ReadAllText("Assets/Content/Words/Frequency/russian_frequency_words.txt");
        SortByFrequency(frequencies, selectedFile);
    }

    [MenuItem("CONTEXT/TextAsset/SortEnglishByFrequency")]
    static void SortEnglishByFrequency(MenuCommand command)
    {
        TextAsset selectedFile = command.context as TextAsset;
        if (selectedFile == null) 
            return;

        var frequencies =  File.ReadAllText("Assets/Content/Words/Frequency/english_frequency_words.txt");
        SortByFrequency(frequencies, selectedFile);
    }

    private static void SortByFrequency(string frequencies, TextAsset selectedFile)
    {
        var frequenciesList = frequencies.Split('\n').ToList();

        string filePath = AssetDatabase.GetAssetPath(selectedFile);
        var words = File.ReadAllText(filePath).Split('\n').ToList();

        words.Sort((a, b) =>
        {
            var resA = a.Length;
            var resB = b.Length;
            if (resA != resB) return resA - resB;
            var indexA = frequenciesList.IndexOf(a.Trim());
            var indexB = frequenciesList.IndexOf(b.Trim());
            if (indexA == -1) resA += 2500; else resA += indexA;
            if (indexB == -1) resB += 2500; else resB += indexB;
            return resA - resB;
        });
        File.WriteAllText(filePath, string.Join("\n", words));
    }
    
    [MenuItem("CONTEXT/TextAsset/RemoveDublicates")]
    static void RemoveDublicates(MenuCommand command)
    {
        TextAsset selectedFile = command.context as TextAsset;
        if (selectedFile == null) 
            return;

        string filePath = AssetDatabase.GetAssetPath(selectedFile);
        var words = File.ReadAllText(filePath).Split('\n').ToList();
        words = words.Distinct().ToList();

        File.WriteAllText(filePath, string.Join("\n", words));
    }
    
    [MenuItem("CONTEXT/TextAsset/CreateLevelsBlock")]
    static void CreateLevelsBlock(MenuCommand command)
    {
        var text = command.context;
        TextAsset selectedFile = text as TextAsset;
        if (selectedFile == null) 
            return;

        string filePath = AssetDatabase.GetAssetPath(selectedFile);
        string fileContent = File.ReadAllText(filePath);
        var strings = fileContent.Split('\n');

        var buffLevelWords = new StringBuilder();
        var buffLevelWordsCount = 0;
        var buffLevelsBlock = new StringBuilder();
        var buffLevelsBlockCount = 0;
        var blocksCount = 0;
        foreach (var str in strings)
        {
            if (buffLevelWordsCount >= 4)
            {
                if (buffLevelsBlockCount >= 40)
                {
                    File.WriteAllText($"Assets/Content/Words/WordsForLevels/LevelsBlock_{++blocksCount}.json", buffLevelsBlock.ToString());
                    buffLevelsBlockCount = 0;
                    buffLevelsBlock.Clear();
                }

                buffLevelsBlock.Append($",\n");
                buffLevelsBlock.Append(CreateLevel(buffLevelWords.ToString()));
                buffLevelsBlockCount += 1;
                buffLevelWordsCount = 0;

                buffLevelWords.Clear();
            }

            buffLevelWordsCount += 1;
            buffLevelWords.Append($"{str.Trim()}\n");
        }

        //File.WriteAllText($"Assets/Content/Words/WordsForLevels/LevelsBlock_{i}.json", sb.ToString());
    }


    private static string CreateLevel(string words)
    {
        var nouns = words.Split('\n');

        var sb = new StringBuilder();
        for (var i = 0 ; i < nouns.Length; i++)
        {
            var noun = nouns[i].Trim();
            sb.Append($"\"{noun.ToUpper()}\": [{GetClusters(nouns[i])}]\n");
        }

        var allWordClusters = sb.ToString().Split('\n');
        sb.Clear();
        sb.Append("{\n");
        sb.Append($"\t\"Words\":\n");
        sb.Append("\t{\n");
        for (var j = 0; j < 4; j++)
        {
            var end = j == 3 ? "" : ",";
            sb.Append($"\t\t{allWordClusters[j]}{end}\n");
        }
        sb.Append("\t}\n");
        sb.Append("}");

        return sb.ToString();
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
    
    // [MenuItem("CONTEXT/TextAsset/LeaveOnlyWords")]
    // static void LeaveOnlyWords(MenuCommand command)
    // {
    //     Debug.Log(command.userData);
    //     var text = command.context;
    //     TextAsset selectedFile = text as TextAsset;
    //     if (selectedFile == null) 
    //         return;
    //
    //     Debug.LogError(AssetDatabase.GetAssetPath(selectedFile));
    //     
    //     var sb = new StringBuilder();
    //     string filePath = AssetDatabase.GetAssetPath(selectedFile);
    //     string fileContent = File.ReadAllText(filePath);
    //     var strings = fileContent.Split('\n');
    //     foreach (var str in strings)
    //     {
    //         // for english words
    //         var parts = str.Split(',');
    //         if (parts.Length < 2) continue;
    //         var word = parts[1];
    //         if (word.Length < 5 || word.Length > 7) continue;
    //         sb.Append($"{word}\n");     
    //         
    //         // for russians words
    //         // var parts = str.Split(' ');
    //         // if (parts.Length < 2) continue;
    //         // var word = parts[2];
    //         // if (word.Length < 5 || word.Length > 7) continue;
    //         // sb.Append($"{word}\n");
    //     }
    //
    //     File.WriteAllText(filePath, sb.ToString());
    // }

}