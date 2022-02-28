using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants {
	public const int ChunkSize = 16;
	public const int WorldHeight = 128;
	public const int ConfigDecimalPrecision = 3;

	public static readonly Vector3[] QuadVertexData = {
		new Vector3(0, 0, 0), //0
		new Vector3(0, 1, 0), //1
		new Vector3(1, 0, 0), //2
		new Vector3(1, 1, 0) //3
	};


	public static readonly int[] QuadTriangleData = {
		0, 1, 3, //triangle 0
		0, 3, 2 //triangle 1
	};

	public static readonly Vector3[] BlockVertexData = {
		new Vector3(0, 0, 0),
		new Vector3(1, 0, 0),
		new Vector3(1, 1, 0),
		new Vector3(0, 1, 0),
		new Vector3(0, 1, 1),
		new Vector3(1, 1, 1),
		new Vector3(1, 0, 1),
		new Vector3(0, 0, 1),
	};


	public static readonly int[] BlockTriangleData = {
		0, 2, 1, //face front
		0, 3, 2,
		2, 3, 4, //face top
		2, 4, 5,
		1, 2, 5, //face right
		1, 5, 6,
		0, 7, 4, //face left
		0, 4, 3,
		5, 4, 7, //face back
		5, 7, 6,
		0, 6, 7, //face bottom
		0, 1, 6
	};

	public static readonly Vector3[] BlockVertexData2 = {
		new Vector3(0.0f, 0.0f, 0.0f),
		new Vector3(1.0f, 0.0f, 0.0f),
		new Vector3(1.0f, 1.0f, 0.0f),
		new Vector3(0.0f, 1.0f, 0.0f),
		new Vector3(0.0f, 0.0f, 1.0f),
		new Vector3(1.0f, 0.0f, 1.0f),
		new Vector3(1.0f, 1.0f, 1.0f),
		new Vector3(0.0f, 1.0f, 1.0f)
	};

	public static readonly int[] BlockTriangleData2 = {
		0, 3, 1, 2, // Back Face
		5, 6, 4, 7, // Front Face
		3, 7, 2, 6, // Top Face
		1, 5, 0, 4, // Bottom Face
		4, 7, 0, 3, // Left Face
		1, 2, 5, 6 // Right Face
	};

	/*
	 * TODO: Move all of this to some form of config file that is easier to edit and maybe can be automatically written
	 * to from the generation scenes
	 */

	public struct NoiseMapParams {
		public float offset, scale;
		public int octaveCount;
		public bool isSharp;
		public List<float> celThresholds;

		public NoiseMapParams(float offset, float scale, int octaveCount, bool isSharp, List<float> celThresholds) {
			this.offset = offset;
			this.scale = scale;
			this.octaveCount = octaveCount;
			this.isSharp = isSharp;
			this.celThresholds = celThresholds;
		}
	}

	public static readonly NoiseMapParams Continentalness =
		new NoiseMapParams(9123.78f, 0.05f, 6, false, new List<float> {0.158f, 0.321f, 0.406f, 0.505f, 0.622f});

	public static readonly NoiseMapParams Erosion =
		new NoiseMapParams(1123.08f, 0.03f, 3, false, new List<float> {0.117f, 0.261f, 0.504f, 0.711f});

	public static readonly NoiseMapParams PeaksAndValleys =
		new NoiseMapParams(123.02f, 0.17f, 1, true, new List<float> {0.091f, 0.195f, 0.267f, 0.489f});

	public static readonly Vector2[] ContinentalnessSplinePoints = {
		new Vector2(0f, 0f),
		new Vector2(0.092f, 0f),
		new Vector2(0.059f, 0f),
		new Vector2(0.131f, 0f),
		new Vector2(0.198f, 0f),
		new Vector2(0.223f, 0f),
		new Vector2(0.223f, 8.0f),
		new Vector2(0.223f, 29.8f),
		new Vector2(0.231f, 27.5f),
		new Vector2(0.324f, 27.7f),
		new Vector2(0.406f, 27.8f),
		new Vector2(0.404f, 26.5f),
		new Vector2(0.403f, 36.5f),
		new Vector2(0.403f, 46.6f),
		new Vector2(0.402f, 47.7f),
		new Vector2(0.404f, 57.7f),
		new Vector2(0.406f, 63.4f),
		new Vector2(0.937f, 64.6f),
		new Vector2(1f, 65.8f)
	};
	
	public static readonly Vector2[] ErosionSplinePoints = {
		
		new Vector2(0f, 125.3f),
		new Vector2(0.077f, 119.7f),
		new Vector2(.040f, 94.9f),
		new Vector2(0.108f, 96.3f),
		new Vector2(0.175f, 97.7f),
		new Vector2(0.182f, 68.9f),
		new Vector2(0.242f, 69.1f),
		new Vector2(0.281f, 69.3f),
		new Vector2(0.309f, 79.1f),
		new Vector2(0.337f, 79.2f),
		new Vector2(0.425f, 79.3f),
		new Vector2(0.451f, 19.7f),
		new Vector2(0.499f, 14.9f),
		new Vector2(0.529f, 12.1f),
		new Vector2(0.640f, 11.1f),
		new Vector2(0.677f, 13.2f),
		new Vector2(0.710f, 15.1f),
		new Vector2(0.737f, 37.6f),
		new Vector2(0.753f, 37.5f),
		new Vector2(0.785f, 37.3f),
		new Vector2(0.830f, 37.4f),
		new Vector2(0.863f, 37.4f),
		new Vector2(0.882f, 37.4f),
		new Vector2(0.900f, 15.2f),
		new Vector2(0.909f, 12.8f),
		new Vector2(0.916f, 10.5f),
		new Vector2(0.950f, 4.5f),
		new Vector2(1f, 4.8f)
	};

	public static readonly Vector2[] PeaksAndValleysSplinePoints = {
		new Vector2(0f, 0f),
		new Vector2(0.033f, 0f),
		new Vector2(0.058f, 0f),
		new Vector2(0.061f, 0f),
		new Vector2(0.098f, 6.0f),
		new Vector2(0.123f, 8.6f),
		new Vector2(0.158f, 14.6f),
		new Vector2(0.226f, 26.5f),
		new Vector2(0.241f, 32.6f),
		new Vector2(0.333f, 33.7f),
		new Vector2(0.412f, 34.6f),
		new Vector2(0.413f, 34.6f),
		new Vector2(0.532f, 34.7f),
		new Vector2(0.633f, 34.9f),
		new Vector2(0.631f, 48.1f),
		new Vector2(0.660f, 59.3f),
		new Vector2(0.680f, 66.7f),
		new Vector2(0.718f, 79.2f),
		new Vector2(0.795f, 84.4f),
		new Vector2(0.854f, 88.5f),
		new Vector2(0.927f, 87.5f),
		new Vector2(1f, 88.2f)
	};
}
