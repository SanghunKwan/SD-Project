using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public float[] saved = new float[(int)SoundWindow.SoundType.MAX] { 100, 100, 100, 100, 100 };

    Dictionary<Transform, AudioSource>[] SourceDictionary = new Dictionary<Transform, AudioSource>[(int)DictionaryType.MAX];
    List<AudioSource> restSourceList = new List<AudioSource>();

    Action<DictionaryType, AudioClip>[] dicTypeAction = new Action<DictionaryType, AudioClip>[2];
    public enum DictionaryType
    {
        BGM,
        FX,
        VOICE,
        INPUTFX,
        MAX
    }


    private void Awake()
    {
        for (int i = 0; i < SourceDictionary.Length; i++)
        {
            SourceDictionary[i] = new Dictionary<Transform, AudioSource>();
        }

    }
    private void Start()
    {
        GameManager.manager.soundManager = this;
        AudioSource bgmSource = transform.GetChild(0).GetComponent<AudioSource>();
        SourceDictionary[(int)DictionaryType.BGM].Add(bgmSource.transform, bgmSource);

        dicTypeAction[0] = NewCompPlay;
        dicTypeAction[1] = OldCompPlay;
    }
    public void VolumeChange(SoundWindow.SoundType type, float fNum)
    {
        int DicIndex = (int)type - 1;

        foreach (var item in SourceDictionary[DicIndex].Values)
        {
            item.volume = fNum;
        }

    }

    public void PlayNewSource(DictionaryType type, AudioClip clip)
    {
        bool isOne = restSourceList.Count != 0;
        dicTypeAction[Convert.ToInt32(isOne)](type, clip);
    }

    void NewCompPlay(DictionaryType type, AudioClip clip)
    {
        GameObject obj = new GameObject("soundPool");
        AudioSource source = obj.AddComponent<AudioSource>();
        source.transform.SetParent(transform);
        source.clip = clip;
        StartCoroutine(SourceStopDelay(type, source, clip));
    }
    void OldCompPlay(DictionaryType type, AudioClip clip)
    {

        AudioSource source = restSourceList[restSourceList.Count - 1];
        source.clip = clip;

        restSourceList.RemoveAt(restSourceList.Count - 1);

        StartCoroutine(SourceStopDelay(type, source, clip));

    }

    IEnumerator SourceStopDelay(DictionaryType type, AudioSource source, AudioClip clip)
    {


        SourceDictionary[(int)type].Add(source.transform, source);
        source.clip = clip;
        source.volume = saved[(int)SoundWindow.SoundType.Main] * saved[(int)type + 1] / 10000;

        source.Play();
        yield return new WaitForSecondsRealtime(clip.length);
        source.Stop();

        SourceDictionary[(int)type].Remove(source.transform);
        restSourceList.Add(source);
        source.clip = null;
    }
}
