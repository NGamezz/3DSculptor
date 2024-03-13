using System;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Xml;

[Serializable]
public class SaveData<T>
{
    public int buildVersion;
    public T data;
}

public class CreateSaveFile
{
    public static void SaveToFile<T>(T thingToSave, int version, string path)
    {
        //var path = Path.Combine(Application.persistentDataPath, $"{fileName}-{version}.save");

        //var bFormatter = new BinaryFormatter();

        var xmlFormatter = new XmlSerializer(thingToSave.GetType());

        path += $"-{version}.save";

        FileStream stream = null;
        try
        {
            if (!File.Exists(path))
            {
                stream = File.Create(path);
                //bFormatter.Serialize(stream, thingToSave);
                xmlFormatter.Serialize(stream, thingToSave);
                Debug.Log("Saved.");
            }
            else
            {
                stream = File.Open(path, FileMode.Truncate);
                xmlFormatter.Serialize(stream, thingToSave);
                //bFormatter.Serialize(stream, thingToSave);
                Debug.Log("File Overwritten.");
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