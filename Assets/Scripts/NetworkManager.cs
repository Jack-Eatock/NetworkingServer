using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{

    public static NetworkManager Instance;

    public GameObject playerPrefab;

    private void Awake() {

        // Ensures only one. Signleton method.
        if (Instance == null) {
            Instance = this;
        }
        else if (Instance != this) {
            Debug.Log("Instance already created, destroying object!");
            Destroy(this);
        }
    }

    private void Start() {

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;

        Server.Start(50,26950);
   
    }

    private void OnApplicationQuit() {
        Server.Stop();
    }

    public Player InstantiatePlayer() {
        return Instantiate(playerPrefab, Vector3.zero, Quaternion.identity).GetComponent<Player>();
    }
}
