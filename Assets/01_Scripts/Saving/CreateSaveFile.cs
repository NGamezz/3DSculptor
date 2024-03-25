using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class SaveData<T, U>
{
    public int buildVersion = 0;
    public int saveVersion = 0;
    public T data;
    public U dataB;
}

public class CreateSaveFile
{
    //Todo : Increase index if file exists, until index reaches max amount, then overwrite first file and continue as before.
    public static void SaveToFile<T, U> ( SaveData<T, U> thingToSave, string path )
    {
        var bFormatter = new BinaryFormatter();

        var version = DataHolder.SaveVersion;
        DataHolder.SaveVersion += 1;

        path += $"-{version}.save";

        thingToSave.saveVersion = version;

        FileStream stream = null;
        try
        {
            if ( !File.Exists(path) )
            {
                stream = File.Create(path);
                bFormatter.Serialize(stream, thingToSave);

                EventManager<TextPopup>.InvokeEvent(new(4.0f, $"Saved File To : {path}"), EventType.OnQueuePopup);
            }
            else
            {
                stream = File.Open(path, FileMode.Truncate);
                bFormatter.Serialize(stream, thingToSave);

                EventManager<TextPopup>.InvokeEvent(new(4.0f, $"Saved File To : {path}"), EventType.OnQueuePopup);
                //DataHolder.TextPopupManager.QueuePopup(new(4.0f, $"Saved File To : {path}"));
            }
        }
        catch ( Exception e )
        {
            UnityEngine.Debug.LogException(e);
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