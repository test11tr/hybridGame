using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CharacterControlManager : MonoBehaviour
{
    //public static CharacterControlManager Instance;

    [HideInInspector] public T11Joystick joystick;
    [HideInInspector] public CharacterController controller;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Animator playerAnimator;
    [HideInInspector] public Canvas inputCanvas;

    [Header("Movement Module")]
    public bool isJoytick;
    public float movementSpeed;
    public float rotationSpeed;

    [Header("Dash Module")]
    public float dashSpeed;
    public float dashDuration;
    public float dashCooldownTime;
    bool dashCooldownComplete = true;
    bool isDashing;

    [Header("Health Module")]
    public GameObject healthModuleCanvas;
    public Image healthBar;
    public Image healthBarEase;
    public int maxHealth;
    public float healthEaseSpeed;
    [HideInInspector] public int currentHealth;

    [Header("Collector Module")]
    public GameObject currencyCollector;

    [Header("Floating Text Module")]
    public floatingText floatingTextPrefab;

    [Header("Effects Module")]
    public ParticleSystem onPowerUpEffect;
    public TrailRenderer speedUpTrail;
    public ParticleSystem dashEffect;
    public ParticleSystem HealEffect;
    
    private bool isDead;
    private Vector3 _moveVector;
    
    private void Awake(){
        /*if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance); 
        }*/

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
            float inputMagnitude = new Vector2(joystick.Horizontal, joystick.Vertical).magnitude;

            Vector3 direction = Vector3.RotateTowards(rb.transform.forward, _moveVector, rotationSpeed * Time.deltaTime, 0.0f);
            rb.transform.rotation = Quaternion.LookRotation(direction);

            playerAnimator.SetBool("isWalking", true);
            playerAnimator.SetFloat("movementSpeed", inputMagnitude);
        }

        else if(joystick.Horizontal == 0 && joystick.Vertical == 0)
        {
            playerAnimator.SetBool("isWalking", false);
            playerAnimator.SetFloat("movementSpeed", 0);
        }

        rb.MovePosition(rb.position + _moveVector);
    }
    #endregion

    #region Dash Module
        public void Dash()
        {
            if(!isDead && !isDashing && dashCooldownComplete)
            {
                playerAnimator.SetBool("isDashing", true);
                dashCooldownComplete = false;
                isDashing = true;
                dashEffect.Play();
                Vector3 dashDirection = rb.transform.forward;
                rb.velocity = dashDirection * dashSpeed;

                DelayHelper.DelayAction(dashDuration, () => 
                { 
                    playerAnimator.SetBool("isDashing", false);
                    dashEffect.Stop();
                    rb.velocity = Vector3.zero; 
                    isDashing = false; 
                });

                DelayHelper.DelayAction(dashCooldownTime + dashDuration, () => 
                { 
                    dashCooldownComplete = true; 
                });
            }
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
        if(!isDead)
        {
            currentHealth -= amount;
            SaveModule.Instance.saveInfo.characterCurrentHealth = currentHealth;

            healthBar.fillAmount = (float)currentHealth / maxHealth;

            if(floatingTextPrefab)
            {
                Vector3 spawnPosition = rb.transform.position;
                spawnPosition.y += 1.5f;
                floatingText _floatingText = Instantiate(floatingTextPrefab, spawnPosition, Quaternion.identity);
                _floatingText.SetText("-" + amount.ToString() + "hp", Color.red, 3f);
            }

            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    public void Heal(int amount)
    {
        if(!isDead)
        {
            currentHealth += amount;
            SaveModule.Instance.saveInfo.characterCurrentHealth = currentHealth;

            healthBar.fillAmount = (float)currentHealth / maxHealth;

            if(floatingTextPrefab)
            {
                Vector3 spawnPosition = rb.transform.position;
                spawnPosition.y += 1.5f;
                floatingText _floatingText = Instantiate(floatingTextPrefab, spawnPosition, Quaternion.identity);
                _floatingText.SetText("+" + amount.ToString() + "hp", Color.green, 3f);
            }

            if (currentHealth >= maxHealth)
            {
                currentHealth = maxHealth;
            }
        }
    }

    void Die()
    {
        currencyCollector.SetActive(false);
        playerAnimator.SetBool("isDead", true);
        isDead = true;
    } 
    #endregion

    #region Effect Actions
        public void PlayPowerUpEffect()
        {
            onPowerUpEffect.Play();
        }
        public void PlayHealEffect()
        {
            HealEffect.Play();
        }
    #endregion

    #region PowerUp Actions
        public void AddHealth(int amount)
        {
            Heal(amount);
        }

        public void SpeedUp(float speedAmount, float duration)
        {
            movementSpeed = movementSpeed * speedAmount;
            speedUpTrail.emitting = true;
            DelayHelper.DelayAction(duration, () => 
            {
                movementSpeed /= speedAmount; 
                speedUpTrail.emitting = false;
            });
            //StartCoroutine(SpeedUpCoroutine(duration));
        }
    #endregion
}