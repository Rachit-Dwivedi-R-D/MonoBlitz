using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public TMP_InputField userName;
    public TMP_InputField createRoomInputField;
    public TMP_InputField joinRoomInputField;

    [Space(10)]
    public Button playButton;

    [Space(10)]
    public TMP_Text connectButtonText;
    public TMP_Text[] userNames;

    [Space(10)]
    public GameObject enterName;
    public GameObject createRoom;
    public GameObject joinRoom;
    public GameObject room;

    private AudioManager audioManager;
    private bool isConnectedToMaster = false;
    private bool isInLobby = false;

    // Shared player color palette
    private static readonly Color[] playerColors = {
        new Color(0.3f, 1f, 0.3f),    // Bright Green
        new Color(1f, 0.7f, 0.2f),    // Orange
        new Color(1f, 0f, 1f),        // Magenta
        new Color(0.6f, 0.3f, 1f),    // Soft Purple
        new Color(1f, 1f, 0.4f)       // Light Yellow
    };

    private void Start()
    {
        audioManager = AudioManager.instance;
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "asia";
    }

    public void Connect()
    {
        if (userName.text.Length <= 0) return;

        PhotonNetwork.NickName = userName.text;
        connectButtonText.text = "Connecting..";
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();

        audioManager.PlaySoundEffect("Click");
    }

    public override void OnConnectedToMaster()
    {
        isConnectedToMaster = true;
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        isInLobby = true;
        enterName.SetActive(false);
        createRoom.SetActive(true);
        joinRoom.SetActive(true);
    }

    public void CreateRoom()
    {
        if (!isConnectedToMaster || !isInLobby) return;

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 5,
            CleanupCacheOnLeave = true,
            IsVisible = true,
            IsOpen = true
        };

        PhotonNetwork.CreateRoom(createRoomInputField.text, roomOptions);
        audioManager.PlaySoundEffect("Click");
    }

    public void JoinRoom()
    {
        audioManager.PlaySoundEffect("Click");

        if (isConnectedToMaster && isInLobby)
        {
            PhotonNetwork.JoinRoom(joinRoomInputField.text);
        }
    }

    public override void OnJoinedRoom()
    {
        createRoom.SetActive(false);
        joinRoom.SetActive(false);
        room.SetActive(true);

        AssignLocalPlayerColor(); // CustomProperties["Color"] will be synced soon

        playButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
    }

    private void AssignLocalPlayerColor()
    {
        int index = Mathf.Min(PhotonNetwork.LocalPlayer.ActorNumber - 1, playerColors.Length - 1);
        string hexColor = ColorUtility.ToHtmlStringRGB(playerColors[index]);

        Hashtable playerProps = new Hashtable
        {
            { "Color", hexColor }
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);
    }

    private void UpdatePlayerNames()
    {
        for (int i = 0; i < userNames.Length; i++)
        {
            userNames[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < PhotonNetwork.PlayerList.Length && i < userNames.Length; i++)
        {
            Player player = PhotonNetwork.PlayerList[i];
            userNames[i].gameObject.SetActive(true);
            userNames[i].text = player.NickName;

            // 🟩 Read color from synced properties
            if (player.CustomProperties.TryGetValue("Color", out object colorValue))
            {
                if (ColorUtility.TryParseHtmlString("#" + colorValue.ToString(), out Color parsedColor))
                {
                    userNames[i].color = parsedColor;
                }
                else
                {
                    userNames[i].color = Color.white;
                }
            }
            else
            {
                userNames[i].color = Color.white;
            }
        }

        playButton.enabled = PhotonNetwork.CurrentRoom.PlayerCount >= 2;
        playButton.GetComponent<Image>().color = playButton.enabled ? Color.white : new Color(0.7f, 0.7f, 0.7f);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        // When a player's color is synced, update the UI
        if (changedProps.ContainsKey("Color"))
        {
            UpdatePlayerNames();
        }
    }

    public void LeftRoom()
    {
        audioManager.PlaySoundEffect("Click");
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        room.SetActive(false);
        createRoom.SetActive(true);
        joinRoom.SetActive(true);
    }

    public void Play()
    {
        audioManager.PlaySoundEffect("Click");
        audioManager.StopMusic();
        PhotonNetwork.LoadLevel("Multiplayer Game");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // Player entered, but we wait for OnPlayerPropertiesUpdate for color before updating UI
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerNames();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        LeftRoom();
    }
}
