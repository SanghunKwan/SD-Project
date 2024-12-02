using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public abstract class JsonSaveLoad : JsonLoad
{
    #region ������ ����

    protected void SaveData<T>(T data, in string fileName) where T : class
    {
        CreateDataFolder("save");
        string wnth = Path.Combine(Application.dataPath, "save", fileName + ".json");
        File.WriteAllText(wnth, JsonUtility.ToJson(data, true));
    }
    protected void CreateDataFolder(in string folderName)
    {
        Directory.CreateDirectory(Path.Combine(Application.dataPath, folderName));
    }
    #endregion  
    #region ������ ����
    protected void DeleteSave(in string fileName)
    {
        string wnth = Path.Combine(Application.dataPath, "save", fileName + ".json");
        File.Delete(wnth);
#if UNITY_EDITOR
        File.Delete(wnth + ".meta");
#endif
    }
    #endregion

}
