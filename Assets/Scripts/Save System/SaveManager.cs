using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


// Các chức năng lưu và tải
public static class SaveManager
{
    #region Game Data

    
    // Lưu dữ liệu game vào file
   
    /// <param name="saveSlot">Slot game từ 0->3</param>
    /// <param name="dataRaw">Dữ liệu thô của game cần lưu</param>
    public static void SaveGame(int saveSlot, SaveDataRaw dataRaw)
    {
        // 

        SaveData data = new SaveData(dataRaw);

        // Tạo serializable file từ dữ liệu thô

        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/AutoSave.sav";

        switch (saveSlot)
        {
            case 1:
                path = Application.persistentDataPath + "/Save1.sav";
                break;
            case 2:
                path = Application.persistentDataPath + "/Save2.sav";
                break;
            case 3:
                path = Application.persistentDataPath + "/Save3.sav";
                break;
        }

        FileStream fileStream = new FileStream(path, FileMode.Create);

        formatter.Serialize(fileStream, data);

        fileStream.Close();

        // Cập nhật slot game lưu với ngày và giờ của lần lưu mới.

        Interface.interfaceClass.UpdateSaveDates();
    }

    
    // Xóa AutoSave
   
    public static void DeleteAutoSave()
    {
        string path = Application.persistentDataPath + "/AutoSave.sav";

        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    
    // Tải dữ liệu hiện có trong file 
   
    /// <param name="saveSlot">Slot game từ 0->3</param>
    public static SaveData LoadGame(int saveSlot)
    {
        // Lưu theo "path"

        string path = Application.persistentDataPath + "/AutoSave.sav";

        switch (saveSlot)
        {
            case 1:
                path = Application.persistentDataPath + "/Save1.sav";
                break;
            case 2:
                path = Application.persistentDataPath + "/Save2.sav";
                break;
            case 3:
                path = Application.persistentDataPath + "/Save3.sav";
                break;
        }

        // Ghi đè nếu đã tồn tại

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SaveData data = formatter.Deserialize(stream) as SaveData;
            stream.Close();

            return data;
        }

        return null;
    }

    #endregion

    #region Settings

    
    // Lưu cài đặt vào tệp
       public static void SaveSettings(SettingsData data)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/Settings.sav";

        FileStream fileStream = new FileStream(path, FileMode.Create);

        formatter.Serialize(fileStream, data);

        fileStream.Close();
    }

    
    // Tải cài đặt trò chơi đã lưu trước đó
       public static SettingsData LoadSettings()
    {
        SettingsData data;

        string path = Application.persistentDataPath + "/Settings.sav";

        // Nếu đã có => tải nó

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            data = formatter.Deserialize(stream) as SettingsData;
            stream.Close();
        }

        // Nếu chưa có => sử dụng theo mặc định

        else
        {
            Options.DefaultValues();

            return LoadSettings();
        }

        return data;
    }

    
    // Lấy thông tin ngày và giờ
    public static string[] GetDates()
    {
        string[] dates = new string[4];

        SaveData data;

        string path = Application.persistentDataPath + "/AutoSave.sav";

        // Nếu đã có => lưu dữ liệu vào mảng

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            data = formatter.Deserialize(stream) as SaveData;
            stream.Close();

            dates[0] = data.saveDate;
        }

        // Nếu ko có => lưu giả tri "0" để giữ chỗ

        else
        {
            dates[0] = "0";
        }

        // Lặp lại với các vị trí lưu

        path = Application.persistentDataPath + "/Save1.sav";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            data = formatter.Deserialize(stream) as SaveData;
            stream.Close();

            dates[1] = data.saveDate;
        }

        else
        {
            dates[1] = "0";
        }

        path = Application.persistentDataPath + "/Save2.sav";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            data = formatter.Deserialize(stream) as SaveData;
            stream.Close();

            dates[2] = data.saveDate;
        }

        else
        {
            dates[2] = "0";
        }

        path = Application.persistentDataPath + "/Save3.sav";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            data = formatter.Deserialize(stream) as SaveData;
            stream.Close();

            dates[3] = data.saveDate;
        }

        else
        {
            dates[3] = "0";
        }

        return dates;
    }

    #endregion

    #region Photon Serialization

    
    // Serializes dữ liệu qua máy chủ Photon
   
    /// <param name="data">Dữ liệu được Serializes</param>
    /// <returns>Dữ liệu ở dạng mảng byte</returns>
    public static byte[] Serialize(object data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream();

        formatter.Serialize(stream, data);

        return stream.ToArray();
    }

    
    // Deserializes mảng byte để có thể đọc
   
    /// <param name="byteData">Dữ liệu ở dạng mảng byte</param>
    /// <returns></returns>
    public static SaveData Deserialize(byte[] byteData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream(byteData);

        return formatter.Deserialize(stream) as SaveData;
    }

    #endregion
}