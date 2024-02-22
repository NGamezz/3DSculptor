using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class LoadSaveFile
{
    private static void HandleLoad<T>(string[] path, Action<T> uponRetrieval)
    {
        var bFormatter = new BinaryFormatter();
        var data = default(T);
        FileStream stream = null;
        try
        {
            if (!File.Exists(path[0])) { throw new IOException("File Doesn't Exist."); }

            stream = File.OpenRead(path[0]);
            data = (T)bFormatter.Deserialize(stream);
        }
        catch (IOException e)
        {
            Debug.Log(e.Message);
        }
        finally
        {
            if (stream != null)
            {
                stream.Flush();
                stream.Close();
            }
        }
        uponRetrieval?.Invoke(data);
    }

    public static void LoadFileAsync<T>(Action<T> uponRetrieval)
    {

        SimpleFileBrowser.FileBrowser.ShowLoadDialog((path) => HandleLoad<T>(path, uponRetrieval), null, SimpleFileBrowser.FileBrowser.PickMode.Files, false, Application.persistentDataPath);
    }
}
