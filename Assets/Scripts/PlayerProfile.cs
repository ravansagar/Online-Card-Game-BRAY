using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Bray;

namespace Bray
{
    public class PlayerProfile : MonoBehaviourPunCallbacks
    {
        private LobbyManager lobbyManager;
        void Start()
        {
            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("NickName"))
            {
                string cachedName = PhotonNetwork.LocalPlayer.CustomProperties["NickName"].ToString();
                PhotonNetwork.NickName = cachedName;
            }
            else if (PlayerPrefs.HasKey("NickName"))
            {
                string userName = PlayerPrefs.GetString("NickName");
                PhotonNetwork.NickName = userName;
                Hashtable playerProperties = new Hashtable();
                playerProperties["NickName"] = userName;
                PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
            }
            else
            {
                lobbyManager.takeNickName();
            }
        }
    }
}