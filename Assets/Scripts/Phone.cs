using UnityEngine;

public class Phone : Pickup
{
    public override void Use()
    {
        EventManager.TriggerPhoneCollected();
    }
}