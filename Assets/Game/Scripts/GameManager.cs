using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game Manager Important References")]
    public static GameManager Instance;
    public SaveModule saveModule;
    public CharacterControlManager player;
    public WalletModule wallet;
    public ExperienceModule experienceModule;
    public DelayHelper delayHelper;
    public Camera mainCamera;
    public CinemachineVirtualCamera vcam;

    [Header("Player Spawner")]
    public Transform SpawnPoint;

    [Header("Gameplay Settings")]
    public int targetFps;
    private bool isGamePaused;

    [Header("Debug")]
    [SerializeField] bool drawGizmos = true;
    

    private void Awake(){
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance); 
        }

        Application.targetFrameRate = targetFps;
        player = Instantiate(player, SpawnPoint.position, Quaternion.identity);
        vcam.Follow = player.rb.transform;
    }

    void OnDrawGizmos()
        {
            if (drawGizmos)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(SpawnPoint.transform.position, 0.5f);
                Gizmos.DrawRay(SpawnPoint.transform.position, transform.position + SpawnPoint.transform.forward * 1f);

                Vector3 plyright = Quaternion.LookRotation(SpawnPoint.transform.forward) * Quaternion.Euler(0, 180 + 20, 0) * new Vector3(0, 0, 1);
                Vector3 plyleft = Quaternion.LookRotation(SpawnPoint.transform.forward) * Quaternion.Euler(0, 180 - 20, 0) * new Vector3(0, 0, 1);
                Gizmos.DrawRay(SpawnPoint.transform.position + SpawnPoint.transform.forward, plyright * 0.25f);
                Gizmos.DrawRay(SpawnPoint.transform.position + SpawnPoint.transform.forward, plyleft * 0.25f);
                Handles.Label(SpawnPoint.transform.position + Vector3.up * .2f, "Player Spawn Position");
            }
        }
}
