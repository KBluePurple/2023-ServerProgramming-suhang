using UnityEngine.UI;

public class ChatElement : UI_Base
{
    private Text _text;

    public override void Init()
    {
        _text = GetComponent<Text>();
    }

    public void SetText(string message)
    {
        _text.text = message;
    }
}