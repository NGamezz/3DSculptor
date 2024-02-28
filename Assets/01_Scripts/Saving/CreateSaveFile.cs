using System;
using System.IO;
using UnityEngine;
using Unity.Mathematics;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveData<T>
{
    public T data;
}

[Serializable]
public class VoxelHolder
{
    public int version;
    public float3[] voxels;
}

public class CreateSaveFile
{
    public static void SaveToFile<T>(T thingToSave, int version, string path)
    {
        //var path = Path.Combine(Application.persistentDataPath, $"{fileName}-{version}.save");
        var bFormatter = new BinaryFormatter();

        path += $"-{version}.save";

        FileStream stream = null;
        try
        {
            if (!File.Exists(path))
            {
                stream = File.Create(path);
                bFormatter.Serialize(stream, thingToSave);
                Debug.Log("Saved.");
            }
            else
            {
                stream = File.Open(path, FileMode.Open);
                stream.Flush();
                Debug.Log("File Exists.");
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        finally
        {
            if (stream != null)
            {
                stream.Flush();
                stream.Close();
            }
        }
    }
}