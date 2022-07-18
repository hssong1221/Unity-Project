using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;


/*
* 런처는 따로 기능은 없고 바로 다음 씬인 로비로 넘어갈 예정 
*/

public class Launcher : MonoBehaviour
{
    public GameObject BackgroundMusic;
    private AudioSource bgm;

    void Awake()
    {
        // 로비 재생 음악
        bgm = BackgroundMusic.GetComponent<AudioSource>();

        bgm.Play();
    }
    void Start()
    {
        SceneManager.LoadScene("Login");

        DontDestroyOnLoad(BackgroundMusic);
    }

}
