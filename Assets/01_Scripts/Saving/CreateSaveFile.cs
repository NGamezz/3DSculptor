using System;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class SaveData<T, U>
{
    public int buildVersion = 0;
    public int saveVersion = 0;
    public T data;
    public U dataB;
}

public struct VersionData
{
    public int version;
    public bool overwrite;

    public VersionData ( int version, bool overwrite )
    {
        this.version = version;
        this.overwrite = overwrite;
    }
}

public class CreateSaveFile
{
    //Increase index if file exists, until index reaches max amount, then overwrite first file and continue as before.
    public static void SaveToFile<T, U> ( ref SaveData<T, U> thingToSave, string path )
    {
        var bFormatter = new BinaryFormatter();

        string cachedPath = path;

        var version = 0;

        path += $"-{version}.save";

        Debug.Log(version);

        FileStream stream = null;
        try
        {
            if ( !File.Exists(path) )
            {
                DataHolder.SaveVersion = 0;

                thingToSave.saveVersion = 0;

                stream = File.Create(path);
                bFormatter.Serialize(stream, thingToSave);
                Debug.Log("Saved.");
            }
            else
            {
                cachedPath += $"-{version + 1}.save";
                thingToSave.saveVersion = version + 1;

                stream = File.Create(path);
                bFormatter.Serialize(stream, thingToSave);
            }
        }
        catch ( Exception e )
        {
            Debug.LogException(e);
        }
        finally
        {
            if ( stream != null )
            {
                stream.Flush();
                stream.Close();
            }
        }
    }
}