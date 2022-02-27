using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Jobs;
using UnityEngine;

public class ChunkManager : MonoBehaviour {
	private Transform playerTransform;

	private static Dictionary<Vector3, Chunk> loadedChunks = new Dictionary<Vector3, Chunk>();
	private static Dictionary<Vector3, Chunk> unloadedChunks = new Dictionary<Vector3, Chunk>();
	private static Queue<Chunk> chunksToBeRendered = new Queue<Chunk>();
	private static Queue<Vector3> chunksToBeRetried = new Queue<Vector3>();

	private int viewDistance;

	private int updateCount = 0;

	void Start() {
		playerTransform = GameObject.Find("Player").transform;
	}

	void Update() {
		while (chunksToBeRendered.Count > 0) {
			Chunk chunk = chunksToBeRendered.Dequeue();
			try {
				chunk.ApplyMesh();
			} catch (ArgumentException) {
				chunksToBeRetried.Enqueue(chunk.GetChunkOrigin());
			}
		}

		while (chunksToBeRetried.Count > 0) {
			Vector3 chunkOrigin = chunksToBeRetried.Dequeue();
			Debug.Log("Retrying unloading/generation of chunk: " + chunkOrigin);
			if (loadedChunks.ContainsKey(chunkOrigin)) {
				UnloadChunk(chunkOrigin);
			} else {
				GenerateChunk(chunkOrigin);
			}
		}

		if (!playerTransform)
			return;

		List<Vector3> visibleChunkOrigins = GetVisibleChunkOrigins();

		foreach (Vector3 chunkOrigin in loadedChunks.Keys.ToList()) {
			if (!visibleChunkOrigins.Contains(chunkOrigin))
				UnloadChunk(chunkOrigin);
			else if (loadedChunks[chunkOrigin].vertexCount == 0) {
				loadedChunks[chunkOrigin].Generate();
				chunksToBeRendered.Enqueue(loadedChunks[chunkOrigin]);
			}
		}

		foreach (Vector3 chunkOrigin in visibleChunkOrigins) {
			if (!loadedChunks.ContainsKey(chunkOrigin)) {
				if (unloadedChunks.ContainsKey(chunkOrigin))
					LoadChunk(chunkOrigin);
				else
					GenerateChunk(chunkOrigin);
			}
		}

		updateCount++;
		// Every 50th update, go through the unloaded chunks and make sure that the meshes are empty
		if (updateCount == 50) {
			foreach (Chunk chunk in unloadedChunks.Values.ToList()) {
				if (chunk.vertexCount > 0)
					chunk.DestroyMesh();
			}

			updateCount = 0;
		}
	}

	public JobHandle GenerateChunk(Vector3 origin) {
		Debug.Log("Generating chunk at: " + origin);

		Chunk chunk = new GameObject("Chunk" + origin).AddComponent<Chunk>();
		chunk.SetChunkOrigin(origin);
		chunk.InitMesh();

		unloadedChunks.Add(origin, chunk);

		return new GenerateChunkJob(origin.x, origin.y, origin.z).Schedule();
	}

	public static void LoadChunk(Vector3 chunkOrigin) {
		unloadedChunks[chunkOrigin].CreateMesh();
		chunksToBeRendered.Enqueue(unloadedChunks[chunkOrigin]);

		loadedChunks.Add(chunkOrigin, unloadedChunks[chunkOrigin]);
		unloadedChunks.Remove(chunkOrigin);
	}

	public static void UnloadChunk(Vector3 chunkOrigin) {
		loadedChunks[chunkOrigin].DestroyMesh();

		unloadedChunks.Add(chunkOrigin, loadedChunks[chunkOrigin]);
		loadedChunks.Remove(chunkOrigin);
	}

	List<Vector3> GetVisibleChunkOrigins() {
		List<Vector3> origins = new List<Vector3>();

		Vector3 playerPos = playerTransform.position;
		int currentChunkOriginX = MathUtil.RoundToMultipleOf(playerPos.x, Constants.ChunkSize);
		int currentChunkOriginZ = MathUtil.RoundToMultipleOf(playerPos.z, Constants.ChunkSize);
		Vector3 currentChunkOrigin = new Vector3(currentChunkOriginX, 0, currentChunkOriginZ);

		for (int x = -viewDistance; x < viewDistance; x++) {
			for (int z = -viewDistance; z < viewDistance; z++) {
				origins.Add(currentChunkOrigin + new Vector3(x * Constants.ChunkSize, 0, z * Constants.ChunkSize));
			}
		}

		return origins;
	}

	public Vector3 GetSpawnPosition() {
		return new Vector3(0, unloadedChunks[Vector3.zero].GetHighestBlockHeightAtPoint(0, 0) + 3, 0);
	}

	public void SetViewDistance(int viewDistance) {
		this.viewDistance = viewDistance;
	}

	struct GenerateChunkJob : IJob {
		private readonly Vector3 origin;

		public GenerateChunkJob(float x, float y, float z) {
			origin = new Vector3(x, y, z);
		}

		public void Execute() {
			if (!unloadedChunks.ContainsKey(origin)) {
				chunksToBeRetried.Enqueue(origin);
				return;
			}

			Chunk chunk = unloadedChunks[origin];

			chunk.Populate();
			chunk.CreateMesh();

			chunksToBeRendered.Enqueue(chunk);
		}
	}
}
