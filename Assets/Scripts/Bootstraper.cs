using UnityEngine;

public class Bootstraper : MonoBehaviour
{
    public static Bootstraper instance;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(instance);
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }
}
