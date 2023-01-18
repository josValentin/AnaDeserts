using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static string SavedDataPath = Application.persistentDataPath + "/SaveData.dat";

    public static void SaveData(AppData appData)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        FileStream stream = new FileStream(SavedDataPath, FileMode.Create);

        formatter.Serialize(stream, appData);

        stream.Close();
    }


    public static AppData LoadData()
    {
        if (File.Exists(SavedDataPath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(SavedDataPath, FileMode.Open);

            try
            {
                AppData data = formatter.Deserialize(stream) as AppData;

                stream.Close();

                return data;
            }
            catch (System.Exception ex)
            {
                stream.Close();

                Debug.Log(ex);

                return null;
            }

        }
        else
        {
            return null;
        }
    }

    public static void DeleteData()
    {
        if (File.Exists(SavedDataPath))
        {
            File.Delete(SavedDataPath);
            Debug.Log("Data deleted");

        }
        else
        {
            Debug.Log("No data to delete");
        }
    }
}
