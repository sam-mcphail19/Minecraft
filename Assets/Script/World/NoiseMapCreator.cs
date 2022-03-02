using System;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMapCreator : MonoBehaviour {
	public int width = 100;
	public int height = 100;
	public float offset = 0f;
	public float scale = 0.1f;
	public float lacunarity;
	public float persistence;
	public bool useBillow;
	[Range(0, 10)] public int octaveCount;
	public int seed;
	public bool autoUpdate;

	private NoiseMap noiseMap;

	Texture2D CreateNoiseMapTexture() {
		noiseMap = new NoiseMap(offset, scale, octaveCount, lacunarity, persistence, useBillow, seed);
		Color[] colors = new Color[width * height];

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				colors[x * width + y] = Color.Lerp(
					Color.black,
					Color.white,
					noiseMap.GetNoise(x, y)
				);
			}
		}

		Texture2D noiseMapTexture = new Texture2D(width, height);
		noiseMapTexture.SetPixels(colors);
		noiseMapTexture.Apply();

		return noiseMapTexture;
	}

	public void SetNoiseTexture() {
		MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
		meshRenderer.sharedMaterial.mainTexture = CreateNoiseMapTexture();
	}

	public void OnValidate() {
		width = Mathf.Max(width, 1);
		height = Mathf.Max(height, 1);
		lacunarity = Mathf.Max(lacunarity, 1);
		octaveCount = Mathf.Max(octaveCount, 0);
	}
}
