using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "SilentData/PickupData")]
public class PickupData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
}
