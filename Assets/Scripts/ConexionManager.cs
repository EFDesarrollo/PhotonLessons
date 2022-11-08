using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using WebSocketSharp;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;
using Photon.Pun.Demo.Cockpit.Forms;
using System.ComponentModel;

public class ConexionManager : MonoBehaviourPunCallbacks
{
    #region Private Vars

    #endregion
    #region Public Vars
    [Header("Paneles UI")] // paneles del menú
    public GameObject initialPanel;
    public GameObject welcomePanel, createRoomPanel, roomPanel, joinRoomPanel;
    [Header("Botones UI")] // Boton de conexion
    public Button connectButton;
    [Header("Inputs UI")] // Input del usuario
    public TMP_InputField nicknameInput;
    public TMP_InputField roomNameInput, minPlayersInput, maxPlayersInput, joinRoomNameInput;
    [Header("Info UI")] // Objetos que contendrán información del y para el jugador
    public TextMeshProUGUI stateText;
    public TextMeshProUGUI userText;
    public TextMeshProUGUI roomUserNikcname, roomName, roomPlayersCount, roomType;
    private void Start()
    {
        SetActivePanel(initialPanel);
    }
    #endregion
    /// <summary>
    /// metodo que cambiara el mensaje de estado
    /// de los paneles de introduccion al juego
    /// </summary>
    /// <param name="texto">Nuevo mensaje a colocar</param>
    private void ChangeState(string text)
    {
        stateText.text = text;
    }
    private void ChangeUser(string text)
    {
        userText.text = text;
    }
    public void CreateRoom(string roomName, byte maxPlayers, bool isVisible = true, bool isOpen = true)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayers;
        roomOptions.IsVisible = isVisible;
        roomOptions.IsOpen = isOpen;
        PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
    }
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }
    public void ModifyRoomPanel()
    {
        // activate panel
        SetActivePanel(roomPanel);
        //actualizar informacion del panel
        //comenzamos leyendo la información.
        Player[] jugadores = PhotonNetwork.PlayerList;
        string playersText = "";
        for (int i = 0; i < jugadores.Length; i++)
        {
            playersText += jugadores[i].ActorNumber + " - " + jugadores[i].NickName + "\n";
        }
        roomUserNikcname.text = playersText;
        roomName.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name;
        roomPlayersCount.text = "Players: " + PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;
        roomType.text = "Room Type: " + (PhotonNetwork.CurrentRoom.IsOpen ? "Open" : "Closed");
    }
    #region Override functions

    public override void OnConnected()
    {
        //base.OnConnected();
        PhotonNetwork.NickName = nicknameInput.text;
        ChangeState("Conected!!");
        ChangeUser(PhotonNetwork.NickName);
        SetActivePanel(welcomePanel);
    }
    public override void OnCreatedRoom()
    {
        //base.OnCreatedRoom();
        ChangeState("Room Created Susefully");
        ModifyRoomPanel();
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        //base.OnCreateRoomFailed(returnCode, message);
        ChangeState("Create room failed: " + message);
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        // Informamos al usuario
        ChangeState("Joined!");
        ModifyRoomPanel();
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        // Informamos al usuario
        ChangeState("Join Failed");
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        // Informamos al usuario
        ChangeState("a player joined!");
        ModifyRoomPanel();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        // Informamos al usuario
        ChangeState("a player Left!");
        ModifyRoomPanel();
    }
    #endregion
    #region On Click Events
    public void OnClickConectToServer()
    {

        if (!(string.IsNullOrEmpty(nicknameInput.text) || string.IsNullOrWhiteSpace(nicknameInput.text)))
            if (!PhotonNetwork.IsConnected)
            {
                ChangeState("Conecting...");
                connectButton.interactable = false;
                PhotonNetwork.ConnectUsingSettings();
            }
            else
                ChangeState("Is conected");
        else
            ChangeState("There isn't a nickname");
    }
   
    public void OnClickCreateRoom()
    {
        if (!(string.IsNullOrEmpty(roomNameInput.text) || string.IsNullOrWhiteSpace(roomNameInput.text)))
        {
            if (int.Parse(maxPlayersInput.text) >= int.Parse(minPlayersInput.text))
            {
                CreateRoom(roomNameInput.text, byte.Parse(maxPlayersInput.text));
                ChangeState("Created");
            }
            else
                ChangeState("Players Input Error");
        }
    }
    public void OnClickJoinRoom()
    {
        if (!(string.IsNullOrEmpty(joinRoomNameInput.text) || string.IsNullOrWhiteSpace(joinRoomNameInput.text)))
            JoinRoom(joinRoomNameInput.text);
        else
            // Informamos al usuario
            ChangeState("name error");
    }
    #endregion
    #region Active Panels
    private void SetActivePanel(GameObject panel)
    {
        initialPanel.SetActive(false);
        welcomePanel.SetActive(false);
        createRoomPanel.SetActive(false);
        roomPanel.SetActive(false);
        joinRoomPanel.SetActive(false);

        panel.SetActive(true);
    }
    public void SetActivePanelInitial()
    {
        SetActivePanel(initialPanel);
    }
    public void SetActivePanelWelcome()
    {
        SetActivePanel(welcomePanel);
    }
    public void SetActivePanelCreateRoom()
    {
        SetActivePanel(createRoomPanel);
    }
    public void SetActivePanelJoinRoom()
    {
        SetActivePanel(joinRoomPanel);
    }
    #endregion
}
