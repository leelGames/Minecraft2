using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName = "Minecraft2/Settings")]
public class Settings : ScriptableObject {
	[Header("Assets")]
	public bool saveToFile = false;
	public bool useFile = false;
	[Header("Assets")]
	public Material[] materials;
	public Texture2D colorMap;

	[Header("Performance")]
	public int renderDistance;
	public bool closeLods = false;
	public bool farLods = true;
	public int lodrenderDistance;

	[Header("Graphics")]
	public bool fog = true;
	public Color fogColor;
	public int fogstart;
	public int fogend;

	[Header("Utility")]
	public int seed;
	public bool voxelentities = true;
}
