using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class DescriptionWindow : MonoBehaviour, IWindowSet
{
    [SerializeField] SettingDescManager settingDescManager;
    AddressableManager addressableManager;
    VideoPlayer videoPlayer;
    TextMeshProUGUI text;
    RawImage texture;
    Button[] button = new Button[2];
    Button playButton;


    int m_page = 1;
    AddressableManager.Video pageVideo;

    Dictionary<bool, Action> toggle = new Dictionary<bool, Action>();

    private void Awake()
    {
        addressableManager = AddressableManager.manager;
        videoPlayer = transform.GetChild(1).GetComponent<VideoPlayer>();
        button = transform.Find("PageButton").GetComponentsInChildren<Button>();
        text = videoPlayer.transform.Find("descText").GetComponent<TextMeshProUGUI>();
        playButton = videoPlayer.transform.GetChild(0).Find("Loaded").GetComponent<Button>();
        texture = videoPlayer.transform.GetChild(0).GetComponent<RawImage>();
        GoPage(0);

        toggle.Add(true, videoPlayer.Play);
        toggle.Add(false, videoPlayer.Pause);

    }

    public void GoPage(int num)
    {
        m_page += num;
        text.text = settingDescManager.readContents.contents[m_page].text;

        pageVideo = (AddressableManager.Video)m_page;

        button[0].interactable = m_page > 1;
        button[1].interactable = m_page < settingDescManager.readContents.contents.Length - 1;

        OnDisable();


    }
    public void LoadVideo()
    {
        LoadVideo(pageVideo);

    }
    void LoadVideo(in AddressableManager.Video video)
    {
        Action<VideoClip> action = (vClip) =>
            {
                videoPlayer.clip = vClip;
                texture.enabled = true;
                playButton.gameObject.SetActive(false);
            };

        addressableManager.LoadVideo(video.ToString(), action);
    }
    public void TogglePlay()
    {
        toggle[videoPlayer.isPaused]();
    }
    private void OnDisable()
    {
        texture.enabled = false;
        videoPlayer.Stop();
        videoPlayer.clip = null;
        playButton.gameObject.SetActive(true);
    }

    public void SaveValue()
    {
    }

    public void RevertValue()
    {
    }
}
