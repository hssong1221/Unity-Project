using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class CommonFunction
{
    // InputField 초기화
    public static void clear(this InputField inputfield)
    {
        inputfield.Select();
        inputfield.text = "";
    }
}
