using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;

public class SteamLobby : MonoBehaviour
{
    public static SteamLobby Instance;

    [SerializeField] private GameObject buttons = null;

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;

    public ulong CurrentLobbyID;

    private const string HostAddressKey = "HostAddress";

    private CustomNetworkManager networkManager;

    void Start(){
        networkManager = GetComponent<CustomNetworkManager>();

        if(!SteamManager.Initialized) return;

        if(Instance == null) Instance = this;

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    public void HostLobby(){
        buttons.SetActive(false);

        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic,networkManager.maxConnections);
    }

    private void OnLobbyCreated(LobbyCreated_t callback){
        if(callback.m_eResult != EResult.k_EResultOK){
            buttons.SetActive(true);
            return;
        }

        networkManager.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby),HostAddressKey,SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby),"name", SteamFriends.GetPersonaName().ToString() + "'s Lobby");
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback){
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback){

        CurrentLobbyID = callback.m_ulSteamIDLobby;

        if(NetworkServer.active) return;

        string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby),HostAddressKey);
        
        networkManager.networkAddress = hostAddress;
        networkManager.StartClient();

        buttons.SetActive(false);
    }
}
