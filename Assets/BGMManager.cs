using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{

    // AudioSource �R���|�[�l���g���i�[���邽�߂̕ϐ�
    private AudioSource audioSource;

    public AudioSource bgmSource;

    // BGM���Đ�����
    public void PlayBGM()
    {
        if (!bgmSource.isPlaying)
        {
            bgmSource.Play();
        }
    }

    // BGM���~����
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
        // AudioSource �R���|�[�l���g���擾����
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayBGM();
    }
}
