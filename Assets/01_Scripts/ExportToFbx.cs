using UnityEngine;
using UnityEditor.Formats.Fbx.Exporter;
using System.IO;
using System;

public class ExportToFbx
{
    public static void Export(UnityEngine.Object[] items, string fileName, Action ifExistsCallback)
    {
        var path = Path.Combine(Application.dataPath, fileName + ".fbx");

        if (File.Exists(path))
        {
            ifExistsCallback?.Invoke();
            return;
        }

        var filePath = Path.Combine(Application.dataPath, fileName);
        ExportModelOptions options = new()
        {
            PreserveImportSettings = true,
            ExportFormat = ExportFormat.Binary
        };
        var result = ModelExporter.ExportObjects(filePath, items, options);
        Debug.Log(result);
    }
}