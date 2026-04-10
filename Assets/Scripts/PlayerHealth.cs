using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    private PlayerInput playerInput;

    [SerializeField] private TMP_Text healthDisplay;
    [SerializeField] private int playerHealthMax;
    [SerializeField] private int playerHealth;

    [SerializeField] private float regenTime;
    [SerializeField] private int regenRate;
    public bool IsHealing = false;
    private bool canHeal = true;


    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions["Heal"].started += ctx => OnHealStarted();
        playerInput.actions["Heal"].canceled += ctx => OnHealCanceled();


        healthDisplay.text = "Health: " + playerHealth;
    }

    private void Update()
    {
        if (playerHealth <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (IsHealing == true)
        {
            StartCoroutine(Heal());
        }
    }

    private void OnHealStarted()
    {
        IsHealing = true;
    }

    private void OnHealCanceled()
    {
        IsHealing = false;
    }

    private IEnumerator Heal()
    {
        if (canHeal == true)
        {
            canHeal = false;

            HealthUpdate(true, regenRate);
            yield return new WaitForSeconds(regenTime);

            canHeal = true;
        }
    }

    public void HealthUpdate(bool isPositive, int change)
    {
        if (isPositive == false)
        {
            playerHealth -= change;
        }
        else
        {
            if (playerHealth < playerHealthMax)
            {
                playerHealth += change;

                if (playerHealth >= playerHealthMax)
                {
                    playerHealth = playerHealthMax;
                }
            }
        }

        healthDisplay.text = "Health: " + playerHealth;
    }

    private void OnDestroy()
    {
        playerInput.actions["Heal"].started -= ctx => OnHealStarted();
        playerInput.actions["Heal"].canceled -= ctx => OnHealCanceled();
    }
}
