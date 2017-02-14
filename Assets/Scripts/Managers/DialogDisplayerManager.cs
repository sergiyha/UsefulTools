using UnityEditor;
using System;
using UnityEngine;

public class DialogDisplayerManager
{
	private static DialogDisplayerManager _instance;
	public static DialogDisplayerManager Instance { get { return _instance = _instance ?? new DialogDisplayerManager(); } }


	private void DisplayNotification(string message, MessageType messageType, bool isWide)
	{
		EditorGUILayout.HelpBox(message, messageType, isWide);
	}



	public void HeightOrWidthIsGreaterThanRange()
	{
		DisplayNotification("Attention: width and hight can't be greater than 10K pxls.", MessageType.Warning, true);
	}

	public void HeightOrWidthIsLessThanRange()
	{
		DisplayNotification("Attention: width and hight can't be less than 1 pxls.", MessageType.Warning, true);
	}

	public void ThereIsNoCameras()
	{
		DisplayNotification("There is no one camera to render. Drag and drop camera to empty field", MessageType.Warning, true);
	}
}
