using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneBooter : MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadScene(1);
    }
}