// ##############################################################################
//
// ICECreaturePUNAdapterEditor.cs
// Version 1.2.10
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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.AnimatedValues;

using ICE.Creatures.EnumTypes;
using ICE.Creatures;
using ICE.Creatures.Objects;
using ICE.Creatures.Attributes;
using ICE.Creatures.EditorInfos;
using ICE.Creatures.EditorHandler;
using ICE.Styles;
using ICE.Layouts;

namespace ICE.Creatures.Adapter
{

	[CustomEditor(typeof(ICECreaturePUNAdapter))]//, CanEditMultipleObjects]
	public class ICECreaturePUNAdapterEditor : Editor 
	{
		private ICECreaturePUNAdapter m_target;
		private PhotonView m_PhotonView = null;
		private PhotonTransformView m_PhotonTransformView = null;

		public override void OnInspectorGUI()
		{
			if( m_target == null )
				m_target = (ICECreaturePUNAdapter)target;

			GUI.changed = false;
			Info.HelpButtonIndex = 0;
			EditorGUILayout.Separator();

			m_PhotonView = m_target.GetComponent<PhotonView>(); 
			m_PhotonTransformView = m_target.GetComponent<PhotonTransformView>(); 

			if( m_PhotonView == null )
			{
				if( ICEEditorLayout.Button( "Add Photon View", "", ICEEditorStyle.ButtonExtraLarge ) )
					m_PhotonView = m_target.gameObject.AddComponent<PhotonView>();
			}
			else
			{
				EditorGUI.BeginDisabledGroup( m_PhotonView.ObservedComponents.FindIndex( _comp => _comp != null &&_comp.GetType() == typeof(ICECreaturePUNAdapter) ) >= 0 );
					if( ICEEditorLayout.Button( "Observe Photon Creature View", "", ICEEditorStyle.ButtonExtraLarge ) )
						m_PhotonView.ObservedComponents.Add( m_target );
				EditorGUI.EndDisabledGroup();

				if( m_PhotonTransformView == null )
				{
					if( ICEEditorLayout.Button( "Add Photon Transform View", "", ICEEditorStyle.ButtonExtraLarge ) )
						m_PhotonTransformView = m_target.gameObject.AddComponent<PhotonTransformView>();
				}
				else
				{
					EditorGUI.BeginDisabledGroup( m_PhotonView.ObservedComponents.FindIndex( _comp => _comp != null &&_comp.GetType() == typeof(PhotonTransformView) ) >= 0 );
					if( ICEEditorLayout.Button( "Observe Photon Transform View", "", ICEEditorStyle.ButtonExtraLarge ) )
						m_PhotonView.ObservedComponents.Add( m_PhotonTransformView );
					EditorGUI.EndDisabledGroup();
				}
			}



			/*
			m_target.UseCreatePlayer = ICEEditorLayout.Toggle( "Spawning Player", "", m_target.UseCreatePlayer , "" );

			EditorGUI.BeginDisabledGroup( m_target.UseCreatePlayer == false );
				EditorGUI.indentLevel++;
					m_target.ReferencePlayerName = EditorHandler.Popups.PlayerPopup("Player","", m_target.ReferencePlayerName, "" );

					EditorGUILayout.Separator();
					m_target.UsePlayerCamera = ICEEditorLayout.Toggle( "Assign Player Camera", "", m_target.UsePlayerCamera , "" );
					EditorGUI.BeginDisabledGroup( m_target.UsePlayerCamera == false );
						EditorGUI.indentLevel++;
							m_target.PlayerCamera = (Camera)EditorGUILayout.ObjectField( "Camera", m_target.PlayerCamera, typeof(Camera), true );
							m_target.PlayerCameraAssignMethod = ICEEditorLayout.Text( "Method", "", m_target.PlayerCameraAssignMethod , "");
						EditorGUI.indentLevel--;
					EditorGUI.EndDisabledGroup();
				EditorGUI.indentLevel--;
			EditorGUI.EndDisabledGroup();
			*/
			EditorGUILayout.Separator();
			if (GUI.changed)
				EditorUtility.SetDirty( m_target );
		}
	}
}
