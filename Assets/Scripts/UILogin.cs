using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;

public class UILogin : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField createInput, jointInput;
    [SerializeField] Button btnCreate, btnJoin;

    void Start()
    {
        btnCreate.onClick.AddListener(CreateRoom);
        btnJoin.onClick.AddListener(JoinRoom);
    }

    private void CreateRoom()
    {
        PhotonNetwork.CreateRoom(createInput.text);
    }

    private void JoinRoom()
    {
        PhotonNetwork.JoinRoom(jointInput.text);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        PhotonNetwork.LoadLevel(1);
    }
}
