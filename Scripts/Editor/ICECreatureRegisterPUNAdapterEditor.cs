// ##############################################################################
//
// ICECreatureRegisterPUNAdapterEditor.cs
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

	[CustomEditor(typeof(ICECreatureRegisterPUNAdapter)), CanEditMultipleObjects]
	public class ICECreatureRegisterPUNAdapterEditor : Editor 
	{
		private ICECreatureRegisterPUNAdapter m_target;

		public override void OnInspectorGUI()
		{
			if( m_target == null )
				m_target = (ICECreatureRegisterPUNAdapter)target;

			GUI.changed = false;
			Info.HelpButtonIndex = 0;
			EditorGUILayout.Separator();

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
			EditorGUILayout.Separator();
			if (GUI.changed)
				EditorUtility.SetDirty( m_target );
		}
	}
}
