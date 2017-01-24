using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class ScreenShotMenuExecutor
{
	static ScreenShotMenuExecutor()
	{
		MenuHandler.OnCreateScreenshot += () =>
		{
			var window = EditorWindow.GetWindow(typeof(ScreenShotMenu));
			window.minSize = new Vector2(300, 400);
		};
	}

}
