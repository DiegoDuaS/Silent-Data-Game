using UnityEngine;

public class Medkit : Pickup
{
    [SerializeField] private int saludRecuperada = 25;

    public override void Use()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.ModifyHealth(saludRecuperada);
        }
    }
}