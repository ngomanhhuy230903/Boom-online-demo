using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit by explosion!");
            GameManager_Offline.Instance.TakeDamage(1); // trừ máu
        }
    }

    private void Start()
    {
        Destroy(gameObject, 0.5f); // vụ nổ biến mất sau 0.5s
    }
}
