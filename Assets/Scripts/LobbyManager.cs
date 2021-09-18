using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private string _roomName = "AndreyLoh";

    public void CreateOrJoinRoom()
    {
        PhotonNetwork.JoinOrCreateRoom(_roomName, new Photon.Realtime.RoomOptions { CleanupCacheOnLeave = true }, null);
    }

    public override void OnJoinedRoom()
    {
        SceneManager.LoadScene("GameScene");
    }
}
