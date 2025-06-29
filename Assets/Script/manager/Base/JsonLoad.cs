using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public abstract class JsonLoad : InitObject
{
    protected enum SaveFileType
    {
        save,
        setting
    }
    #region 데이터 로드
    protected T LoadData<T>(in string fileName) where T : class
    {
        string readJson = File.ReadAllText(TablePath(fileName));
        return JsonUtility.FromJson<T>(readJson);
    }
    protected string TablePath(in string fileName)
    {
        return Path.Combine(GetFolder("DataTable"), fileName + ".json");
    }
    #endregion
    #region 세이브 로드
    protected T LoadSave<T>(string pathFile) where T : class
    {
        string readJson = File.ReadAllText(pathFile);
        return JsonUtility.FromJson<T>(readJson);
    }
    protected string SavePath(int index, SaveFileType type)
    {
        return Path.Combine(GetFolder("save"), type.ToString() + (index + 1).ToString() + ".json");
    }
    protected string SavePath(SaveFileType type) => Path.Combine(GetFolder("save"), type.ToString() + ".json");
    #endregion
    #region 세이브 데이터 확인
    protected bool SaveDataExist(int index, SaveFileType type)
    {
        return File.Exists(SavePath(index, type));
    }
    protected bool SaveDataExist(SaveFileType type)
    {
        return File.Exists(SavePath(type));
    }
    protected void SaveDataFloor(int index, out int floor, out int day)
    {
        string[] readJson = new string[2];
        int[] intJson = new int[2];
        using (StreamReader sr = new StreamReader(SavePath(index, SaveFileType.save)))
        {
            sr.ReadLine();
            readJson[0] = sr.ReadLine();
            sr.ReadLine();
            sr.ReadLine();
            readJson[1] = sr.ReadLine();
        }
        for (int i = 0; i < intJson.Length; i++)
        {
            intJson[i] = int.Parse(readJson[i].Split(":")[1].Trim(','));
        }
        floor = intJson[0];
        day = intJson[1];
    }
    #endregion
    #region 경로
    protected void CreateFolder(in string folderName)
    {
        Directory.CreateDirectory(GetFolder(folderName));
    }
    protected string GetFolder(in string folderName)
    {
        return Path.Combine(Application.streamingAssetsPath, folderName);
    }
    #endregion
    public void SDF<T>() where T : class, new()
    {
        T asdf = new T();

        string wnth = Path.Combine(Application.streamingAssetsPath, "DataTable/asdf.json");

        File.WriteAllText(wnth, JsonUtility.ToJson(asdf, true));

    }

}
