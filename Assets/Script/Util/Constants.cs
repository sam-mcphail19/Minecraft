using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants {
	public const int ChunkSize = 16;
	public const int WorldHeight = 128;

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
		1, 2, 5, 6  // Right Face
	};
}