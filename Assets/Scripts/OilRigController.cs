using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class OilRigController : MonoBehaviour
{
    public int Health = 100;
    public bool IsDestructible { get; private set; } = true;

    private GameObject player;

    [SerializeField] private GameObject damageNumber;

    void Start()
    {
        try { player = GameObject.FindWithTag("Player"); }
        catch (Exception e)
        {
            Debug.LogException(e);
            Debug.LogError("Player object not found. Make sure the player has the tag 'Player'.");
        }
    }

    public void Hit(int damage, Vector3 damageNumberPos)
    {
        if (IsDestructible)
        {
            // deal damage
            Health -= damage;

            GameObject dn = Instantiate(damageNumber, damageNumberPos, Quaternion.identity);
            DamageNumber dnScript = dn.GetComponent<DamageNumber>();
            dnScript.UpdateText(damage.ToString());

            if (Health <= 0)
            {
                Die();
                return;
            }
        }
        else
        {
            GameObject dn = Instantiate(damageNumber, damageNumberPos, Quaternion.identity);
            DamageNumber dnScript = dn.GetComponent<DamageNumber>();
            dnScript.UpdateText("0 PROTECTED");
            dn.GetComponent<TextMeshPro>().color = Color.blue;
        }
    }

    public void SetDestructible(bool destructible)
    {
        IsDestructible = destructible;

        //visual change?
    }

    public void Die()
    {
        //notify wave spawner
        FindFirstObjectByType<WaveSpawningSystem>().OilRigDestroyed(this);

        Destroy(gameObject);
    }
}
