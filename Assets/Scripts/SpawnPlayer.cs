using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    public GameObject PlayerPrefab;

    private void Start()
    {
        Vector3 pos = new Vector3 (Random.Range(-5f, 5f), 1, Random.Range(-5f, 5f));
        PhotonNetwork.Instantiate(PlayerPrefab.name, pos, Quaternion.identity);

    }
}
