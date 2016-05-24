// ##############################################################################
//
// ICECreaturePUNAdapter.cs
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
	[RequireComponent (typeof (ICECreatureControl))]
	public class ICECreaturePUNAdapter : Photon.MonoBehaviour {

		protected ICECreatureControl m_Control = null;

		protected float m_LastSynchronizationTime = 0f;
		protected float m_SyncDelay = 0f;
		protected float m_SyncTime = 0f;
		protected Vector3 m_SyncStartPosition = Vector3.zero;
		protected Vector3 m_SyncEndPosition = Vector3.zero;

		public virtual void Awake()
		{
			m_Control = transform.GetComponent<ICECreatureControl>(); 
		}

		public virtual void OnPhotonSerializeView( PhotonStream stream, PhotonMessageInfo info )
		{
			if( stream.isWriting )
			{
				stream.SendNext( m_Control.BehaviourModeKey );	
			}
			else     												// !stream.isWriting
			{
				m_Control.BehaviourModeKey = (string)stream.ReceiveNext();

				/*m_SyncEndPosition = (Vector3)stream.ReceiveNext();
				m_SyncStartPosition = transform.position;*/

				m_SyncTime = 0f;
				m_SyncDelay = Time.time - m_LastSynchronizationTime;
				m_LastSynchronizationTime = Time.time;
			}
		}
	}
}





