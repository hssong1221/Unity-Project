using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
public class Launcher : MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadScene("Login");
    }

}
