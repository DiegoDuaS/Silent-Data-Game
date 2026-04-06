using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

[Serializable]
public class PlayerData
{
    public Vector3 position;
    public int currentHealth;
    public int securityLevel;
    public int filesCollected;
    public bool hasPhone;
    public List<string> inventoryItems = new List<string>();
}

[Serializable]
public class WorldData
{
    public List<string> activeItemsNames = new List<string>();
    public List<string> activeEnemiesNames = new List<string>();
}

[Serializable]
public class SessionData
{
    public PlayerData playerData;
    public WorldData worldData;
}

public class PersistenceManager : MonoBehaviour
{
    public static PersistenceManager Instance;
    private string path;
    public SessionData session;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        path = Application.persistentDataPath + "/save.json";

        session = new SessionData
        {
            playerData = new PlayerData(),
            worldData = new WorldData()
        };
    }

    private void Start()
    {
        Invoke("LoadSessionData", 0.1f);
        EventManager.OnCheckpointReached += SaveSessionData;
    }

    private void OnDestroy()
    {
        EventManager.OnCheckpointReached -= SaveSessionData;
    }

    public void SaveSessionData()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            session.playerData.position = player.transform.position;
            session.playerData.currentHealth = LevelManager.Instance.currentHealth;
            session.playerData.securityLevel = LevelManager.Instance.GetSecurityLevel();
            session.playerData.filesCollected = LevelManager.Instance.GetFilesCollected();
            session.playerData.hasPhone = LevelManager.Instance.HasPhone();
            session.playerData.inventoryItems = LevelManager.Instance.GetInventory();
        }

        session.worldData.activeItemsNames.Clear();
        session.worldData.activeEnemiesNames.Clear();

        foreach (GameObject item in GameObject.FindGameObjectsWithTag("Collectible"))
        {
            session.worldData.activeItemsNames.Add(item.name);
        }

        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            session.worldData.activeEnemiesNames.Add(enemy.name);
        }

        string json = JsonUtility.ToJson(session, true);
        File.WriteAllText(path, json);
        Debug.Log("<color=green>Sesion saved in: </color>" + path);
    }

    public void LoadSessionData()
    {
        if (!File.Exists(path)) return;

        string json = File.ReadAllText(path);
        session = JsonUtility.FromJson<SessionData>(json);

 
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            player.transform.position = session.playerData.position;
            if (cc != null) cc.enabled = true;

            Debug.Log("<color=yellow>Player moved to saved position: </color>" + session.playerData.position);
        }


        LevelManager.Instance.currentHealth = session.playerData.currentHealth;
        LevelManager.Instance.SetSecurityLevel(session.playerData.securityLevel);
        LevelManager.Instance.SetFilesCollected(session.playerData.filesCollected);
        LevelManager.Instance.SetInventory(session.playerData.inventoryItems);

        EventManager.TriggerHealthChanged(session.playerData.currentHealth);
        EventManager.TriggerSecurityLevelChanged(session.playerData.securityLevel);
        EventManager.TriggerFileCollected(session.playerData.filesCollected);

        CleanWorld();
        Debug.Log("<color=cyan>Sesion Loaded.</color>");
    }

    private void CleanWorld()
    {
        GameObject[] todosLosItems = GameObject.FindGameObjectsWithTag("Collectible");
        foreach (GameObject item in todosLosItems)
        {
            if (!session.worldData.activeItemsNames.Contains(item.name))
            {
                Destroy(item);
            }
        }

        GameObject[] todosLosEnemigos = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in todosLosEnemigos)
        {
            if (!session.worldData.activeEnemiesNames.Contains(enemy.name))
            {
                Destroy(enemy);
            }
        }
    }
}