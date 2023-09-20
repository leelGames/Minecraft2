using System;
using UnityEngine;
using TMPro;

public class DebugScreen : MonoBehaviour {

    public World world;
    public TextMeshProUGUI text;

    float fps;
    float tps;
    float timer;

    void Update() {
		Chunk c = world.GetChunkC(world.playerChunkCoord);
		try {
            string debugText = "";
            debugText += fps + "fps " + tps + "tps\n";
            debugText += "X: " + world.player.Position.x + " Y: " + world.player.Position.y + " Z: " + world.player.Position.z + "\n";
            debugText += "Chunk: " + world.playerChunkCoord.x + " " + world.playerChunkCoord.y + " " + chunkstates[c.state] + "\n" ;
            debugText += "Chunkqueue: G: " + world.threads.chunksToGenerate.Count + " U: " + world.threads.chunksToUpdate.Count + " D: " + world.threads.chunksToDraw.Count + " B: " + world.blocksToUpdate.Count + " L: " + world.threads.lodsToUpdate.Count + "D: " + world.threads.lodsToDraw.Count + "\n";
            debugText += "Facing: " + world.player.dir4 + " " + world.player.highlight.dir12 + " " + faceingNames[world.player.dir6] + "\n";
            debugText += "BBlock: " + GetBlockInfo(world.player.highlight.breakPos) + "\n";
			debugText += "PBlock: " + GetBlockInfo(world.player.highlight.blockPlacePos) + "\n";
			debugText += "Biome: " + BiomeData.biomes[world.GetGenData(world.player.Position).TerrainData.biomeid].name + "\n";
            debugText += "BreakSlap: " + world.player.highlight.bslab + " PlaceSlap: " + world.player.highlight.pslab + "\n";
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

    string GetBlockInfo(Vector3Int pos) {
		VoxelData block = world.GetVoxelData(pos);
        string info = block.block.id + " " + block.block.blockName + " ";
        if (block.block.dataSize.Length == 1) info += block.block.dataSize[0] + ": " + block.mainAtr + " ";
        else if (block.block.dataSize.Length > 1) {
            for (int i = 0; i < block.block.dataSize.Length; i++) {
                info += block.block.dataSize[i] + ": " + block.data[i] + " ";
            }
        }
        return info + block.flag;
	}


    static readonly string[] chunkstates = { "Initialising..", "Generating Terrain...", "Generating Structures.", "Updating..", "Drawing Mesh...", "" };
	static readonly string[] faceingNames = { "Down (-Y)", "Up (+Y)", "West (-X)", "East (+X)", "North (+Z)", "South (-Z)" };
}
