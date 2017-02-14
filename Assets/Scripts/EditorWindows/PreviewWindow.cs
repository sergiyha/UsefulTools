using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class PreviewWindow : EditorWindow
{
	private Vector2 aspectToPreviev;
	private Dictionary<string, Texture2D> texturesData;
	private bool isTranspatented;
	private const int maxHeight = 200;
	private const int maxWidth = 300;

	private Vector2 previewScrollView = new Vector2(20, 20);

	private Vector3 currentPreviewWidthHeight;
	private Camera[] tempCameras;

	public void LoadDataToPreviev(ref Camera[] cameras, int width, int height, bool isTranspatented)
	{
		tempCameras = cameras;
		this.isTranspatented = isTranspatented;
		int gcd = BasicMath.LeastCommonMultiple.gcf(width, height);
		aspectToPreviev = new Vector2((int)width / gcd, (int)height / gcd);
		currentPreviewWidthHeight = GetcurrentHeighAndWidth(aspectToPreviev);

		MakePreviewShot(
			((int)currentPreviewWidthHeight.x > width) ? width : ((int)currentPreviewWidthHeight.x < 1) ? 1 : (int)currentPreviewWidthHeight.x,
			((int)currentPreviewWidthHeight.y > height) ? height : ((int)currentPreviewWidthHeight.y < 1) ? 1 : ((int)currentPreviewWidthHeight.y),
			ref cameras);
	}
	private Vector2 SetMaxHandW(int count)
	{
		int maxHeight = PreviewWindow.maxHeight;
		int maxWidth = PreviewWindow.maxWidth;
		switch (count)
		{
			case 1:
				maxHeight *= 3;
				maxWidth *= 3;
				break;
			case 2:
				maxHeight *= 2;
				maxWidth *= 2;
				break;
			default:
				maxHeight *= 1;
				maxWidth *= 1;
				break;
		}
		return new Vector2(maxWidth, maxHeight);
	}

	private int GetNotNullCameraCount(Camera[] cameras)
	{
		int cameraCounts = 0;
		foreach (var camera in cameras)
		{
			if (camera != null) cameraCounts++;
		}
		return cameraCounts;
	}

	private Vector2 GetcurrentHeighAndWidth(Vector2 aspectRatio)
	{
		Func<Vector2, float> getMultiplier = (aspectRatioCheck) =>
		  {
			  return (aspectRatioCheck.x > aspectRatioCheck.y) ?
			  (float)(SetMaxHandW(GetNotNullCameraCount(tempCameras)).x / aspectRatioCheck.x) :
			  (float)(SetMaxHandW(GetNotNullCameraCount(tempCameras)).y / aspectRatioCheck.y);
		  };
		float multiplier = getMultiplier(aspectRatio);
		return aspectRatio * multiplier;
	}


	private void OnGUI()
	{
		DrawPreview();
	}
	private void DrawPreview()
	{
		int offsetWidth = 5;
		int offsetHeight = 15;
		int columnsCount = (int)(this.position.width / (currentPreviewWidthHeight.x - offsetWidth));
		int rowsCount = GetCountRows(columnsCount);

		int itemCountInRow = 0;
		int rowsCounter = 0;
		int i = 0;

		Action drawPreviews = () =>
		{
			foreach (var texture in texturesData)
			{

				if (itemCountInRow == 0 && columnsCount == 0)
				{
					columnsCount = 1;
				}

				GUI.DrawTexture(new Rect((currentPreviewWidthHeight.x + offsetWidth) * itemCountInRow,
										 (currentPreviewWidthHeight.y + offsetHeight) * rowsCounter,
										 currentPreviewWidthHeight.x,
										 currentPreviewWidthHeight.y),
										 texture.Value);

				GUI.Label(new Rect(((currentPreviewWidthHeight.x + offsetWidth) * itemCountInRow) + currentPreviewWidthHeight.x / 2,
									((currentPreviewWidthHeight.y + offsetHeight) * rowsCounter) + currentPreviewWidthHeight.y,
								   100,
								   100)
								   , texture.Key);
				itemCountInRow++;

				if (itemCountInRow == columnsCount)
				{
					itemCountInRow = 0;
					rowsCounter++;
				}

			}
		};

		if ((rowsCount * (currentPreviewWidthHeight.y + offsetWidth)) > this.position.height)
		{
			previewScrollView = GUI.BeginScrollView(new Rect(0, 0, this.position.width, this.position.height),
													previewScrollView,
													new Rect(0, 0, 0, (rowsCount * (currentPreviewWidthHeight.y + offsetHeight))));
			drawPreviews();
			GUI.EndScrollView();
		}
		else
		{
			drawPreviews();
		}
	}


	private int GetCountRows(int columns)
	{
		int itemCountInRow = 0;
		int rowsCount = 0;
		if (columns == 0)
		{
			return texturesData.Count;
		}
		foreach (var texture in texturesData)
		{

			if (itemCountInRow == columns)
			{
				itemCountInRow = 0;
				rowsCount++;
			}
			itemCountInRow++;
		}
		return rowsCount + 1;
	}



	public void MakePreviewShot(int w, int h, ref Camera[] cameras, TextureFormat textureFormat = TextureFormat.ARGB32)// TextureFormat.RGB24)
	{
		texturesData = new Dictionary<string, Texture2D>();
		int i = 0;
		foreach (var camera in cameras)
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
			screenShot.Apply();//Todo same camera names
			texturesData.Add(camera.name, screenShot);
			Caching.CleanCache();

		}
	}


}
