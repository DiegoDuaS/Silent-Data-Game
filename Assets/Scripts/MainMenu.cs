using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [Header("UI Containers")]
    [SerializeField] private GameObject mainMenuContainer;
    [SerializeField] private GameObject levelSelectContainer;


    public void OpenLevelSelect()
    {
        mainMenuContainer.SetActive(false);
        levelSelectContainer.SetActive(true);
    }

    public void CloseLevelSelect()
    {
        mainMenuContainer.SetActive(true);
        levelSelectContainer.SetActive(false);
    }

    // --- Lógica de Inicio ---

    public void StartNewGame()
    {
        Debug.Log("[MainMenu] Iniciando partida nueva...");
        GameManager.Instance.ChangeScene("Level1", 0);
    }

    public void ContinueGame()
    {
        Debug.Log("[MainMenu] Continuando partida...");
        GameManager.Instance.ChangeScene("Level1", 0);
    }

    // --- Selección Directa de Niveles ---

    public void SelectLevel1()
    {
        GameManager.Instance.ChangeScene("Level1", 0);
    }

    public void SelectLevel2()
    {
        GameManager.Instance.ChangeScene("Level2", 1);
    }

    public void SelectLevel3()
    {
        GameManager.Instance.ChangeScene("Level3", 2);
    }

    public void QuitGame()
    {
        Debug.Log("[MainMenu] Saliendo de la aplicación...");
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #else
                Application.Quit();
    #endif
        }
    }