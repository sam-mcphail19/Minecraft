using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseGenerator {
	public static float Perlin2D(Vector2 position, float offset, float scale, int octaves, ulong seed) {
		ulong xSeed = seed * 5471L;
		ulong ySeed = seed * 3889L;

		float sum = 0f;
		float frequency = 1f;
		float amplitude = 1f;

		for (int i = 0; i < octaves; i++) {
			float noise = Mathf.PerlinNoise(
				(position.x * (i+1) + xSeed + 0.1f * frequency) / Constants.ChunkSize * scale + offset,
				(position.y * (i+1) + ySeed + 0.1f * frequency) / Constants.ChunkSize * scale + offset
			);
			
			sum += (noise * 2 - 1) * amplitude;
			
			frequency *= 0.5f;
			amplitude *= 0.5f;
		}

		return (sum + 1) / 2;
	}
	
	public static float Perlin2DSharp(Vector2 position, float offset, float scale, int octaves, ulong seed) {
		ulong xSeed = seed * 5471L;
		ulong ySeed = seed * 3889L;

		float sum = 0f;
		float frequency = 1f;
		float amplitude = 1f;

		for (int i = 0; i < octaves; i++) {
			float noise = Mathf.PerlinNoise(
				(position.x * (i+1) + xSeed + 0.1f * frequency) / Constants.ChunkSize * scale + offset,
				(position.y * (i+1) + ySeed + 0.1f * frequency) / Constants.ChunkSize * scale + offset
			);
			
			sum += Math.Abs((noise * 2 - 1) * amplitude);
			sum = sum * 2 - 1;
			
			frequency *= 0.5f;
			amplitude *= 0.5f;
		}

		return (sum + 1) / 2;
	}
}
