using UnityEngine;

public class Phone : Pickup
{
    public override void Use()
    {
        EventLevel1Manager.TriggerPhoneCollected();
    }
}