using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseGenerator {
	public static float Perlin2D(Vector2 position, float offset, float scale, int seed) {
		return Mathf.PerlinNoise(
			(position.x + seed + 0.1f) / Constants.ChunkSize * scale + offset,
			(position.y + seed + 0.1f) / Constants.ChunkSize * scale + offset
		);
	}
}
