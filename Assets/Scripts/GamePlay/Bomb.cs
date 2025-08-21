using UnityEngine;

public class Bomb : MonoBehaviour
{
    public GameObject explosionPrefab; 
    public float countdownExplosion = 3f;      
    public float explosionOffsetY = 1f;   
    public int explosionRange = 5;

    private static int globalExplosionGroupCounter = 0;
    private int myExplosionGroupId;

    void Start()
    {
        myExplosionGroupId = ++globalExplosionGroupCounter;
        Invoke("Explode", countdownExplosion);  
    }

    void Explode()
    {
        if (explosionPrefab != null)
        {
            Vector3 center = transform.position + Vector3.up * explosionOffsetY;
            SpawnExplosion(center);

            SpawnExplosion(transform.position + Vector3.up * explosionOffsetY, Vector3.forward);
            SpawnExplosion(transform.position + Vector3.up * explosionOffsetY, Vector3.back);
            SpawnExplosion(transform.position + Vector3.up * explosionOffsetY, Vector3.left);
            SpawnExplosion(transform.position + Vector3.up * explosionOffsetY, Vector3.right);
        } 

        Destroy(gameObject);
    }

    void SpawnExplosion(Vector3 basePos, Vector3? direction = null)
    {
        if (direction == null)
        {
            GameObject obj = Instantiate(explosionPrefab, basePos, Quaternion.identity);
            obj.GetComponent<Explosion>().explosionGroupId = myExplosionGroupId;
            return;
        }

        for (int i = 1; i <= explosionRange; i++)
        {
            Vector3 spawnPos = basePos + direction.Value * i;
            GameObject obj = Instantiate(explosionPrefab, spawnPos, Quaternion.identity);
            obj.GetComponent<Explosion>().explosionGroupId = myExplosionGroupId;
        }
    }
}
