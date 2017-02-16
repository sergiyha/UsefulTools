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
			window.minSize = new Vector2(400, 400);
			window.maxSize = new Vector2(600, 600);
		};
	}

}
