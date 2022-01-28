using UnityEngine;
using UnityEngine.UI;

public class UIText : Text
{
    [SerializeField] private bool m_DisableWordWrap;
    // Text 상속받은 커스텀 UIText
    // 본래 텍스트가 MaxWidth일때 단어 단위로 줄바꿈되지만
    // 이 UIText는 한 음? 단위로 줄바꿈됨
    public override string text
    {
        get => base.text;
        set
        {
            if (m_DisableWordWrap)
            {
                string nsbp = value.Replace(' ', '\u00A0');
                base.text = nsbp;
                return;
            }
            base.text = value;
        }
    }
}