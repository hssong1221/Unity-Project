using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
public class Launcher : MonoBehaviour
{
    public GameObject BackgroundMusic;
    private AudioSource bgm;

    void Awake()
    {
        bgm = BackgroundMusic.GetComponent<AudioSource>();

        bgm.Play();
        DontDestroyOnLoad(BackgroundMusic);
    }
    void Start()
    {
        SceneManager.LoadScene("Login");
    }

}
