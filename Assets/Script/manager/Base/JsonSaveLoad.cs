using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public abstract class JsonSaveLoad : JsonLoad
{
    #region 데이터 저장

    protected void SaveData<T>(T data, in string fileName) where T : class
    {
        CreateFolder("save");
        string wnth = Path.Combine(GetFolder("save"), fileName + ".json");
        File.WriteAllText(wnth, JsonUtility.ToJson(data, true));
    }
    #endregion  
    #region 데이터 삭제
    protected void DeleteSave(in string fileName)
    {
        string wnth = Path.Combine(GetFolder("save"), fileName + ".json");
        File.Delete(wnth);
#if UNITY_EDITOR
        File.Delete(wnth + ".meta");
#endif
    }
    #endregion

}
