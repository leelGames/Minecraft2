using System;
using UnityEngine;
using TMPro;
using System.Text;

public class DebugScreen : MonoBehaviour {
    public TextMeshProUGUI text;

    float fps;
    float tps;
    float timer;

    void Update() {
        World w = World.currend;
        Player p = Player.client;
		Chunk c = w.GetChunkC(w.playerChunkCoord);
		try {
            StringBuilder debugText = new();
            debugText.Append(fps + "fps " + tps + "tps" + "Ticklag: " + (w.time - w.threads.fixedTime) + "\n");
            debugText.Append("X: " + p.Position.x + " Y: " + p.Position.y + " Z: " + p.Position.z + "\n");
            debugText.Append("Chunk: " + w.playerChunkCoord.x + " " + w.playerChunkCoord.y + " " + chunkstates[c.state] + "\n");
            debugText.Append("Chunkqueue: G: " + w.threads.chunksToGenerate.Count + " U: " + w.threads.chunksToUpdate.Count + " D: " + w.threads.chunksToDraw.Count + " B: " + w.threads.blocksToUpdate.Count + " L: " + w.threads.lodsToUpdate.Count + "D: " + w.threads.lodsToDraw.Count + "\n");
            debugText.Append("Facing: " + p.dir4 + " " + p.highlight.dir12 + " " + faceingNames[p.dir6] + "\n");
            debugText.Append("BBlock: " + GetBlockInfo(w.GetVoxelData(p.highlight.breakPos)) + "\n");
			debugText.Append("PBlock: " + GetBlockInfo(w.GetVoxelData(p.highlight.blockPlacePos)) + "\n");
            debugText.Append("SPBlock: " + GetBlockInfo(w.GetVoxelData(p.highlight.slabPlacePos)) + "\n");
			debugText.Append("Biome: " + BiomeData.biomes[w.GetGenData(p.Position).TerrainData.biomeid].name + "\n");
            debugText.Append("BreakSlap: " + p.highlight.bslab + " PlaceSlap: " + p.highlight.pslab + "\n");
			debugText.Append(getFlag(p.highlight.blockPlacePos));
            
            
            text.text = debugText.ToString();
        }
        //Anfällig für Exceptions
        catch (Exception e) { text.text = "Could not read all Debug Data:\n" + e.Message; }
        
        //fps berechnen
		if (timer > 0.5f) {
            fps = Mathf.FloorToInt(1f / Time.unscaledDeltaTime);
            tps = Mathf.FloorToInt(1f / Time.fixedUnscaledDeltaTime);
            timer = 0;
        }
        else timer += Time.deltaTime;
	}

    string GetBlockInfo(VoxelData block) {
        string info = "";
        if (block.block.blockName != "Block Combination") {
             info = block.block.blockName + " | ";
            for (int i = 0; i < block.block.dataSize.Length; i++) {
                info += block.block.dataSize[i] + ": " + block.data[i] + " ";
            }
        } else {
            int offset = 0;
            for (int j = 0; j < 2; j++) {
                Block a = BD.blocks[block.data[offset]];
                info += a.blockName + " | ";
                for (int i = 0; i < a.dataSize.Length; i++) {
                    info += a.dataSize[i] + ": " + block.data[offset + i + 1] + " ";
                }
                offset += a.dataSize.Length + 1;
                info += ", ";
            }
        }
        return info;
	}

    bool getFlag(Vector3Int pos) {
        Chunk c = World.currend.GetChunkP(pos);
        return c.GetFlag(pos - c.Position);
    }


    static readonly string[] chunkstates = { "Initialising..", "Generating Terrain...", "Generating Structures.", "Updating..", "Drawing Mesh...", "" };
	static readonly string[] faceingNames = { "Down (-Y)", "Up (+Y)", "West (-X)", "East (+X)", "North (+Z)", "South (-Z)" };
}
