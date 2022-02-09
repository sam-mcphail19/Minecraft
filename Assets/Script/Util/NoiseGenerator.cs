using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseGenerator {
	static int width = 0;
	static int height = 0;
	static int seed = 0;
	static float scale = 0;
	static int octaves = 0;
	static float persistance = 0;
	static float lacunarity = 0;
	static Vector2 offset = Vector2.zero;

	private static float maxHeight = float.MaxValue;
	private static float minHeight = float.MinValue;

	public static void Init(int width, int height, int seed, float scale, int octaves, float persistance,
		float lacunarity, Vector2 offset) {
		NoiseGenerator.width = width;
		NoiseGenerator.height = height;
		NoiseGenerator.seed = seed;
		NoiseGenerator.scale = scale;
		NoiseGenerator.octaves = octaves;
		NoiseGenerator.persistance = persistance;
		NoiseGenerator.lacunarity = lacunarity;
		NoiseGenerator.offset = offset;
	}

	// https://github.com/SebLague/Procedural-Landmass-Generation/blob/master/Proc%20Gen%20E03/Assets/Scripts/Noise.cs
	public static float[,] GenerateNoiseMap(Vector2 additionalOffset) {
		float[,] noiseMap = new float[width, height];

		System.Random prng = new System.Random(seed);
		Vector2[] octaveOffsets = new Vector2[octaves];
		for (int i = 0; i < octaves; i++) {
			float offsetX = prng.Next(-100000, 100000) + offset.x + additionalOffset.x;
			float offsetY = prng.Next(-100000, 100000) + offset.y + additionalOffset.y;
			octaveOffsets[i] = new Vector2(offsetX, offsetY);
		}

		if (scale <= 0) {
			scale = 0.0001f;
		}

		float maxNoiseHeight = float.MinValue;
		float minNoiseHeight = float.MaxValue;

		float halfWidth = width / 2f;
		float halfHeight = height / 2f;


		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				float amplitude = 1;
				float frequency = 1;
				float noiseHeight = 0;

				for (int i = 0; i < octaves; i++) {
					float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
					float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

					float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
					noiseHeight += perlinValue * amplitude;

					amplitude *= persistance;
					frequency *= lacunarity;
				}

				if (noiseHeight > maxNoiseHeight) {
					maxNoiseHeight = noiseHeight;
				}
				else if (noiseHeight < minNoiseHeight) {
					minNoiseHeight = noiseHeight;
				}

				noiseMap[x, y] = noiseHeight;
			}
		}

		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
			}
		}

		return noiseMap;
	}
}
