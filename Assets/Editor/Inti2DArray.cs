using UnityEngine;
using UnityEditor;
using System.IO;


public class InitTex2DArray : EditorWindow {

	Texture2DArray texArray;
	Texture2D colorMap;
	string texturePath = "Texture/";
	string arrayPath = "Assets/Materials/";
	int textureCount = 16;
	int colorMapSize = 64;

	static readonly string[] names = {
		"AO", "Color", "Normal" , "Specular"
	};

    [MenuItem("Minecraft 2/Init Tex2D Array")]

    public static void ShowWindow() {
        
        EditorWindow.GetWindow(typeof(InitTex2DArray));
    }

    private void OnGUI() {
        texturePath = EditorGUILayout.TextField("Textures Path", texturePath);
		arrayPath = EditorGUILayout.TextField("Output Path", arrayPath);
		textureCount = EditorGUILayout.IntField("Texture Count", textureCount);
	
		if (GUILayout.Button("GetTextureArray")) {
			GetTextureArray();
		}
		if (GUILayout.Button("GetColorMap")) {
			GetColorMap();
			colorMap.Apply();
			//File.WriteAllBytes(arrayPath + "ColorMap.png", colorMap.EncodeToPNG());
			AssetDatabase.CreateAsset(colorMap, arrayPath + "ColorMap.asset");
		}

	}

	void GetTextureArray() {
		Texture2D[] textures = Resources.LoadAll<Texture2D>(texturePath);
		int size = textures[0].width;
		Debug.Log(textures.Length % textureCount);
		for (int j = 0; j < textures.Length / textureCount; j++) {

			texArray = new Texture2DArray(size, size, textureCount, TextureFormat.ARGB32, true);
			for (int i = 0; i < textureCount; i++) {
				texArray.SetPixels32(textures[j * textureCount + i].GetPixels32(0), i, 0);
			}
			texArray.Apply();
			AssetDatabase.CreateAsset(texArray, arrayPath + names[j] + ".asset");
		}
	}

	void GetColorMap() {
		Texture2D[] textures = Resources.LoadAll<Texture2D>(texturePath + "BaseColor/");
		//int size = textures[0].width;
		colorMap = new Texture2D(colorMapSize, colorMapSize, TextureFormat.ARGB32, true);
		colorMap.filterMode = FilterMode.Point;
		colorMap.wrapMode = TextureWrapMode.Clamp;
		int index = 0;
		for (int x = 0; x < colorMapSize; x++) {
			for (int y = 0; x < colorMapSize; y++) {
				colorMap.SetPixel(x, y, Block.GetTextureColor(textures[index].GetPixels()), 0);
				index++;
				if (index >= textures.Length) return;
			}
		}
	}
}
