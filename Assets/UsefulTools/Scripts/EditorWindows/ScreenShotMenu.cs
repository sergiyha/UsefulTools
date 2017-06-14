using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class ScreenShotMenu : EditorWindow
{
	private SerializedProperty stringsProperty;
	private SerializedObject serializedCameraObj;
	private CamerasScriptableObject cameraScriptableObject;

	public Vector2 cameraScrollView = Vector2.zero;
	public string Path;
	const int maxDimention = 10000;
	const int minDimention = 1;
	public string inputName;
	public string inputPath;
	bool isTransparent;

	public int w = 1;
	public int h = 1;

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

	public AspectRatioStates currentAspectRatio;

	private List<OutOfRangeStates> statesOnThisFrame = new List<OutOfRangeStates>();

	private void PainTransparancyToggle()
	{
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Transparency: ", GUILayout.MaxWidth(100f));
		isTransparent = EditorGUILayout.Toggle(isTransparent);
		EditorGUILayout.EndHorizontal();
	}


	private void OnEnable()
	{
		Path = Application.dataPath;
		CreateSerializedCameraProperty();
	}

	private void CreateSerializedCameraProperty()
	{
		cameraScriptableObject = CreateInstance<CamerasScriptableObject>();
		ScriptableObject cameraScriptable = cameraScriptableObject;
		serializedCameraObj = new SerializedObject(cameraScriptable);
		stringsProperty = serializedCameraObj.FindProperty("cameras");
	}

	private void OnGUI()
	{

		EnshureThatSerializedObjectWasntDestroyed();

		PaintAspectRatio();
		AspectChecker();
		PaintResolution();
		PainTransparancyToggle();
		PaintCameras();
		PaintPathToSaveBlock();
		PaintShotButton();
		PaintPreviev();

		CheckIfShowCamerasExistingNotification();
		OutOfRangeChecker();
	}


	private bool cameraExistingNotificationHolder = false;
	private void CheckIfShowCamerasExistingNotification()
	{
		if (cameraExistingNotificationHolder)
		{
			DialogDisplayerManager.Instance.ThereIsNoCameras();
		}
	}


	private void PaintPreviev()
	{
		if (GUILayout.Button("Preview"))
		{
			if (IsCameraExist())
			{
				var prevWindow = GetWindow<PreviewWindow>();
				prevWindow.LoadDataToPreviev(ref cameraScriptableObject.cameras, w, h, true);
			}
			else
			{
				cameraExistingNotificationHolder = true;
			}
		}
	}

	private bool IsCameraExist()
	{
		bool isCameraExist = false;
		if (cameraScriptableObject.cameras == null) return false;
		foreach (var camera in cameraScriptableObject.cameras)
		{
			if (camera != null)
			{
				isCameraExist = true;
				break;
			}
		}
		return isCameraExist;
	}

	private float timeToShowNotification = 3f;

	private void Update()
	{
		CheckIfCameraNotificationIsExist();
	}

	private void CheckIfCameraNotificationIsExist()
	{
		if (cameraExistingNotificationHolder)
		{
			timeToShowNotification -= 0.01f;

			if (timeToShowNotification <= 0f)
			{
				cameraExistingNotificationHolder = false;
				timeToShowNotification = 3.0f;
				Repaint();
			}
		}
	}

	private void PaintShotButton()
	{
		if (GUILayout.Button("Shot"))
		{
			if (IsCameraExist())
			{
				MakeShot(w, h, GetCorrectTextureFormat());
			}
			else
			{
				cameraExistingNotificationHolder = true;
			}
		}
	}

	void SetWidthAndHeightIfCustom()
	{
		resolutionData = GetResolutionDictionary();
		chosenResolutionArray = GetResolutionArr();
		w = (int)resolutionData[chosenResolutionArray[resolutionIndex]].x;
		h = (int)resolutionData[chosenResolutionArray[resolutionIndex]].y;
	}


	private TextureFormat GetCorrectTextureFormat()
	{
		return isTransparent ? TextureFormat.ARGB32 : TextureFormat.RGB24;
	}

	private void EnshureThatSerializedObjectWasntDestroyed()
	{
		if (serializedCameraObj.targetObject == null)
		{
			CreateSerializedCameraProperty();
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
			cameraScrollView = EditorGUILayout.BeginScrollView(cameraScrollView, GUILayout.Width(position.width), GUILayout.Height(300));
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

	private void PaintResolution()
	{
		EditorStyles.label.fontStyle = FontStyle.Bold;
		EditorGUILayout.LabelField("Select  Resolution");
		EditorStyles.label.fontStyle = EditorStyles.label.fontStyle;


		if (currentAspectRatio != AspectRatioStates.Manualy)
		{
			resolutionIndex = EditorGUILayout.Popup(resolutionIndex, GetResolutionArr());
			SetWidthAndHeightIfCustom();
		}
		else
		{
			EditorGUILayout.BeginHorizontal("box");
			EditorGUILayout.LabelField("Width:", GUILayout.MaxWidth(40f));
			w = EditorGUILayout.IntField(SetCorrectDimention(w));
			EditorGUILayout.LabelField("Height:", GUILayout.MaxWidth(45f));
			h = EditorGUILayout.IntField(SetCorrectDimention(h));
			Repaint();
			EditorGUILayout.EndHorizontal();
		}
	}

	private void PaintAspectRatio()
	{
		EditorStyles.label.fontStyle = FontStyle.Bold;
		EditorGUILayout.LabelField("Select Aspect Ratio");
		EditorStyles.label.fontStyle = EditorStyles.label.fontStyle;

		ratioIndex = EditorGUILayout.Popup(ratioIndex, aspectRatios);
	}


	private void AspectChecker()
	{
		switch (ratioIndex)
		{
			case 0:
				currentAspectRatio = AspectRatioStates.SixteenByNine;
				break;
			case 1:
				currentAspectRatio = AspectRatioStates.SixteenByTen;
				break;
			case 2:
				currentAspectRatio = AspectRatioStates.Manualy;
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
			case AspectRatioStates.SixteenByNine:
				a = resolution_16x9;
				break;
			case AspectRatioStates.SixteenByTen:
				a = resolution_16x10;
				break;
			case AspectRatioStates.Manualy:
				a = null;
				break;
		}
		if (a == null) Debug.LogError("resolution Array is null");
		return a;
	}


	public void MakeShot(int w, int h, TextureFormat textureFormat = TextureFormat.RGB24)
	{
		int i = 0;
		foreach (var camera in cameraScriptableObject.cameras)
		{
			if (camera == null) continue;
			i++;
			RenderTexture rt = new RenderTexture(w, h, 24);

			camera.targetTexture = rt;
			Texture2D screenShot = new Texture2D(w, h, textureFormat, false);
			camera.Render();
			RenderTexture.active = rt;
			screenShot.ReadPixels(new Rect(0, 0, w, h), 0, 0);
			camera.targetTexture = null;
			RenderTexture.active = null;
			byte[] bytes = screenShot.EncodeToPNG();
			System.IO.File.WriteAllBytes((Path == Application.dataPath) ? Application.dataPath + "/" + inputName + "_" + i + ".png" : Path + "/" + inputName + "_" + i + ".png", bytes);
		}
	}

	private void OutOfRangeChecker()
	{
		if (statesOnThisFrame.Contains(OutOfRangeStates.MaxOutOfRange))
		{
			DialogDisplayerManager.Instance.HeightOrWidthIsGreaterThanRange();
		}
		else if (statesOnThisFrame.Contains(OutOfRangeStates.MinOutOfRange))
		{
			DialogDisplayerManager.Instance.HeightOrWidthIsLessThanRange();
		}
		else if (statesOnThisFrame.Contains(OutOfRangeStates.None))
		{
			return;
		}

		statesOnThisFrame.Clear();
	}

	private int SetCorrectDimention(int dimention)
	{
		Func<int> getMaxDimention = () =>
		{
			statesOnThisFrame.Add(OutOfRangeStates.MaxOutOfRange);
			return maxDimention;
		};

		Func<int> getMinDimention = () =>
		{
			statesOnThisFrame.Add(OutOfRangeStates.MinOutOfRange);
			return minDimention;
		};

		Func<int> getDimention = () =>
		{
			statesOnThisFrame.Add(OutOfRangeStates.None);
			return dimention;
		};
		int retValue = 0;

		retValue = (dimention <= 0 || dimention == minDimention)
				   ? getMinDimention() :
				   (dimention >= maxDimention || dimention == maxDimention)
				   ? getMaxDimention() : getDimention();
		return retValue;
	}
}





