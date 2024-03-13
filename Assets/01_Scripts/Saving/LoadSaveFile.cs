using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using Unity.Collections;
using UnityEngine;

public class LoadSaveFile
{
    private static void HandleLoad<T>(string[] path, Action<byte[]> uponRetrieval)
    {
        //var xmlFormatter = new XmlSerializer(typeof(SaveData<byte[]>));
        //var bFormatter = new BinaryFormatter();
        var xmlFormatter = new XmlSerializer(typeof(SaveData<T>));
        byte[] data;
        FileStream stream = null;
        try
        {
            if (!File.Exists(path[0])) { throw new IOException("File Doesn't Exist."); }

            stream = File.Open(path[0], FileMode.Open);

            var saveData = (SaveData<byte[]>)xmlFormatter.Deserialize(stream);

            data = saveData.data;
            //data = File.ReadAllBytes(path[0]);

            uponRetrieval?.Invoke(data);
            //stream = File.OpenRead(path[0]);
            //data = (T)bFormatter.Deserialize(stream);
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
    }

    //private static void HandleLoad<T>(string[] path, Action<T> uponRetrieval)
    //{
    //    var bFormatter = new BinaryFormatter();
    //    var data = default(T);
    //    FileStream stream = null;
    //    try
    //    {
    //        if (!File.Exists(path[0])) { throw new IOException("File Doesn't Exist."); }

    //        byte[] bytes = File.ReadAllBytes(path[0]);

    //        //stream = File.OpenRead(path[0]);
    //        //data = (T)bFormatter.Deserialize(stream);
    //    }
    //    catch (IOException e)
    //    {
    //        Debug.Log(e.Message);
    //    }
    //    finally
    //    {
    //        if (stream != null)
    //        {
    //            stream.Flush();
    //            stream.Close();
    //        }
    //    }
    //    uponRetrieval?.Invoke(data);
    //}

    public static void LoadFileAsync<T>(Action<byte[]> uponRetrieval)
    {
        SimpleFileBrowser.FileBrowser.ShowLoadDialog((path) => HandleLoad<T>(path, uponRetrieval), null, SimpleFileBrowser.FileBrowser.PickMode.Files, false, Application.persistentDataPath);
    }
}
