using System;
using System.IO;
using UnityEngine;
using Unity.Mathematics;

public class SaveData<T>
{
    public T data;
}

public class VoxelHolder
{
    public int version;
    public float3[] voxels;
}

public class CreateSaveFile<T>
{
    public void SaveToFile<T>(T thingToSave)
    {
        var json = JsonUtility.ToJson(thingToSave);
    }

    private SaveData<T> scoreSaveData = new();
    private string fileName;

    public CreateSaveFile(string fileName)
    {
        this.fileName = fileName;
    }

    public void SaveObjectToJson(T value)
    {
        scoreSaveData.data = value;

        string saveDataString = JsonUtility.ToJson(scoreSaveData);
        File.WriteAllText(Application.persistentDataPath + $"/{fileName}.json", saveDataString);
        Debug.Log("Saved Data");
    }

    public T ReturnSavedInt()
    {
        if (!File.Exists(Application.persistentDataPath + $"/{fileName}.json")) { return default; }

        string filePath = Path.Combine(Application.persistentDataPath + $"/{fileName}.json");
        string data = File.ReadAllText(filePath);
        SaveData<T> scoreSaveData = JsonUtility.FromJson<SaveData<T>>(data);

        return scoreSaveData.data;
    }
}