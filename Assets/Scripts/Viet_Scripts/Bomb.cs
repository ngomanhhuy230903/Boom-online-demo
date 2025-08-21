using UnityEngine;

public class Bomb : MonoBehaviour
{
    public GameObject explosionPrefab; 
    public float countdownExplosion = 3f;      
    public float explosionOffsetY = 1f;   // nâng cao hiệu ứng
    public int explosionRange = 5;        // số bước nổ theo 4 hướng

    void Start()
    {
        Invoke("Explode", countdownExplosion);  
    }

    void Explode()
    {
        if (explosionPrefab != null)
        {
            // Tâm nổ
            Vector3 center = transform.position + Vector3.up * explosionOffsetY;
            Instantiate(explosionPrefab, center, Quaternion.identity);

            // Nổ 4 hướng chữ thập
            SpawnExplosion(Vector3.forward);  // lên
            SpawnExplosion(Vector3.back);     // xuống
            SpawnExplosion(Vector3.left);     // trái
            SpawnExplosion(Vector3.right);    // phải
        } 

        Destroy(gameObject);
    }

    void SpawnExplosion(Vector3 direction)
    {
        for (int i = 1; i <= explosionRange; i++) // từ 1 đến 5 đơn vị
        {
            Vector3 spawnPos = transform.position + Vector3.up * explosionOffsetY + direction * i;
            Instantiate(explosionPrefab, spawnPos, Quaternion.identity);
        }
    }
}