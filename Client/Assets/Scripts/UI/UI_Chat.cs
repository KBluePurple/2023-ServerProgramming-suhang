using System;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_Chat : UI_Scene
{
    [SerializeField] private ChatElement chatElementPrefab;
    [SerializeField] private InputField inputField;
    [SerializeField] private int maxChatCount = 10;
    
    private readonly List<ChatElement> _chatElements = new();

    public override void Init()
    {
        Managers.UI.SetChatUI(this);
    }

    public void AddChat(string message)
    {
        var chatElement = Instantiate(chatElementPrefab, transform);
        chatElement.gameObject.SetActive(true);
        chatElement.SetText(message);

        _chatElements.Add(chatElement);

        if (_chatElements.Count <= maxChatCount) return;

        var removeElement = _chatElements[0];
        _chatElements.RemoveAt(0);
        Managers.Resource.Destroy(removeElement.gameObject);
    }
    
    public void Chat(string message)
    {
        if (string.IsNullOrEmpty(message)) return;

        var packet = new C_Msg
        {
            Message = message
        };
        
        Managers.Network.Send(packet);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            inputField.Select();
            inputField.ActivateInputField();
            inputField.text = "";
        }
    }
}