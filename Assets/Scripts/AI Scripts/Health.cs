using TMPro;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int health = 100;
    public TMP_Text debugText;
    public GameObject lastHitBy = null;
    [Tooltip("calls OnHit on this. can be null.")]
    [HideInInspector] public EnemyBase enemy;

    

    public delegate void EnemyKillEvent();
    public static event EnemyKillEvent enemyKill;

    private void Start()
    {
        enemy = GetComponent<EnemyBase>();
    }

    private void Update()
    {
        if (debugText)
        {
            debugText.text = "HP: " + health.ToString();
        }
    }

    public void TakeDamage(int damage, int damageType, GameObject attacker)
    {
        if (health > 0) 
        {
            health -= damage;

            // if destroyed
            if (health <= 0)
            {
                health = 0;
                enemy.OnDestroyed(damageType);

                if (lastHitBy)
                {
                    if (lastHitBy.TryGetComponent<RangedWeapon>(out RangedWeapon rw))
                        enemyKill();
                    else
                        Debug.Log("Seems Like a grenade");
                }

                Transform objectTransform = transform;
                SoundManager2.Instance.PlaySound("RobotDeathSFX", objectTransform.transform);
                if (WaveManager.waveManagerInstance != null) WaveManager.waveManagerInstance.enemiesRemaining--;
            }
            else
            {
                lastHitBy = attacker;
                enemy.OnHit(damage, damageType);
            }
        }
    }

    private void OnDestroy()
    {
        if (lastHitBy)
        {
            if (lastHitBy.TryGetComponent<RangedWeapon>(out RangedWeapon rw))
                enemyKill();
            else
                Debug.Log("Seems Like a grenade");
        }
    }
}
