using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class ScreenShotMenu : EditorWindow
{
    public string path;

    private void OnEnable()
    {
        path = Application.dataPath;
    }

    public Camera camera;
    //private Camera camera = SceneView.currentDrawingSceneView.camera;
    public string[] aspectRatios = new string[] { "16x9", "16x10" };
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

    public int i = 0;
    public AspectRatio currentAspectRatio;


    void OnGUI()
    {
        var origFontStyle = EditorStyles.label.fontStyle;
        EditorStyles.label.fontStyle = FontStyle.Bold;
        EditorGUILayout.LabelField("Select Aspect Ratio");
        EditorStyles.label.fontStyle = origFontStyle;

        ratioIndex = EditorGUILayout.Popup(ratioIndex, aspectRatios);

        EditorStyles.label.fontStyle = FontStyle.Bold;
        EditorGUILayout.LabelField("Select  Resolution");
        EditorStyles.label.fontStyle = origFontStyle;

        resolutionIndex = EditorGUILayout.Popup(resolutionIndex, GetResolutionArr());
        camera = (Camera)EditorGUILayout.ObjectField("Drag camera here:", camera, typeof(Camera), true);



        if (GUILayout.Button(path == Application.dataPath ? "Choose Directory" : path))
        {
            var path = EditorUtility.OpenFolderPanel("Choose directory", " fasasf", "ffffff");
            this.path = path;
        }

        if (GUILayout.Button("Shoot"))
        {
            resolutionData = GetResolutionDictionary();
            chosenResolutionArray = GetResolutionArr();
            MakeShot();
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
        switch (ratioIndex)
        {
            case 0:
                a = resolution_16x9;
                currentAspectRatio = AspectRatio.SixteenByNine;
                break;
            case 1:
                a = resolution_16x10;
                currentAspectRatio = AspectRatio.SixteenByTen;
                break;
        }
        if (a == null) Debug.LogError("resolution Array is null");
        return a;
    }

    public void MakeShot()
    {
        int w = (int)resolutionData[chosenResolutionArray[resolutionIndex]].x;
        int h = (int)resolutionData[chosenResolutionArray[resolutionIndex]].y;

        RenderTexture rt = new RenderTexture(w, h, 24);
        camera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(w, h, TextureFormat.RGB24, false);
        camera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, w, h), 0, 0);
        camera.targetTexture = null;
        RenderTexture.active = null;
        byte[] bytes = screenShot.EncodeToPNG();
        System.IO.File.WriteAllBytes((path == Application.dataPath) ? Application.dataPath : path + "/1.png", bytes);
    }
}

public enum AspectRatio
{
    SixteenByNine, SixteenByTen,
}



