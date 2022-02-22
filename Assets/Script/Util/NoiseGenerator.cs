using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseGenerator {
	private const long XSeedMultiplier = 5471L;
	private const long YSeedMultiplier = 3889L;

	public static float Perlin2D(Vector2 position, float offset, float scale, int octaves, ulong seed) {
		float sum = 0f;
		float frequency = 1f;
		float amplitude = 1f;

		for (int i = 0; i < octaves; i++) {
			float noise = Mathf.PerlinNoise(
				(position.x * (i + 1) + seed * XSeedMultiplier + 0.1f * frequency) / Constants.ChunkSize * scale +
				offset,
				(position.y * (i + 1) + seed * YSeedMultiplier + 0.1f * frequency) / Constants.ChunkSize * scale +
				offset
			);

			sum += (noise * 2 - 1) * amplitude;

			frequency *= 0.5f;
			amplitude *= 0.5f;
		}

		return (sum + 1) / 2;
	}

	public static float Perlin2DSharp(Vector2 position, float offset, float scale, ulong seed) {
		float noise = Mathf.PerlinNoise(
			(position.x + seed * XSeedMultiplier + 0.1f) / Constants.ChunkSize * scale + offset,
			(position.y + seed * YSeedMultiplier + 0.1f) / Constants.ChunkSize * scale + offset
		);

		return Math.Abs(noise * 2 - 1);
	}
}
