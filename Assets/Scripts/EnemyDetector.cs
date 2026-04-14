using System;
using TMPro;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    [SerializeField] private Array enemyList;
    private int remainingEnemies;
    [SerializeField] private TMP_Text playerDisplay;

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

    public void UpdateEnemyDisplayText(int enemyCount)
    {
        if (enemyCount <= 0)
        {
            Debug.Log("All Enemies Defeated");
            playerDisplay.text = "ALL TARGETS DESTROYED";
        }
        else
        {
            playerDisplay.text = "Enemies Remaining: " + enemyCount;
        }
    }
}
