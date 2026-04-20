using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    private PlayerInput playerInput;

    [SerializeField] private PlayerCombat playerCombat;

    [SerializeField] private TMP_Text healthDisplay;
    [SerializeField] private float playerHealthMax;
    [SerializeField] private float playerHealth;

    public TMP_Text ShieldDisplay;
    public TMP_Text DrDisplay;
    public float PlayerShieldMax;
    public float PlayerShield;
    private bool shieldRegen = true;
    [SerializeField] private float shieldRechargeDelayStart;
    [SerializeField] private int shieldRechargeRate;
    [SerializeField] private float shieldRechargeDelay;
    private bool canRecharge = true;
    [SerializeField] private float shieldBrokenDelay;
    private bool shieldBroken = false;

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
        ShieldDisplay.text = "Shield: " + PlayerShield;
    }

    private void Update()
    {
        if (playerHealth <= 0)
        {
            SceneManager.LoadScene("DiedScene");
        }

        if (IsHealing == true)
        {
            StartCoroutine(Heal());
        }

        if (shieldRegen == true)
        {
            if (PlayerShield < PlayerShieldMax && canRecharge == true)
            {
                StartCoroutine(ShieldRegen());
            }
            else if (PlayerShield >= PlayerShieldMax)
            {
                PlayerShield = PlayerShieldMax;
                ShieldDisplay.text = "Shield: " + PlayerShield;
                DrDisplay.text = "Damage Reduction: 100%";
            }
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

    //The delay before the shield starts regenerating
    public IEnumerator ShieldRecharge()
    {
        shieldRegen = false;
        yield return new WaitForSeconds(shieldRechargeDelayStart);
        shieldRegen = true;
    }

    //The harsher delay to shield regeneration set by breaking the shield
    public IEnumerator ShieldBreak()
    {
        shieldBroken = true;
        yield return new WaitForSeconds(shieldBrokenDelay);
        shieldBroken = false;
    }

    //Starts regenerating the shield and updates both shield and damage reduction texts
    private IEnumerator ShieldRegen()
    {
        if (canRecharge == true && shieldBroken == false && playerCombat.IsBlocking == false)
        {
            canRecharge = false;
            PlayerShield += shieldRechargeRate;

            ShieldDisplay.text = "Shield: " + PlayerShield;

            if (PlayerShield >= (PlayerShieldMax * 0.75))
            {
                DrDisplay.text = "Damage Reduction: 100%";
            }
            else if (PlayerShield < (PlayerShieldMax * 0.75) && PlayerShield >= (PlayerShieldMax * 0.5))
            {
                DrDisplay.text = "Damage Reduction: 75%";
            }
            else if (PlayerShield < (PlayerShieldMax * 0.5) && PlayerShield >= (PlayerShieldMax * 0.25))
            {
                DrDisplay.text = "Damage Reduction: 50%";
            }
            else if (PlayerShield < (PlayerShieldMax * 0.25) && PlayerShield > 0)
            {
                DrDisplay.text = "Damage Reduction: 25%";
            }
            else
            {
                DrDisplay.text = "Damage Reduction: BROKEN";
            }

            yield return new WaitForSeconds(shieldRechargeDelay);
            canRecharge = true;
        }
    }

    public void HealthUpdate(bool isPositive, float change)
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
