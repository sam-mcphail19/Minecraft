using System;
using System.Collections.Generic;
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

	void OnGUI() {
		Vector3 playerPos = GameObject.Find("Player").transform.position;
		GUI.Label(new Rect(10, 10, 150, 20), "Position: " + playerPos);
		GUI.Label(new Rect(10, 35, 150, 20), "C: " + continentalness.GetNoise(playerPos.x, playerPos.z));
		GUI.Label(new Rect(10, 60, 150, 20), "E: " + erosion.GetNoise(playerPos.x, playerPos.z));
		GUI.Label(new Rect(10, 85, 150, 20), "PV: " + peaksAndValleys.GetNoise(playerPos.x, playerPos.z));

		GUI.Label(new Rect(Screen.width - 150, 10, 150, 20), "FPS: " + 1.0f / Time.smoothDeltaTime);
	}

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
		List<NoiseMap> noiseMaps = JsonUtil.LoadNoiseMaps();

		continentalness = noiseMaps[0];
		erosion = noiseMaps[1];
		peaksAndValleys = noiseMaps[2];

		continentalness.seed = seed;
		erosion.seed = seed;
		peaksAndValleys.seed = seed;
	}

	void InitTerrainHeightSplines() {
		List<Path> noiseMapHeights = JsonUtil.LoadNoiseMapHeights();
		continentalnessShape = noiseMapHeights[0];
		JsonUtil.PrintToJson(continentalnessShape);
		erosionShape = new Path(Constants.ErosionSplinePoints);
		peaksAndValleysShape = new Path(Constants.PeaksAndValleysSplinePoints);
	}
}
