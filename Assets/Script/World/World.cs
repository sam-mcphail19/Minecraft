using System;
using System.Collections;
using System.Collections.Generic;
using EasyButtons;
using UnityEngine;
using MinecraftBlockRegistry;

public class World : MonoBehaviour {
	public float offset;
	public float scale;
	public ulong seed;

	private Dictionary<Vector3, Chunk> chunks = new Dictionary<Vector3, Chunk>();

	private NoiseMap continentalness;
	private NoiseMap erosion;
	private NoiseMap peaksAndValleys;

	private Path continentalnessShape;
	private Path erosionShape;
	private Path peaksAndValleysShape;

	// Start is called before the first frame update
	void Start() {
		BlockRegistry.Init();
		InitNoiseMaps();
		InitTerrainHeightSplines();

		GenerateWorld();

		GameObject.Find("Player").transform.position =
			new Vector3(0, chunks[Vector3.zero].GetHighestBlockHeightAtPoint(0, 0) + 3, 0);
	}

	public void GenerateWorld() {
		int xChunks = 8;
		int zChunks = 8;

		for (int x = -xChunks / 2; x < xChunks / 2; x++) {
			for (int z = -zChunks / 2; z < zChunks / 2; z++) {
				GameObject newChunk = new GameObject("Chunk" + ((z + zChunks / 2) + (x + xChunks / 2) * zChunks));
				Chunk chunk = newChunk.AddComponent<Chunk>();
				chunk.transform.SetParent(transform);
				Vector3 origin = new Vector3(x * Constants.ChunkSize, 0, z * Constants.ChunkSize);
				chunks.Add(origin, chunk);
				chunk.SetChunkOrigin(origin);
				chunk.Generate(this);
			}
		}
	}
	/*
	 * Multinoise lists the parameters used at the player's position in order to place a biome.
	   C is continentalness, E is erosion, T is temperature, H is humidity, and W is weirdness.
		- Continentalness goes up as you go more inland. In areas with low continentalness values, oceans may generate.
		- Erosion determines how flat or mountainous terrain is. Higher values result in flatter areas, lower values result in mountainous areas.
		- Temperature and humidity have no impact on the terrain itself, and determining only biome placement.
		- Weirdness indirectly drives the PV (peaks and valleys) noise and determines which biome variant gets placed.
	 */

	public int GetBlock(int x, int y, int z) {
		if (y <= 3)
			return (int) BlockType.Bedrock;

		//float noise = NoiseGenerator.Perlin2D(new Vector2(x, z), offset, scale, 1, seed);
		//int height = Mathf.FloorToInt(42 * noise);

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

	public int GetBlock(Vector3 pos) {
		return GetBlock((int) pos.x, (int) pos.y, (int) pos.z);
	}

	void InitNoiseMaps() {
		continentalness = gameObject.AddComponent<NoiseMap>();
		continentalness.offset = Constants.Continentalness.offset;
		continentalness.scale = Constants.Continentalness.scale;
		continentalness.octaveCount = Constants.Continentalness.octaveCount;
		continentalness.isSharp = Constants.Continentalness.isSharp;
		//continentalness.celThresholds = Constants.Continentalness.celThresholds;
		continentalness.celThresholds = new List<float>();
		continentalness.seed = Constants.Continentalness.seed;

		erosion = gameObject.AddComponent<NoiseMap>();
		erosion.offset = Constants.Erosion.offset;
		erosion.scale = Constants.Erosion.scale;
		erosion.octaveCount = Constants.Erosion.octaveCount;
		erosion.isSharp = Constants.Erosion.isSharp;
		//erosion.celThresholds = Constants.Erosion.celThresholds;
		erosion.celThresholds = new List<float>();
		erosion.seed = Constants.Erosion.seed;

		peaksAndValleys = gameObject.AddComponent<NoiseMap>();
		peaksAndValleys.offset = Constants.PeaksAndValleys.offset;
		peaksAndValleys.scale = Constants.PeaksAndValleys.scale;
		peaksAndValleys.octaveCount = Constants.PeaksAndValleys.octaveCount;
		peaksAndValleys.isSharp = Constants.PeaksAndValleys.isSharp;
		//peaksAndValleys.celThresholds = Constants.PeaksAndValleys.celThresholds;
		peaksAndValleys.celThresholds = new List<float>();
		peaksAndValleys.seed = Constants.PeaksAndValleys.seed;
	}

	void InitTerrainHeightSplines() {
		continentalnessShape = new Path(Constants.ContinentalnessSplinePoints);
		erosionShape = new Path(Constants.ErosionSplinePoints);
		peaksAndValleysShape = new Path(Constants.PeaksAndValleysSplinePoints);
	}

	[Button]
	public void RegenerateWorld() {
		if (chunks.Count > 0) {
			foreach (Chunk chunk in chunks.Values) {
				Destroy(chunk.gameObject);
			}

			chunks = new Dictionary<Vector3, Chunk>();
		}

		GenerateWorld();
	}
}
