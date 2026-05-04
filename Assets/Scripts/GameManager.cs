using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private string[][] levelMessages = new string[][]
    {
        new string[] { "> INITIALIZING LONG-RANGE SCANS...", "> DOWNLOADING AREA SCHEMATICS...", "> MAPPING PATROL ROUTES...", "> INFILTRATION VECTOR CALCULATED." },
        new string[] { "> BREACH CONFIRMED...", "> ACCESSING INTERNAL SERVERS...", "> SEARCHING FOR ENCRYPTED FILES...", "> OBJECTIVE: LOCATE CLASSIFIED DOCUMENTS." },
        new string[] { "> INTEL SECURED...", "> ENCRYPTING DATA STREAMS...", "> WIPING ACCESS LOGS...", "> RUN. GET OUT NOW." },
    };

    public string[] CurrentLoadingText { get; private set; }

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    public void ChangeScene(string targetScene, int levelIndex = -1)
    {
        if (levelIndex >= 0 && levelIndex < levelMessages.Length)
        {
            CurrentLoadingText = levelMessages[levelIndex];
        }

        StartCoroutine(LoadSceneSequence(targetScene));
    }

    private IEnumerator LoadSceneSequence(string targetScene)
    {
        if (targetScene == "MainMenu")
        {
            AsyncOperation menuOp = SceneManager.LoadSceneAsync(targetScene);

            while (!menuOp.isDone)
            {
                yield return null;
            }
            yield break;
        }

        TerminalLoadingEffect.ResetState();

        yield return SceneManager.LoadSceneAsync("LoadingScene", LoadSceneMode.Additive);

        AsyncOperation targetOp = SceneManager.LoadSceneAsync(targetScene);
        targetOp.allowSceneActivation = false;

        while (!TerminalLoadingEffect.IsReadyToTransition)
        {
            yield return null;
        }

        targetOp.allowSceneActivation = true;

        while (!targetOp.isDone) yield return null;

        Scene loadingScene = SceneManager.GetSceneByName("LoadingScene");
        if (loadingScene.isLoaded)
        {
            SceneManager.UnloadSceneAsync(loadingScene);
        }
    
    }
} 