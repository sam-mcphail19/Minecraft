using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MinecraftBlockRegistry;
using Unity.Jobs;

public class World : MonoBehaviour {
	public ulong seed;

	public int viewDistance;

	private Transform playerTransform;

	private static Dictionary<Vector3, Chunk> loadedChunks = new Dictionary<Vector3, Chunk>();
	private static Dictionary<Vector3, Chunk> unloadedChunks = new Dictionary<Vector3, Chunk>();
	private static Queue<Chunk> chunksToBeRendered = new Queue<Chunk>();
	private static Queue<Vector3> chunksToBeRetried = new Queue<Vector3>();

	private int updateCount = 0;

	private static NoiseMap continentalness;
	private static NoiseMap erosion;
	private static NoiseMap peaksAndValleys;

	private static Path continentalnessShape;
	private static Path erosionShape;
	private static Path peaksAndValleysShape;

	// Start is called before the first frame update
	void Start() {
		BlockRegistry.Init();
		InitNoiseMaps();
		InitTerrainHeightSplines();

		JobHandle originChunkJob = GenerateChunk(Vector3.zero);
		originChunkJob.Complete();

		playerTransform = GameObject.Find("Player").transform;
		playerTransform.position =
			new Vector3(0, unloadedChunks[Vector3.zero].GetHighestBlockHeightAtPoint(0, 0) + 3, 0);
	}

	void Update() {
		while (chunksToBeRendered.Count > 0) {
			Chunk chunk = chunksToBeRendered.Dequeue();
			try {
				chunk.ApplyMesh();
			} catch (ArgumentException e) {
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
				if(chunk.vertexCount > 0)
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

	/*
	 * Multinoise lists the parameters used at the player's position in order to place a biome.
	   C is continentalness, E is erosion, T is temperature, H is humidity, and W is weirdness.
		- Continentalness goes up as you go more inland. In areas with low continentalness values, oceans may generate.
		- Erosion determines how flat or mountainous terrain is. Higher values result in flatter areas, lower values result in mountainous areas.
		- Temperature and humidity have no impact on the terrain itself, and determining only biome placement.
		- Weirdness indirectly drives the PV (peaks and valleys) noise and determines which biome variant gets placed.
	 */
	public static int GetBlock(int x, int y, int z) {
		if (y <= 3)
			return (int) BlockType.Bedrock;

		float c = continentalnessShape.Evaluate(continentalness.GetNoise(x, z));
		float e = erosionShape.Evaluate(erosion.GetNoise(x, z));
		float p = peaksAndValleysShape.Evaluate(peaksAndValleys.GetNoise(x, z));

		int height = Mathf.FloorToInt((c + e + p) / 3);

		if (y > height)
			return (int) BlockType.Air;

		if (y > height - 3)
			return (int) BlockType.Dirt;

		return (int) BlockType.Stone;
	}

	public static int GetBlock(Vector3 pos) {
		return GetBlock((int) pos.x, (int) pos.y, (int) pos.z);
	}

	void InitNoiseMaps() {
		continentalness = gameObject.AddComponent<NoiseMap>();
		continentalness.offset = Constants.Continentalness.offset;
		continentalness.scale = Constants.Continentalness.scale;
		continentalness.octaveCount = Constants.Continentalness.octaveCount;
		continentalness.isSharp = Constants.Continentalness.isSharp;
		continentalness.celThresholds = new List<float>();
		continentalness.seed = seed;

		erosion = gameObject.AddComponent<NoiseMap>();
		erosion.offset = Constants.Erosion.offset;
		erosion.scale = Constants.Erosion.scale;
		erosion.octaveCount = Constants.Erosion.octaveCount;
		erosion.isSharp = Constants.Erosion.isSharp;
		erosion.celThresholds = new List<float>();
		erosion.seed = seed;

		peaksAndValleys = gameObject.AddComponent<NoiseMap>();
		peaksAndValleys.offset = Constants.PeaksAndValleys.offset;
		peaksAndValleys.scale = Constants.PeaksAndValleys.scale;
		peaksAndValleys.octaveCount = Constants.PeaksAndValleys.octaveCount;
		peaksAndValleys.isSharp = Constants.PeaksAndValleys.isSharp;
		peaksAndValleys.celThresholds = new List<float>();
		peaksAndValleys.seed = seed;
	}

	void InitTerrainHeightSplines() {
		continentalnessShape = new Path(Constants.ContinentalnessSplinePoints);
		erosionShape = new Path(Constants.ErosionSplinePoints);
		peaksAndValleysShape = new Path(Constants.PeaksAndValleysSplinePoints);
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
