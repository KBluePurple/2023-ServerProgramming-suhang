using System.Collections.Generic;
using UnityEngine;

public class UI_Chat : UI_Scene
{
    [SerializeField] private ChatElement chatElementPrefab;
    [SerializeField] private int maxChatCount = 10;
    private readonly List<ChatElement> _chatElements = new();

    public override void Init()
    {
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
}