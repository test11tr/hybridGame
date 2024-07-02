using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum IncrementalMenuType
    {
        Menu,
        Wheel
    }

    [Foldout("Game Manager Important References", foldEverything = true, styled = true, readOnly = false)]
    public static GameManager Instance;
    public SaveModule saveModule;
    public CharacterControlManager player;
    public WalletModule wallet;
    public ExperienceModule experienceModule;
    public DelayHelper delayHelper;
    public SoundModule soundModule;
    public CameraShakeModule cameraShakeModule;
    public incrementalManagerMenu incrementelMenuManager;
    public incrementalManagerWheel incrementelWheelManager;
    public IncrementalMenuType incrementalMenuType;
    public Camera mainCamera;
    public CinemachineVirtualCamera vcam;

    [Foldout("Player Spawner", foldEverything = true, styled = true, readOnly = false)]
    public Transform SpawnPoint;
    
    [Foldout("Gameplay Settings", foldEverything = true, styled = true, readOnly = true)]
    public int targetFps;
    [DisplayWithoutEdit()] public bool isGamePaused;

    [Foldout("Debug", foldEverything = true, styled = true, readOnly = false)]
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

    public void openIncrementalMenu(){
        if(incrementalMenuType == IncrementalMenuType.Menu){
            incrementelMenuManager.gameObject.SetActive(true);
        }else{
            incrementelWheelManager.gameObject.SetActive(true);
        }
    }

    public void closeIncrementalMenu(){
        if(incrementalMenuType == IncrementalMenuType.Menu){
            incrementelMenuManager.gameObject.SetActive(false);
        }else{
            incrementelWheelManager.gameObject.SetActive(false);
        }
    }

    public void SetTrue (GameObject target)
        {
            target.SetActive (true);
        }

        public void SetFalse (GameObject target)
        {
            target.SetActive (false);
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
