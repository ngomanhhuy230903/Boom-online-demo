using Photon.Pun;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;


public class RoomManager : MonoBehaviourPunCallbacks
{
    //public static RoomManager instance;
    //void Awake()
    //{
    //    if (instance != null && instance != this)
    //    { 

    //        Destroy(gameObject);
    //        return;
    //    }
    //    DontDestroyOnLoad(gameObject);
    //    instance = this;

    //}
    //public override void OnEnable()
    //{
    //    base.OnEnable();
    //    SceneManager.sceneLoaded += OnSceneLoaded;
    //}

    //public override void OnDisable()
    //{
    //    base.OnDisable();
    //    SceneManager.sceneLoaded -= OnSceneLoaded;
    //}
    //void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    //{
    //    if (scene.buildIndex == 1)
    //    {
    //        Vector3 spawnPosition = new Vector3((PhotonNetwork.CurrentRoom.PlayerCount - 1) * 2.0f, 0, 0);

    //        PhotonNetwork.Instantiate("Player", spawnPosition, Quaternion.identity);
    //    }
    //}
}
