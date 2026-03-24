using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    private static string savePath => Application.persistentDataPath + "/save.dat";

    public static void SaveProgress(int maxUnlockedLevel)
    {
        SaveData data = new SaveData
        {
            maxUnlockedLevel = maxUnlockedLevel
        };

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(savePath, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();

        Debug.Log($"Сохранено. Разблокировано уровней: {maxUnlockedLevel}");
    }

    public static int LoadProgress()
    {
        if (File.Exists(savePath))
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(savePath, FileMode.Open);

                SaveData data = formatter.Deserialize(stream) as SaveData;
                stream.Close();

                Debug.Log($"Загружено. Разблокировано уровней: {data.maxUnlockedLevel}");
                return data.maxUnlockedLevel;
            }
            catch
            {
                Debug.Log("Ошибка загрузки сохранения");
                return 1;
            }
        }
        else
        {
            Debug.Log("Файл сохранения не найден");
            return 1;
        }
    }

    public static void ResetProgress()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
        SaveProgress(1);
    }
}

[System.Serializable]
public class SaveData
{
    public int maxUnlockedLevel;
}