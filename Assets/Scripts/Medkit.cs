using UnityEngine;

public class Medikit : Pickup
{
    [SerializeField] private int saludRecuperada = 25;

    public override void Use()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.ModificarSalud(saludRecuperada);
        }
    }
}