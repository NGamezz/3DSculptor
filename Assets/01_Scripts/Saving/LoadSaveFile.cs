using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class LoadSaveFile
{
    private static void HandleLoad<T, U> ( string[] path, Action<SaveData<T, U>> uponRetrieval )
    {
        var bFormatter = new BinaryFormatter();
        FileStream stream = null;
        try
        {
            if ( !File.Exists(path[0]) )
            { throw new IOException("File Doesn't Exist."); }

            stream = File.Open(path[0], FileMode.Open);

            var saveData = (SaveData<T, U>)bFormatter.Deserialize(stream);

            DataHolder.SaveVersion = saveData.saveVersion;

            uponRetrieval?.Invoke(saveData);
        }
        catch ( IOException e )
        {
            Debug.Log(e.Message);
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

    public static void LoadFileAsync<T, U> ( Action<SaveData<T, U>> uponRetrieval )
    {
        SimpleFileBrowser.FileBrowser.ShowLoadDialog(( path ) => HandleLoad(path, uponRetrieval), () => EventManager<TextPopup>.InvokeEvent(new(2, "Cancelled Load."), EventType.OnQueuePopup), SimpleFileBrowser.FileBrowser.PickMode.Files, false, Application.persistentDataPath);
    }
}
