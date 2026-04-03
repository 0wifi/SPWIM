using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class HealthAndDamage : MonoBehaviour
{
    public int Health;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Destroys self when at or under 0 HP
        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
