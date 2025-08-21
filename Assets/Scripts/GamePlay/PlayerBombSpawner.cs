using UnityEngine;

public class PlayerBombSpawner : MonoBehaviour
{
    public GameObject bombPrefab;  
    public float spawnDistance = 2f; // Khoảng cách trước mặt Player

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnBomb();
        }
    }

    void SpawnBomb()
    {
        // Tính vị trí spawn bom trước mặt player
        Vector3 spawnPos = transform.position + transform.forward * spawnDistance;
        Quaternion spawnRot = Quaternion.identity; // Bom đứng thẳng

        Instantiate(bombPrefab, spawnPos, spawnRot);
    }
}
