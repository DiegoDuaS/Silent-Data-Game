using UnityEngine;

public class Gun : Pickup
{
    public override void Use()
    {
        EventLevel1Manager.TriggerGunCollected();
    }
}