using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class ScreenShotMenu : EditorWindow
{
	void OnEnable()
	{
		Path = Application.dataPath;
		cameraScriptableObject = CreateInstance<CamerasScriptableObject>();
		ScriptableObject cameraScriptable = cameraScriptableObject;
		serializedCameraObj = new SerializedObject(cameraScriptable);
		stringsProperty = serializedCameraObj.FindProperty("cameras");

	}

	private SerializedProperty stringsProperty;
	private SerializedObject serializedCameraObj;
	private CamerasScriptableObject cameraScriptableObject;


	public Vector2 cameraScrollView = Vector2.zero;
	public string Path;
	const int maxDimention = 10000;
	public string inputName;
	public string inputPath;
	bool camerasIsCreated;

	public int w;
	public int h;

	private int camerasCount;

	public string[] aspectRatios = new string[] { "16x9", "16x10", "Manualy" };

	public int ratioIndex;


	public string[] resolution_16x9 = new string[] {
		"1920x1080",
		"3840x2160",
		"5120x2880",
		"7680x4320",

	};

	public string[] resolution_16x10 = new string[]
	{
		"1440x900",
		"1680x1050",
		"1920x1200",
		"2560x1600"
	};
	public int resolutionIndex;

	Dictionary<string, Vector2> resolutionData;
	string[] chosenResolutionArray;

	public AspectRatio currentAspectRatio;



	void ResolutionBlock()
	{

	}

	void OnGUI()
	{
		var origFontStyle = EditorStyles.label.fontStyle;
		PaintAspectRatio(origFontStyle);
		AspectChecker();
		PaintResolution(origFontStyle);
		PaintCameras();
		PaintPathToSaveBlock();
		PaitShotButton();
	}

	private void PaitShotButton()
	{
		if (GUILayout.Button("Shoot"))
		{
			if (currentAspectRatio == AspectRatio.SixteenByNine || currentAspectRatio == AspectRatio.SixteenByTen)
			{
				resolutionData = GetResolutionDictionary();
				chosenResolutionArray = GetResolutionArr();
				w = (int)resolutionData[chosenResolutionArray[resolutionIndex]].x;
				h = (int)resolutionData[chosenResolutionArray[resolutionIndex]].y;
			}
			else if (currentAspectRatio == AspectRatio.NON)
			{

			}

			MakeShot(w, h);
		}
	}

	private void PaintCameras()
	{
		Action drawCamera = () =>
		{
			EditorGUILayout.PropertyField(stringsProperty, true);
			serializedCameraObj.ApplyModifiedProperties();
		};


		if (cameraScriptableObject.cameras == null || cameraScriptableObject.cameras.Length == 0 || cameraScriptableObject.cameras.Length <= 15)// if camera null
		{
			drawCamera();
		}
		else//if camera not null
		{
			cameraScrollView = EditorGUILayout.BeginScrollView(cameraScrollView, GUILayout.Width(300), GUILayout.Height(300));
			drawCamera();
			Repaint();
			EditorGUILayout.EndScrollView();

		}

		inputName = EditorGUILayout.TextField("Choose name: ", inputName);
	}

	private void PaintPathToSaveBlock()
	{
		EditorGUILayout.BeginHorizontal("box");
		EditorGUILayout.LabelField(Path);
		if (GUILayout.Button("Choose Directory", GUILayout.MaxWidth(120f)))
		{
			var path = EditorUtility.OpenFolderPanel("Choose Directory", "", "");
			Path = path;
		}
		if (Path == "")
		{
			Path = Application.dataPath;
		}

		EditorGUILayout.EndHorizontal();
	}

	private void PaintResolution(FontStyle origFontStyle)
	{
		EditorStyles.label.fontStyle = FontStyle.Bold;
		EditorGUILayout.LabelField("Select  Resolution");
		EditorStyles.label.fontStyle = origFontStyle;


		if (currentAspectRatio != AspectRatio.NON)
		{
			resolutionIndex = EditorGUILayout.Popup(resolutionIndex, GetResolutionArr());
		}
		else
		{
			Rect r = EditorGUILayout.BeginHorizontal("box");
			EditorGUILayout.LabelField("Width:", GUILayout.MaxWidth(40f));
			w = EditorGUILayout.IntField(w = (w > maxDimention) ? maxDimention : w);
			EditorGUILayout.LabelField("Height:", GUILayout.MaxWidth(40f));
			h = EditorGUILayout.IntField(h = (h > maxDimention) ? maxDimention : h);
			Repaint();
			EditorGUILayout.EndHorizontal();
		}
	}

	private void PaintAspectRatio(FontStyle origFontStyle)
	{
		EditorStyles.label.fontStyle = FontStyle.Bold;
		EditorGUILayout.LabelField("Select Aspect Ratio");
		EditorStyles.label.fontStyle = origFontStyle;

		ratioIndex = EditorGUILayout.Popup(ratioIndex, aspectRatios);
	}


	private void AspectChecker()
	{
		switch (ratioIndex)
		{
			case 0:
				currentAspectRatio = AspectRatio.SixteenByNine;
				break;
			case 1:
				currentAspectRatio = AspectRatio.SixteenByTen;
				break;
			case 2:
				currentAspectRatio = AspectRatio.NON;
				break;
		}
	}


	private void CreateResolutionData()
	{
		resolutionData = new Dictionary<string, Vector2>();
	}

	private Dictionary<string, Vector2> GetResolutionDictionary()
	{
		var dataResDict = new Dictionary<string, Vector2>();
		foreach (var item in GetResolutionArr())
		{
			int i = item.IndexOf("x");
			dataResDict.Add(
				item,
				new Vector2((float.Parse(item.Substring(0, i))), float.Parse(item.Substring(i + 1, (item.Length - 1) - i))));
		}
		return dataResDict;
	}


	private string[] GetResolutionArr()
	{
		string[] a = null;
		switch (currentAspectRatio)
		{
			case AspectRatio.SixteenByNine:
				a = resolution_16x9;
				break;
			case AspectRatio.SixteenByTen:
				a = resolution_16x10;
				break;
			case AspectRatio.NON:
				a = null;
				break;
		}
		if (a == null) Debug.LogError("resolution Array is null");
		return a;
	}


	public void MakeShot(int w, int h)
	{
		RenderTexture rt = new RenderTexture(w, h, 24);
		//	camera.targetTexture = rt;
		Texture2D screenShot = new Texture2D(w, h, TextureFormat.RGB24, false);
		//camera.Render();
		RenderTexture.active = rt;
		screenShot.ReadPixels(new Rect(0, 0, w, h), 0, 0);
		//camera.targetTexture = null;
		RenderTexture.active = null;
		byte[] bytes = screenShot.EncodeToPNG();
		System.IO.File.WriteAllBytes((Path == Application.dataPath) ? Application.dataPath : Path + "/" + inputName + ".png", bytes);
	}
}

public enum AspectRatio
{
	SixteenByNine, SixteenByTen, NON
}



