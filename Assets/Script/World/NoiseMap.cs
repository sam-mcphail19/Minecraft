using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMap : MonoBehaviour {
	public int width = 100;
	public int height = 100;
	public float offset = 0f;
	public float scale = 0.1f;
	[Range(0, 10)] public int octaveCount;
	public bool isSharp;
	[Range(0, 1)] public List<float> celThresholds;
	public ulong seed;
	public bool autoUpdate;

	// Start is called before the first frame update
	void Start() {
		SetNoiseTexture();
	}

	public void SetNoiseTexture() {
		MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
		meshRenderer.material.mainTexture = CreateNoiseMapTexture();
	}

	Texture2D CreateNoiseMapTexture() {
		Color[] noiseMap = new Color[width * height];

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				Color color = Color.Lerp(
					Color.black,
					Color.white,
					GetNoise(x, y)
				);

				noiseMap[x * width + y] = celThresholds.Count > 0 ? GetColorCel(color) : color;
			}
		}

		Texture2D noiseMapTexture = new Texture2D(width, height);
		noiseMapTexture.SetPixels(noiseMap);
		noiseMapTexture.Apply();

		return noiseMapTexture;
	}

	Color GetColorCel(Color color) {
		if (color.r < celThresholds[0])
			return Color.black;

		int i = 1;
		while (i < celThresholds.Count) {
			if (color.r < celThresholds[i]) {
				float colorVal = i / (float) celThresholds.Count;
				return new Color(colorVal, colorVal, colorVal);
			}

			i++;
		}

		return Color.white;
	}

	float GetNoise(int x, int y) {
		return isSharp
			? NoiseGenerator.Perlin2DSharp(new Vector2(x, y), offset, scale, seed)
			: NoiseGenerator.Perlin2D(new Vector2(x, y), offset, scale, octaveCount, seed);
	}
}
