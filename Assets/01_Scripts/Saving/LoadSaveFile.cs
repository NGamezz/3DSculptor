using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class LoadSaveFile : MonoBehaviour
{

    public static T LoadFileAsync<T>()
    {
        var bFormatter = new BinaryFormatter();

        var options = UnityEditor.EditorUtility.OpenFilePanel("Test", Application.persistentDataPath, "save");
        if (options.Length == 0)
        {
            Debug.LogWarning("Failed to start Panel, or cancelled.");
            return default;
        }

        var data = default(T);
        FileStream stream = null;
        try
        {
            if (!File.Exists(options)) { throw new IOException("File Doesn't Exist."); }

            stream = File.OpenRead(options);
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

        return data;
    }
}
