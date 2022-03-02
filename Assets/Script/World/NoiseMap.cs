using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMap {
	public float offset;
	public float scale;
	public float lacunarity;
	public float persistence;
	public int octaveCount;
	public bool useBillow;

	public int seed {
		set => InitNoise(value);
	}

	private FastNoise noise;


	public NoiseMap(float offset, float scale, int octaveCount, float lacunarity, float persistence, bool useBillow,
		int seed) {
		this.offset = offset;
		this.scale = scale;
		this.octaveCount = octaveCount;
		this.lacunarity = lacunarity;
		this.persistence = persistence;
		this.useBillow = useBillow;
		this.seed = seed;
	}

	public float GetNoise(float x, float y) {
		return (noise.GetSimplexFractal(x + offset, y + offset) + 1) / 2;
	}

	public void InitNoise(int seed) {
		noise = new FastNoise(seed);
		noise.SetFractalOctaves(octaveCount);
		noise.SetFractalLacunarity(lacunarity);
		noise.SetFrequency(scale);
		noise.SetFractalGain(persistence);
		if (useBillow)
			noise.SetFractalType(FastNoise.FractalType.Billow);
	}

	public override string ToString() {
		return "NoiseMap(" +
		       "offset=" + offset +
		       ",scale=" + scale +
		       ",octaves=" + octaveCount +
		       ",lacunarity=" + lacunarity +
		       ",persistence=" + persistence +
		       ")";
	}
}
