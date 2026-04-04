using System;
using TMPro;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    [SerializeField] private Array enemyList;
    private int remainingEnemies;
    [SerializeField] private TMP_Text playerDisplay;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FindEnemy();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FindEnemy()
    {
        enemyList = GameObject.FindGameObjectsWithTag("Enemy");
        remainingEnemies = enemyList.Length;
        if (remainingEnemies <= 0)
        {
            Debug.Log("All Enemies Defeated");
            playerDisplay.text = "ALL TARGETS DESTROYED";
        }
        else 
        { 
            Debug.Log(enemyList.ToString());
            playerDisplay.text = "Enemies Remaining: " + enemyList.Length;
        }
    }
}
