using UnityEditor;
using System;
using UnityEngine;

public class DialogDisplayerManager
{
	private static DialogDisplayerManager _instance;
	public static DialogDisplayerManager Instance { get { return _instance ?? new DialogDisplayerManager(); } }

	public void DisplayNotification(string message, MessageType messageType, bool isWide)
	{
		EditorGUILayout.HelpBox(message, messageType, isWide);
	}


	public void HeightOrWidthIsGreaterThanRange()
	{
		DisplayNotification("Attention: width and hight can't be greater then 10000 pxls.", MessageType.Warning, true);
	}

	public void HeightOrWidthIsLessThanRange()
	{
		DisplayNotification("Attention: width and hight can't be less then 1 pxls.", MessageType.Warning, true);
	}
}
