using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants {
	public const int ChunkSize = 16;
	public const int WorldHeight = 128;
	public const int WaterLevel = 50;
	public const int ConfigDecimalPrecision = 3;


	public static readonly Vector3[] BlockVertexData = {
		new Vector3(0.0f, 0.0f, 0.0f),
		new Vector3(1.0f, 0.0f, 0.0f),
		new Vector3(1.0f, 1.0f, 0.0f),
		new Vector3(0.0f, 1.0f, 0.0f),
		new Vector3(0.0f, 0.0f, 1.0f),
		new Vector3(1.0f, 0.0f, 1.0f),
		new Vector3(1.0f, 1.0f, 1.0f),
		new Vector3(0.0f, 1.0f, 1.0f)
	};

	public static readonly int[] BlockTriangleData = {
		0, 3, 1, 2, // Back Face
		5, 6, 4, 7, // Front Face
		3, 7, 2, 6, // Top Face
		1, 5, 0, 4, // Bottom Face
		4, 7, 0, 3, // Left Face
		1, 2, 5, 6 // Right Face
	};
}
