using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Data
{
    // 체력
    public int hp;
    // 덱
    public List<CardData> deck;
    // 직업
    public string job;
    // 현재 이벤트
    public EventData currentEvent;
    // 이벤트 리스트
    public List<EventData> processableMainEventList;
    public List<EventData> processableSubEventList;
    // 딜레이 딕셔너리
    public Dictionary<EventData, int> delayDictionary;
    // 아이템 인벤토리
    public List<Item> items = new List<Item>();
    //옵션
    public int selectedResolutionIndex;
    public bool fullscreen;
    public float masterVolume;
    public float bgmVolume;


    public int Hp
    {
        get { return hp; }
        set { hp = value; }
    }

    public List<CardData> Deck
    {
        get { return deck; }
        set { deck = value; }
    }

    public EventData CurrentEvent
    {
        get { return currentEvent; }
        set { currentEvent = value; }
    }

    public List<EventData> ProcessableMainEventList
    {
        get { return processableMainEventList; }
        set { processableMainEventList = value; }
    }

    public List<EventData> ProcessableSubEventList
    {
        get { return processableSubEventList; }
        set { processableSubEventList = value; }
    }

    public Dictionary<EventData, int> DelayDictionary
    {
        get { return delayDictionary; }
        set { delayDictionary = value; }
    }

    public List<Item> Items
    {
        get { return items; }
        set { items = value; }
    }

    public int SelectedResolutionIndex
    {
        get { return selectedResolutionIndex; }
        set { selectedResolutionIndex = value; }
    }

    public bool Fullscreen
    {
        get { return fullscreen; }
        set {  fullscreen = value; }
    }

    public float MasterVolume
    {
        get { return masterVolume; }
        set {  masterVolume = value; }
    }

    public float BgmVolume
    {
        get { return bgmVolume; }
        set { bgmVolume = value; }
    }
}
