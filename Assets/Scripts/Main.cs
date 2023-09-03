using UnityEngine;
using System.IO;
using System;

public static class Main {
    public static Settings s;
	public static Material[] materials;
	public static Texture2D colorMap;
	public static Mesh2[][] meshTable;

	public static void Init(Settings s) {
        string settingsfile;

        if (s.saveToFile) {
            settingsfile = JsonUtility.ToJson(s);
            File.WriteAllText(Application.dataPath + "/settings.json", settingsfile);
        }
        if (s.useFile) {
            settingsfile = File.ReadAllText(Application.dataPath + "/settings.json");
            JsonUtility.FromJsonOverwrite(settingsfile, s);
        }
        materials = s.materials;
        colorMap = s.colorMap;
		Main.s = s;
		RenderSettings.fog = Main.s.fog;
        RenderSettings.fogMode = FogMode.Linear;
		RenderSettings.fogColor = Main.s.fogColor;
		RenderSettings.fogStartDistance = (Main.s.lodrenderDistance / 2 + Main.s.fogstart) * VD.LODWidth;
		RenderSettings.fogEndDistance = (Main.s.lodrenderDistance / 2 + Main.s.fogend) * VD.LODWidth;
        QualitySettings.shadowDistance = Main.s.renderDistance * VD.ChunkWidth;
        /*QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = -1;*/
        InitMeshes();
	}


	//Packt Meshs in f�r Threads lesbare Listen
	static void InitMeshes() {
        Mesh[] load = Resources.LoadAll<Mesh>("Mesh");
        meshTable = new Mesh2[BD.meshcount.Length][];
        int index = 0;

        for (int i = 0; i < BD.meshcount.Length; i++) {
            meshTable[i] = new Mesh2[BD.meshcount[i]];
            for (int j = 0; j < BD.meshcount[i]; j++) {
                meshTable[i][j] = new Mesh2(load[index], CalculateBounds(load[index]));
                index++;
            }
        }
    }
    
    static Bounds CalculateBounds(Mesh mesh) {
        Vector3 min = new Vector3(10, 10, 10);
        Vector3 max = new Vector3(-19, -10, -10);

        for (int i = 0; i < mesh.vertices.Length; i++) {
            Vector3 vert = Round(mesh.vertices[i]);

			if (vert.x < min.x) min.x = vert.x;
			else if (vert.x > max.x) max.x = vert.x;
			if (vert.y < min.y) min.y = vert.y;
			else if (vert.y > max.y) max.y = vert.y;
			if (vert.z < min.z) min.z = vert.z;
			else if (vert.z > max.z) max.z = vert.z;
		}
        Bounds b = new Bounds();
        b.SetMinMax(min, max);
        return b;
    }
    static Vector3 Round(Vector3 v) {
        return ((Vector3) Vector3Int.RoundToInt(v * 100f)) / 100f;
    }
    public static string[] Concat(string[] x, string[] y) {
		string[] z = new string[x.Length + y.Length];
		x.CopyTo(z, 0);
		y.CopyTo(z, x.Length);
		return z;
	} 
}

public struct Mesh2 {
    public Vector3[] vertices;
    public Vector2[] uv;
    public int[] triangles;
    public Bounds bounds;

    public Mesh2(Mesh mesh, Bounds b) {
        vertices = mesh.vertices;
        uv = mesh.uv;
        triangles = mesh.triangles;
        //ungenau
        //this.bounds = mesh.bounds;
        bounds = b;
    }
}
