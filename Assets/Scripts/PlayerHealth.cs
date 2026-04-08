using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private TMP_Text healthDisplay;
    [SerializeField] private int playerHealth;


    private void Start()
    {
        healthDisplay.text = "Health: " + playerHealth;
    }

    private void Update()
    {
        if (playerHealth <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void HealthUpdate(int damage)
    {
        playerHealth -= damage;
        healthDisplay.text = "Health: " + playerHealth;
    }
}
