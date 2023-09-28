using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class WorldThreading {
	readonly World world;

	public ChunkQueue chunksToGenerate;
	public ChunkQueue chunksToUpdate;
	public ChunkQueue chunksToDraw;
	public ChunkQueue lodsToUpdate;
	public ChunkQueue lodsToDraw;

	readonly Thread chunkUpdateThread;
	readonly Thread chunkGenerateThread;
	readonly Thread lodUpdateThread;

	public object chunkLock;
	public object lodLock;

	public WorldThreading() {
		world = World.currend;
		chunkLock = new();
		lodLock = new();

		chunksToGenerate = new(chunkLock);
		chunksToUpdate = new(chunkLock);
		chunksToDraw = new(chunkLock);
		lodsToUpdate = new(lodLock);
		lodsToDraw = new(lodLock);

		//Threads starten
		chunkUpdateThread = new Thread(new ThreadStart(ThreadedUpdate));
		chunkUpdateThread.Start();
		chunkGenerateThread = new Thread(new ThreadStart(ThreadedGenerate));
		chunkGenerateThread.Start();
		lodUpdateThread = new Thread(new ThreadStart(ThreadedLOD));
		lodUpdateThread.Start();
	}

	public void Stop() {
		chunkGenerateThread.Abort();
		chunkUpdateThread.Abort();
		lodUpdateThread.Abort();
	}

	public void Draw() {
		//Chunkmesh Erstellen
		UpdateEvent e;
		Chunk chunk;
		LODHeightMap lod;

		while (chunksToDraw.Count > 0) {
			e = chunksToDraw.Remove();
			chunk = world.GetChunkC(e.coord);
			if (chunk.state == 4) {
				chunk.render.DrawMesh(e);
				chunk.state = 5;
			}
		}
		//if (chunksToUpdate.Count == 0)
		chunksToUpdate.ClearBuffer();

		while (lodsToDraw.Count > 0) {
			lod = world.GetLodC(lodsToDraw.Remove().coord);
			lod.DrawMesh();
			lod.gameObject.SetActive(true);
		}
		if (lodsToUpdate.Count == 0) lodsToUpdate.ClearBuffer();
	}

	void ThreadedGenerate() {
		UpdateEvent e;
		Chunk chunk;
		while (true) {
			if (chunksToGenerate.Count > 0) {
				e = chunksToGenerate.Remove();
				chunk = world.GetChunkC(e.coord);

				//Chunk generieren
				if (chunk.state < 2) {
					chunk.Generate();
					chunk.GenerateSurounding();
				}
				//Strukturen generieren
				if (chunk.state < 3) {
					chunk.GenerateStructures();
					chunk.StrucktureSurrounding();
					chunk.CalcHeightData();
				}
				if (chunk.state < 4) {
					chunksToUpdate.Add(e);
				}
			}
		}
	}

	void ThreadedUpdate() {
		UpdateEvent e;
		Chunk chunk;
		while (true) {
			if (chunksToUpdate.Count > 0) {
				e = chunksToUpdate.Remove();
				chunk = world.GetChunkC(e.coord);
			
				if (chunk.state != 4) {
					chunk.UpdateChunk(e);
					chunksToDraw.Add(e);
				}
			}
		}
	}

	void ThreadedLOD() {
		UpdateEvent e;
		while (true) {
			if (lodsToUpdate.Count > 0) {
				e = lodsToUpdate.Remove();
				world.GetLodC(e.coord).UpdateMesh(e.lod);
				lodsToDraw.Add(e);
			}
		}
	}
}

public class ChunkQueue {
	readonly LinkedList<UpdateEvent> queue;
	//Buffer sorgt daf�r, dass der selbe Chunk nicht mehrmals im gleichen Tick geupdatet werden kann (1 Tick delay)
	readonly Stack<UpdateEvent> buffer;
	readonly object threadLock;

	public ChunkQueue(object threadLock) {
		queue = new();
		buffer = new();
		this.threadLock = threadLock;
	}

	public int Count { get { return queue.Count; } }

	public void Add(UpdateEvent e) {
		lock (threadLock) queue.AddLast(e);
	}

	public UpdateEvent Remove() {
		lock (threadLock) {
			UpdateEvent f = queue.First.Value;
			queue.RemoveFirst();
			return f;
		}
	}

	public UpdateEvent Get() {
		return queue.First.Value;
	}

	public void AddBuffer(UpdateEvent e) {
		foreach (UpdateEvent c in buffer) {
			if (e.coord == c.coord) return;
		}
		buffer.Push(e);
	}
	
	public void ClearBuffer() {
		lock (threadLock) {

			while (buffer.Count > 0) {
				queue.AddFirst(buffer.Pop());
			}
		}
	}
}

public struct UpdateEvent {
	public Vector2Int coord;
	public int lod;
	public bool heightmap;
	public bool dynamic;
	public bool collider;

	public UpdateEvent(Vector2Int coord, int lod, bool dynamic, bool collider) {
		this.coord = coord;
		this.lod = lod;
		this.dynamic = dynamic;
		this.collider = collider;
		this.heightmap = false;
	}

	public UpdateEvent(Vector2Int coord, int lod) {
		this.coord = coord;
		this.lod = lod;
		this.heightmap = true;
		this.collider = false;
		this.dynamic = false;
	}
}

