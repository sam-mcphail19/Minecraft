using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMap {
	public int width = 100;
	public int height = 100;
	public float offset = 0f;
	public float scale = 0.1f;
	[Range(0, 10)] public int octaveCount;
	public bool isSharp;
	[Range(0, 1)] public List<float> celThresholds;
	public ulong seed;
	public bool autoUpdate;
	public string mapName;

	Texture2D CreateNoiseMapTexture() {
		Color[] noiseMap = new Color[width * height];

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				noiseMap[x * width + y] = Color.Lerp(
					Color.black,
					Color.white,
					GetNoise(x, y)
				);;
			}
		}

		Texture2D noiseMapTexture = new Texture2D(width, height);
		noiseMapTexture.SetPixels(noiseMap);
		noiseMapTexture.Apply();

		return noiseMapTexture;
	}

	float GetNoiseCel(float value) {
		if (value < celThresholds[0])
			return 0f;

		int i = 1;
		while (i < celThresholds.Count) {
			if (value < celThresholds[i]) {
				return i / (float) celThresholds.Count;
			}
			i++;
		}

		return 1f;
	}

	public float GetNoise(float x, float y) {
		float noise = isSharp
			? NoiseGenerator.Perlin2DSharp(new Vector2(x, y), offset, scale, seed)
			: NoiseGenerator.Perlin2D(new Vector2(x, y), offset, scale, octaveCount, seed);

		return celThresholds.Count > 0 ? GetNoiseCel(noise) : noise;
	}
}
