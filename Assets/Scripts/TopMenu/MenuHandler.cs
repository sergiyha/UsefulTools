using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;


public class MenuHandler : MonoBehaviour
{

	public static event Action OnCreateScreenshot;
	

	[MenuItem("UsefulTools/Open Screenshot Menu")]
	static void CreateScreenShot()
	{
		OnCreateScreenshot();
	}

	private static bool IsHadlerIsExist(Action _event)
	{
		if (_event != null)
			return true;
		else return false;
	}

	private static void ExecuteEvent(Action _event)
	{
		if (IsHadlerIsExist(_event))
		{
			_event();
		}
		else
		{
			Debug.LogError("Event " + _event.GetType().Name + "is unsubscribed !!!");
		}
	}


}
