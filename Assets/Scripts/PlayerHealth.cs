using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private TMP_Text healthDisplay;
    [SerializeField] private int playerHealth;


    private void Start()
    {
        healthDisplay.text = "Health: " + playerHealth;
    }

    public void HealthUpdate(int damage)
    {
        playerHealth -= damage;
        healthDisplay.text = "Health: " + playerHealth;
    }
}
