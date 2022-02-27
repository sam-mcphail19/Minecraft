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

	private ChunkManager chunkManager;

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

		chunkManager = gameObject.AddComponent<ChunkManager>();
		chunkManager.SetViewDistance(viewDistance);
		
		JobHandle originChunkJob = chunkManager.GenerateChunk(Vector3.zero);
		originChunkJob.Complete();

		GameObject.Find("Player").transform.position = chunkManager.GetSpawnPosition();
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
}
