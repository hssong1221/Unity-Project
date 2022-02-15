using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class CommonFunction
{
    // InputField 초기화
    public static void clear(this InputField inputfield)
    {
        inputfield.Select();
        inputfield.text = "";
    }

    public static void clear(this TMP_InputField inputfield)
    {
        inputfield.Select();
        inputfield.text = "";
    }
}
