using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CharacterControlManager : MonoBehaviour
{
    public static CharacterControlManager Instance;

    [HideInInspector] public T11Joystick joystick;
    [HideInInspector] public CharacterController controller;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Animator playerAnimator;
    [HideInInspector] public Canvas inputCanvas;

    [Header("Movement Module")]
    public bool isJoytick;
    public float movementSpeed;
    public float rotationSpeed;

    [Header("Health Module")]
    public GameObject healthModuleCanvas;
    public Image healthBar;
    public Image healthBarEase;
    public int maxHealth;
    public float healthEaseSpeed;

    [Header("CollectorModule")]
    public GameObject currencyCollector;

    [Header("Floating Text")]
    public floatingText floatingTextPrefab;
    
    private bool isDead;
    private Vector3 _moveVector;
    private int currentHealth;

    private void Awake(){
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance); 
        }

        joystick = GetComponentInChildren<T11Joystick>();
        controller = GetComponentInChildren<CharacterController>();
        rb = GetComponentInChildren<Rigidbody>();
        playerAnimator = GetComponentInChildren<Animator>();
        inputCanvas = GetComponentInChildren<Canvas>();
    }

    private void Start()
    {
        if(isJoytick)
            EnableJoystick();

        loadHealth();
    }

    void Update()
    {
        CheckHealthForUI();
    }

    private void FixedUpdate()
    {
        if(!isDead)
            MoveCharacter();
    }

    #region Movement Module
    public void EnableJoystick()
    {
        isJoytick = true;
        inputCanvas.gameObject.SetActive(true);
    }

    private void MoveCharacter()
    {
        _moveVector = Vector3.zero;
        _moveVector.x = joystick.Horizontal * movementSpeed * Time.deltaTime;
        _moveVector.z = joystick.Vertical * movementSpeed * Time.deltaTime;

        if(joystick.Horizontal != 0 || joystick.Vertical != 0)
        {
            Vector3 direction = Vector3.RotateTowards(rb.transform.forward, _moveVector, rotationSpeed * Time.deltaTime, 0.0f).normalized;
            rb.transform.rotation = Quaternion.LookRotation(direction);

            playerAnimator.SetBool("isWalking", true);
        }

        else if(joystick.Horizontal == 0 && joystick.Vertical == 0)
        {
            playerAnimator.SetBool("isWalking", false);
        }

        rb.MovePosition(rb.position + _moveVector);
    }
    #endregion

    #region Health Module
    private void loadHealth()
    {
        currentHealth = SaveModule.Instance.saveInfo.characterCurrentHealth;
        healthBar.fillAmount = (float)currentHealth / maxHealth;
        healthBarEase.fillAmount = (float)currentHealth / maxHealth;

        if(currentHealth <= 0)
        {
            currentHealth = maxHealth;
            SaveModule.Instance.saveInfo.characterCurrentHealth = currentHealth;
            healthBar.fillAmount = maxHealth;
            healthBarEase.fillAmount = maxHealth;
        }
    }

    private void CheckHealthForUI()
    {
        if(currentHealth < maxHealth)
        {
            healthModuleCanvas.SetActive(true);
        }else if(currentHealth <= 0)
        {
            currencyCollector.SetActive(false);
        }
        else
        {
            healthModuleCanvas.SetActive(false);
        }

        if(healthBar.fillAmount != healthBarEase.fillAmount)
        {
            healthBarEase.fillAmount = Mathf.Lerp(healthBarEase.fillAmount, healthBar.fillAmount, healthEaseSpeed * Time.deltaTime);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        SaveModule.Instance.saveInfo.characterCurrentHealth = currentHealth;

        healthBar.fillAmount = (float)currentHealth / maxHealth;

        if(floatingTextPrefab)
        {
            Vector3 spawnPosition = rb.transform.position;
            spawnPosition.y += 1.5f;
            floatingText _floatingText = Instantiate(floatingTextPrefab, spawnPosition, Quaternion.identity);
            _floatingText.SetText("-" + amount.ToString(), Color.red, 6f);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        playerAnimator.SetBool("isDead", true);
        isDead = true;
    } 
    #endregion
}
