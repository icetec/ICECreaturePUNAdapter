using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ICECreaturePUNAdapterDemoGUI : Photon.MonoBehaviour {

	public bool AutoConnect = true;
	public byte Version = 1;

	public Button ButtonConnect = null;
	public Button ButtonDisconnect = null;
	public Text TextStatus = null;
	public Image ImageStatus = null;
	public Image ImageMaster = null;

	private bool ConnectInUpdate = true;

	public virtual void Start()
	{
		//PhotonNetwork.autoJoinLobby = false; 
	}

	public virtual void Update()
	{
		if( TextStatus != null ){
			TextStatus.text = PhotonNetwork.connectionStateDetailed.ToString();
		}

		if( ! PhotonNetwork.connected )
		{
			if( ButtonConnect != null ) ButtonConnect.gameObject.SetActive( true );
			if( ButtonDisconnect != null ) ButtonDisconnect.gameObject.SetActive( false );
		}
		else
		{
			if( ButtonConnect != null ) ButtonConnect.gameObject.SetActive( false );
			if( ButtonDisconnect != null ) ButtonDisconnect.gameObject.SetActive( true );
		}

		if (ConnectInUpdate && AutoConnect && !PhotonNetwork.connected)
		{
			Debug.Log("Update() was called by Unity. Scene is loaded. Let's connect to the Photon Master Server. Calling: PhotonNetwork.ConnectUsingSettings();");

			ConnectInUpdate = false;
			PhotonNetwork.ConnectUsingSettings( "1.0" );//Version + "." + SceneManagerHelper.ActiveSceneBuildIndex);
		}

		if( ImageMaster )
		{
			if( PhotonNetwork.isMasterClient )
				ImageMaster.color = new Color( 1,1,1, 1 );
			else
				ImageMaster.color = new Color( 1,1,1, 0.25f );
		}
	}


	// below, we implement some callbacks of PUN
	// you can find PUN's callbacks in the class PunBehaviour or in enum PhotonNetworkingMessage


	public virtual void OnConnectedToMaster()
	{
		if( ImageStatus != null )
			ImageStatus.color = Color.blue;
		
		//Debug.Log("OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room. Calling: PhotonNetwork.JoinRandomRoom();");
		PhotonNetwork.JoinRandomRoom();
	}

	public virtual void OnDisconnectedFromPhoton()
	{
		if( ImageStatus != null )
			ImageStatus.color = Color.grey;
	}

	public virtual void OnJoinedLobby()
	{
		if( ImageStatus != null )
			ImageStatus.color = Color.yellow;
		
		//Debug.Log("OnJoinedLobby(). This client is connected and does get a room-list, which gets stored as PhotonNetwork.GetRoomList(). This script now calls: PhotonNetwork.JoinRandomRoom();");
		PhotonNetwork.JoinRandomRoom();
	}

	public virtual void OnPhotonRandomJoinFailed()
	{
		if( ImageStatus != null )
			ImageStatus.color = Color.red;
		
		//Debug.Log("OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one. Calling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");
		PhotonNetwork.CreateRoom(null, new RoomOptions() { maxPlayers = 4 }, null);
	}

	// the following methods are implemented to give you some context. re-implement them as needed.

	public virtual void OnFailedToConnectToPhoton(DisconnectCause cause)
	{
		if( ImageStatus != null )
			ImageStatus.color = Color.red;
		
		//Debug.LogError("Cause: " + cause);
	}

	public void OnJoinedRoom()
	{
		if( ImageStatus != null )
			ImageStatus.color = Color.green;
		
		//Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room. From here on, your game would be running. For reference, all callbacks are listed in enum: PhotonNetworkingMessage");
	}

	public void Connect(){
		PhotonNetwork.ConnectUsingSettings(null);
	}

	public void Disonnect(){
		PhotonNetwork.Disconnect();
	}
}
