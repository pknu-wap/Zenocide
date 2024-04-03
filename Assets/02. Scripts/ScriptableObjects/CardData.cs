// ±ËπŒ√∂
using UnityEngine;

[CreateAssetMenu(fileName = "Card Data", menuName = "Scriptable Objects/Card Data", order = 0)]
public class CardData : ScriptableObject
{
    public new string name;
    [TextArea(3, 5)]
    public string description;
    public Sprite sprite;

    public int cost;
    public int attack;
    public int defense;
}
