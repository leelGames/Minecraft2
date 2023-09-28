using System;
using UnityEngine;
using TMPro;

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
            string debugText = "";
            debugText += fps + "fps " + tps + "tps\n";
            debugText += "X: " + p.Position.x + " Y: " + p.Position.y + " Z: " + p.Position.z + "\n";
            debugText += "Chunk: " + w.playerChunkCoord.x + " " + w.playerChunkCoord.y + " " + chunkstates[c.state] + "\n" ;
            debugText += "Chunkqueue: G: " + w.threads.chunksToGenerate.Count + " U: " + w.threads.chunksToUpdate.Count + " D: " + w.threads.chunksToDraw.Count + " B: " + w.blocksToUpdate.Count + " L: " + w.threads.lodsToUpdate.Count + "D: " + w.threads.lodsToDraw.Count + "\n";
            debugText += "Facing: " + p.dir4 + " " + p.highlight.dir12 + " " + faceingNames[p.dir6] + "\n";
            debugText += "BBlock: " + GetBlockInfo(w.GetVoxelData(p.highlight.breakPos)) + "\n";
			debugText += "PBlock: " + GetBlockInfo(w.GetVoxelData(p.highlight.blockPlacePos)) + "\n";
            debugText += "SPBlock: " + GetBlockInfo(w.GetVoxelData(p.highlight.slabPlacePos)) + "\n";
			debugText += "Biome: " + BiomeData.biomes[w.GetGenData(p.Position).TerrainData.biomeid].name + "\n";
            debugText += "BreakSlap: " + p.highlight.bslab + " PlaceSlap: " + p.highlight.pslab + "\n";
            //debugText += world.GetBounds(world.player.highlight.breakPos).min.ToString("F10") + " " + world.GetBounds(world.player.highlight.breakPos).max.ToString("F10");
            //debugText += world.IsGrounded(world.player.highlight.breakPos);
			text.text = debugText;
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


    static readonly string[] chunkstates = { "Initialising..", "Generating Terrain...", "Generating Structures.", "Updating..", "Drawing Mesh...", "" };
	static readonly string[] faceingNames = { "Down (-Y)", "Up (+Y)", "West (-X)", "East (+X)", "North (+Z)", "South (-Z)" };
}
