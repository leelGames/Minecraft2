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

	public BlockEventQueue blocksToUpdate;

	readonly Thread chunkUpdateThread;
	readonly Thread chunkGenerateThread;
	readonly Thread lodUpdateThread;
	readonly Thread blockUpdateThread;



	public int fixedTime;

	public WorldThreading() {
		world = World.currend;
		object chunkLock = new();
		object lodLock = new();
		object blockLock = new();

		chunksToGenerate = new(chunkLock);
		chunksToUpdate = new(chunkLock);
		chunksToDraw = new(chunkLock);
		lodsToUpdate = new(lodLock);
		lodsToDraw = new(lodLock);
		blocksToUpdate = new(blockLock);

		//Threads starten
		chunkUpdateThread = new Thread(new ThreadStart(ThreadedUpdate));
		chunkUpdateThread.Start();
		chunkGenerateThread = new Thread(new ThreadStart(ThreadedGenerate));
		chunkGenerateThread.Start();
		lodUpdateThread = new Thread(new ThreadStart(ThreadedLOD));
		lodUpdateThread.Start();
		blockUpdateThread = new Thread(new ThreadStart(ThreadedBlockUpdate));
		blockUpdateThread.Start();
	}

	public void Stop() {
		chunkGenerateThread.Abort();
		chunkUpdateThread.Abort();
		lodUpdateThread.Abort();
		blockUpdateThread.Abort();
	}

	void ThreadedBlockUpdate() {
		fixedTime = world.time;
		while(true) {
			while (world.time > fixedTime) {
				HandleblockEvents();
				fixedTime++;
			}
		}
	}

	void HandleblockEvents() {
		Vector3Int pos;
        Chunk c;
	
		while (blocksToUpdate.Count > 0 && blocksToUpdate.Peek().time == world.time) {
			pos = blocksToUpdate.Get();
			c = world.GetChunkP(pos);
			c.SetFlag(pos - c.Position, false);
			
			if (c.GetBlock(pos - c.Position).OnBlockUpdate(c, pos - c.Position)) {
				world.UpdateBlockFast(pos);
			}
			
		}
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
	//Buffer sorgt dafür, dass der selbe Chunk nicht mehrmals im gleichen Tick geupdatet werden kann (1 Tick delay)
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
		lock (threadLock) {
			foreach (UpdateEvent c in buffer) {
				if (e.coord == c.coord) return;
			}
			buffer.Push(e);
		}
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
//Binary Heap Priority Queue von GPT
public class BlockEventQueue {
    public List<BlockEvent> items;

	object threadLock;
	
	public BlockEventQueue(object threadLock) {
		items = new();
		this.threadLock = threadLock;
	}

    public void Add(BlockEvent blockEvent) {
        lock (threadLock) {
			items.Add(blockEvent);
		}
		int childIndex = items.Count - 1;

        while (childIndex > 0) {
            int parentIndex = (childIndex - 1) / 2;

            if (items[childIndex].time >= items[parentIndex].time) break;

            // Swap the child and parent
            (items[childIndex], items[parentIndex]) = (items[parentIndex], items[childIndex]);
            childIndex = parentIndex;
        }
    }

	public BlockEvent Peek() {
		return items[0];
	}

    public Vector3Int Get() {

        int lastIndex = items.Count - 1;
        (items[0], items[lastIndex]) = (items[lastIndex], items[0]);
        var item = items[lastIndex].pos;
		
        lock (threadLock) {
			items.RemoveAt(lastIndex);
		}

        int parentIndex = 0;
        while (true) {
            int leftChildIndex = 2 * parentIndex + 1;
            int rightChildIndex = 2 * parentIndex + 2;
            int smallestChildIndex = parentIndex;

            if (leftChildIndex < items.Count && items[leftChildIndex].time < items[smallestChildIndex].time) smallestChildIndex = leftChildIndex;

            if (rightChildIndex < items.Count && items[rightChildIndex].time < items[smallestChildIndex].time) smallestChildIndex = rightChildIndex;

            if (smallestChildIndex == parentIndex) break;

            // Swap the parent and smallest child
            (items[parentIndex], items[smallestChildIndex]) = (items[smallestChildIndex], items[parentIndex]);
            parentIndex = smallestChildIndex;
        }

        return item;
    }

    public int Count => items.Count;

    //public bool IsEmpty => Count == 0;
}
