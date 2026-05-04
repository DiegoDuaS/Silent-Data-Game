using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapLoader 
{   
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        if(Object.FindFirstObjectByType<Bootstraper>() != null)
            return;
        
        SceneManager.LoadScene("Bootstrap", LoadSceneMode.Additive);
    }
}
