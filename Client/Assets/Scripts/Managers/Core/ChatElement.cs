using System;
using UnityEngine;
using UnityEngine.UI;

public class ChatElement : MonoBehaviour
{
    private Text _text;

    private void Awake()
    {
        _text = GetComponent<Text>();
    }

    public void SetText(string message)
    {
        _text.text = message;
    }
}