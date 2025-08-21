using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        Debug.Log($"{gameObject.name} nhận {damage} sát thương, HP từ {currentHealth} giảm còn {currentHealth - damage}");
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log($"{gameObject.name} đã chết!");

        gameObject.SetActive(false);

        if (GameManager_Offline.Instance != null)
        {
            // Nếu chính là currentPlayer của local
            if (gameObject == GameManager_Offline.Instance.currentPlayer)
            {
                // check xem còn ai sống không
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                bool othersAlive = false;
                foreach (var p in players)
                {
                    if (p != gameObject)
                    {
                        Health h = p.GetComponent<Health>();
                        if (h != null && h.IsAlive())
                        {
                            othersAlive = true;
                            break;
                        }
                    }
                }

                if (othersAlive)
                {
                    GameManager_Offline.Instance.GameOver("Bạn đã chết!");
                    return; // không cần check win nữa
                }
            }

            // nếu không phải currentPlayer hoặc không còn ai → check win
            GameManager_Offline.Instance.CheckWinCondition();
        }
    }


    public bool IsAlive()
    {
        return !isDead && currentHealth > 0;
    }
}