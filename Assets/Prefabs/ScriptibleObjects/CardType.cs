using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardTypeScriptibleObject", menuName = "CardType/CardTypeScriptibleObject")]
public class CardType : ScriptableObject
{
    [Header("Info")]
    public Sprite CardIcon;
    public int MaxCardNumber;
    public int setAmount;
}
