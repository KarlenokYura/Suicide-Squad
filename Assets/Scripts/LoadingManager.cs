using Photon.Pun;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviourPunCallbacks
{
    public void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("LobbyScene");
    }
}
