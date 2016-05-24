// ##############################################################################
//
// ICECreatureRegisterPUNAdapter.cs
// Version 1.1.21
//
// © Pit Vetterick, ICE Technologies Consulting LTD. All Rights Reserved.
// http://www.icecreaturecontrol.com
// mailto:support@icecreaturecontrol.com
// 
// Unity Asset Store End User License Agreement (EULA)
// http://unity3d.com/legal/as_terms
//
// ##############################################################################


using UnityEngine;
using System.Collections;

using ICE;
using ICE.World;

using ICE.Creatures;
using ICE.Creatures.Objects;
using ICE.Creatures.EnumTypes;
using ICE.Utilities;

namespace ICE.Creatures.Adapter
{
	[RequireComponent (typeof (ICECreatureRegister))]
	public class ICECreatureRegisterPUNAdapter : Photon.MonoBehaviour {

		public bool UsePlayerCamera = true;
		public Camera PlayerCamera = null;
		public string PlayerCameraAssignMethod = "AssignCameraTarget";

		public bool UseRenamingPlayer = true;
		public string PlayerName = "Player";

		public string ReferencePlayerName = "";
		public bool UseMultiplayer = true;
		public bool UseCreatePlayer = true;
		public int MaxPlayersPerRoom = 10;

		private ICECreatureRegister m_Register = null;
		void Awake(){
			m_Register = ICECreatureRegister.Instance;

			if( m_Register == null )
				return;

			m_Register.NetworkAdapter = NetworkAdapterType.PUN;

			m_Register.OnDestroyObject += OnDestroyObject;
			m_Register.OnSpawnObject += OnSpawnObject;


			// deactivates the pool management if it is not the master
			if( PhotonNetwork.isMasterClient == false )
			{
				//m_Register.UsePoolManagement = false;
			}
		}
		// Use this for initialization
		void Start () {


		}

		/// <summary>
		/// Raises the enable event.
		/// </summary>
		protected virtual void OnEnable()
		{
			ICEWorldInfo.IsMultiplayer = UseMultiplayer;
		}

		/// <summary>
		/// Raises the disable event.
		/// </summary>
		protected virtual void OnDisable()
		{
			ICEWorldInfo.IsMultiplayer = false;
		}

		/// <summary>
		/// Update this instance.
		/// </summary>
		void Update () {

		}

		// CREATURE REGISTER 

		/// <summary>
		/// Raises the spawn object event.
		/// </summary>
		/// <param name="_object">Object.</param>
		/// <param name="_reference">Reference.</param>
		/// <param name="_position">Position.</param>
		/// <param name="_rotation">Rotation.</param>
		public void OnSpawnObject( out GameObject _object, GameObject _reference, Vector3 _position, Quaternion _rotation )
		{
			_object = null;

			if( PhotonNetwork.isMasterClient == true || _reference.GetComponent<ICECreaturePlayer>() != null )
				_object = PhotonNetwork.InstantiateSceneObject( _reference.name, _position, _rotation, 0, null);
		}

		/// <summary>
		/// Raises the destroy object event.
		/// </summary>
		/// <param name="_object">Object.</param>
		/// <param name="_destroyed">Destroyed.</param>
		public void OnDestroyObject( GameObject _object, out bool _destroyed )
		{
			if( PhotonNetwork.isMasterClient == true && _object != null && _object.GetComponent<PhotonView>() != null ){
				PhotonNetwork.Destroy( _object );
				_destroyed = true;
			}
			else
				_destroyed = false;
		}

		// PHOTON NETWORK

		public void OnJoinedRoom()
		{
			if( PhotonNetwork.isMasterClient )
				m_Register.InitialSpawn();

			if( PhotonNetwork.isMasterClient )
				PhotonNetwork.room.maxPlayers = MaxPlayersPerRoom;

			// TODO: use PhotonNetwork.LoadLevel to load level while automatically pausing network queue
			// ("call this in OnJoinedRoom to make sure no cached RPCs are fired in the wrong scene")
			// also, get level from room properties / master

			// send spawn request to master client
			string name = "Unnamed";

			// sent as RPC instead of in 'OnPhotonPlayerConnected' because the
			// MasterClient does not run the latter for itself + we don't want
			// to do the request on all clients

			//if(FindObjectOfType<vp_MPMaster>())	// in rare cases there might not be a vp_MPMaster, for example: a chat lobby
			//	photonView.RPC("RequestInitialSpawnInfo", PhotonTargets.MasterClient, PhotonNetwork.player, 0, name);

			ICEWorldInfo.IsMasterClient = PhotonNetwork.isMasterClient;

			if( UseCreatePlayer ) 
			{
				// gets the refrence group of your selected reference player
				ReferenceGroupObject _group = m_Register.GetGroupByName( ReferencePlayerName );

				if( _group != null )
				{
					_group.MaxCoexistingObjects = MaxPlayersPerRoom;
					//GameObject _player = _group.Spawn();

					Vector3 position = _group.GetSpawnPosition();

					GameObject _player = PhotonNetwork.Instantiate( ReferencePlayerName, position, Quaternion.identity, 0 );

					if( _player != null )
					{
						if( UseRenamingPlayer && ! string.IsNullOrEmpty( PlayerName ) )
						{
							_player.name = PlayerName; 
						}

						if( UsePlayerCamera && PlayerCamera != null && ! string.IsNullOrEmpty( PlayerCameraAssignMethod ) )
						{
							/*
							 	// Your Camara script should contain something like this to assign the created player as 
								// target for your camera ...
							 	public void AssignCameraTarget( GameObject _object )
								{
									if( _object == null )
										return;

									Target = _object.transform;
								}
							 */
							PlayerCamera.SendMessageUpwards( PlayerCameraAssignMethod, _player, SendMessageOptions.DontRequireReceiver );	
						}
					}
				}
			}

			m_Register.NetworkConnectedAndReady = true;
			Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room. From here on, your game would be running. For reference, all callbacks are listed in enum: PhotonNetworkingMessage");
		}
		/*
		public virtual void OnConnectedToMaster()
		{
			Debug.Log("OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room. Calling: PhotonNetwork.JoinRandomRoom();");
			//PhotonNetwork.JoinRandomRoom();
		}
	
		public virtual void OnJoinedLobby()
		{
			Debug.Log("OnJoinedLobby(). This client is connected and does get a room-list, which gets stored as PhotonNetwork.GetRoomList(). This script now calls: PhotonNetwork.JoinRandomRoom();");
			//PhotonNetwork.JoinRandomRoom();
		}

		public virtual void OnPhotonRandomJoinFailed()
		{
			Debug.Log("OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one. Calling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");
			//PhotonNetwork.CreateRoom(null, new RoomOptions() { maxPlayers = 4 }, null);
		}

		// the following methods are implemented to give you some context. re-implement them as needed.

		public virtual void OnFailedToConnectToPhoton(DisconnectCause cause)
		{
			Debug.LogError("Cause: " + cause);
		}


		protected virtual void Connect()
		{

			//PhotonNetwork.ConnectUsingSettings("0.1");

		}


		/// <summary>
		/// used internally to disconnect and immediately reconnect
		/// </summary>
		protected virtual void Reconnect()
		{

			if (PhotonNetwork.connectionStateDetailed != PeerState.Disconnected
				&& PhotonNetwork.connectionStateDetailed != PeerState.PeerCreated)
			{
				PhotonNetwork.Disconnect();
			}

			Connect();

		//	m_LastPeerState = PeerState.Uninitialized;

		}

*/
	}
}





