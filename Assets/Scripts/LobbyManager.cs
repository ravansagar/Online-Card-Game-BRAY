using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using System.IO;

namespace Bray
{
    public class LobbyManager : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private Image bgImage;
        [SerializeField]
        private TMP_InputField createRoomInput;
        [SerializeField]
        private TMP_InputField joinRoomInput;
        [SerializeField]
        private TMP_Text Text;
        [SerializeField]
        private GameObject InitialPanel;
        [SerializeField]
        private GameObject CreatePanel;
        [SerializeField]
        private GameObject JoinPanel;
        [SerializeField]
        private GameObject JoinedPanel;
        [SerializeField]
        private GameObject primaryPanel;
        [SerializeField]
        private TMP_InputField nickName;
        [SerializeField]
        private Image wave;
        [SerializeField]
        private TMP_Text waveText;
        private GameObject previousPanel;
        private GameObject currentPanel;
        private float lastBackPressTime = 0f;
        private float backPressInterval = 2f;
        private void Start()
        {
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.ConnectUsingSettings();
            }
            Screen.SetResolution(Screen.width, Screen.height, false);
            if (bgImage != null)
            {
                bgImage.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
            }
            previousPanel = null;
            currentPanel = null;
        }
        private void Awake()
        {
            if (PlayerPrefs.HasKey("NickName"))
            {
                PhotonNetwork.LocalPlayer.NickName = PlayerPrefs.HasKey("NickName") ? PlayerPrefs.GetString("NickName") : $"Player {PhotonNetwork.LocalPlayer.ActorNumber}";
                primaryPanel?.SetActive(false);
                WaveText();
                InitialPanel.SetActive(true);
                CreatePanel.SetActive(false);
                JoinPanel.SetActive(false);
                JoinedPanel.SetActive(false);
                previousPanel = InitialPanel;
                currentPanel = InitialPanel;
            }
            else
            {
                takeNickName();
            }
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                
                if(Time.time - lastBackPressTime < backPressInterval)
                {
                    #if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
                    #else
                        Application.Quit();
                    #endif
                }
                else
                {
                    lastBackPressTime = Time.time;
                }
                if(previousPanel == currentPanel) { }
                else
                {
                    currentPanel.SetActive(false);
                    previousPanel.SetActive(true);
                }
            }
        }
        private void WaveText()
        {
            if (!PlayerPrefs.HasKey("NickName"))
            {
                wave.sprite = null;
                waveText.text = "";
            }
            else
            {
                string playerNickName = PlayerPrefs.GetString("NickName");
                waveText.text = $"Welcome back {playerNickName}";
            }
        }
      
        public void takeNickName()
        {
            InitialPanel.SetActive(false);
            CreatePanel.SetActive(false);
            JoinPanel.SetActive(false);
            JoinedPanel.SetActive(false);
            primaryPanel.SetActive(true);
        }
        public void OnNameEntered()
        {
            string userName = nickName.text;
            PlayerPrefs.SetString("NickName", userName);
            PlayerPrefs.Save();
            PhotonNetwork.NickName = userName;
            Hashtable playerProperties = new Hashtable();
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
            PlayerPrefs.Save();
            Awake();
        }
        public void CreateRoom()
        {
            InitialPanel?.SetActive(false);
            previousPanel = InitialPanel;
            CreatePanel.SetActive(true);
            currentPanel = CreatePanel;
        }

        public void JoinRoom()
        {
            InitialPanel?.SetActive(false);
            previousPanel = InitialPanel;
            JoinPanel.SetActive(true);
            currentPanel = JoinPanel;
        }

        public void CreateRoomWithInput()
        {
            string roomName = createRoomInput.text;
            if (!string.IsNullOrEmpty(roomName))
            {
                PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = 4 });
            }
        }

        public void JoinRandomRoom()
        {
            PhotonNetwork.JoinRandomRoom();
        }

        public void JoinRoomWithInput()
        {
            string roomName = joinRoomInput.text;
            if (!string.IsNullOrEmpty(roomName))
            {
                PhotonNetwork.JoinRoom(roomName);
            }
        }
        public override void OnJoinedRoom()
        {
            if (PlayerPrefs.HasKey("NickName") || PlayerPrefs.HasKey("ImagePath"))
            {
                string savedNickname = PlayerPrefs.GetString("NickName");
                string savedPath = PlayerPrefs.GetString("ImagePath");
                PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "NickName", savedNickname } });
                PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "ImagePath", savedPath } });
                PhotonNetwork.LocalPlayer.NickName = savedNickname;
            }
            JoinedPanel.SetActive(true);
            InitialPanel.SetActive(false);
            CreatePanel.SetActive(false);
            JoinPanel.SetActive(false);

            Text.text = "You joined " + PhotonNetwork.CurrentRoom.Name + " room. Waiting for " +
                        (4 - PhotonNetwork.CurrentRoom.PlayerCount) + " players to start the game...";

            CheckPlayerCount();
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Text.text = "Waiting for " + (4 - PhotonNetwork.CurrentRoom.PlayerCount) + " players to start the game...";
            CheckPlayerCount();
        }
        private void CheckPlayerCount()
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 4)
            {
                Text.text = "4 players joined the room\nStarting the game...";
                PhotonNetwork.LoadLevel("GameScene");
            }
        }
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }
    }
}