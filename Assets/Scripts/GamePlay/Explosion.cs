using UnityEngine;
using System.Collections.Generic;

public class Explosion : MonoBehaviour
{
    public int explosionGroupId;
    private static Dictionary<int, HashSet<GameObject>> groupDamagedObjects 
        = new Dictionary<int, HashSet<GameObject>>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!groupDamagedObjects.ContainsKey(explosionGroupId))
                groupDamagedObjects[explosionGroupId] = new HashSet<GameObject>();

            var damagedSet = groupDamagedObjects[explosionGroupId];

            if (!damagedSet.Contains(other.gameObject))
            {
                Health health = other.GetComponent<Health>();
                if (health != null && health.IsAlive())
                {
                    Debug.Log($"[ExplosionGroup {explosionGroupId}] Gây 1 sát thương cho {other.gameObject.name}");
                    health.TakeDamage(1);
                    damagedSet.Add(other.gameObject);
                }
            }
        }
    }

    private void Start()
    {
        Destroy(gameObject, 0.5f); 
    }
}
