using System.Collections.Generic;
using System.IO;
using EasyLoc;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu]
public class CatTipsTextSO : LocalizableSO
{
    public List<string> TextList;
    public override bool Localize(Language language)
    {
        if (localizationCSV == null)
        {
            return false;
        }
        string[] lines = localizationCSV.text.Split('\n');
        int requiredColumn = -1;
        string[] headers = lines[0].Split(';');
        for (int i = 0; i < headers.Length; i++)
        {
            if (headers[i].Contains("_"+language))
            {
                requiredColumn = i;
                break;
            }
        }
        if (requiredColumn < 1)
        {
            return false;
        }
        TextList.Clear();
        for (int i = 1; i < lines.Length; i++)
        {
            string[] data = lines[i].Split(';');
            if (data[0].StartsWith(name))
            {
                TextList.Add(data[requiredColumn]);
            }
        }
        return true;
    }

#if UNITY_EDITOR
    [ContextMenu("Export all tip providers")]
    public void ExportTips()
    {
        string path = "/Localize/Tips.csv";
        var file = File.CreateText(Application.dataPath+path);
        file.WriteLine("id;_RU;_EN");
        var tips = Resources.FindObjectsOfTypeAll<CatTipsTextSO>();
        foreach (var tipSet in tips)
        {
            Debug.Log($"exporting {tipSet.name}...");
            for (var index = 0; index < tipSet.TextList.Count; index++)
            {
                var tip = tipSet.TextList[index];
                file.WriteLine($"{tipSet.name}.{index};{tip}");
            }
        }
        file.Close();
        Debug.Log("Done! File saved at "+path);
        foreach (var textSo in tips)
        {
            if (!textSo.localizationCSV)
            {
                textSo.localizationCSV = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets"+path);
                EditorUtility.SetDirty(textSo);
            }
        }
    }
    #endif
}
