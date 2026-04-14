using UnityEngine;

public class EnemyAttackHitbox : MonoBehaviour
{
    public EnemyController enemyController;
    [HideInInspector] public int AttackDamage;
    [HideInInspector] public bool HasHitPlayerYet;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AttackDamage = enemyController.AttackDamage;
    }

    private void OnEnable()
    {
        HasHitPlayerYet = false;
    }
}
