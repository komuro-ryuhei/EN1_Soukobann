using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{

    // AudioSource コンポーネントを格納するための変数
    private AudioSource audioSource;

    public AudioSource bgmSource;

    // BGMを再生する
    public void PlayBGM()
    {
        if (!bgmSource.isPlaying)
        {
            bgmSource.Play();
        }
    }

    // BGMを停止する
    public void StopBGM()
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Stop();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // AudioSource コンポーネントを取得する
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayBGM();
    }
}
