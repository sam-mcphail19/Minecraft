using System;
using System.Collections;
using System.Collections.Generic;
using EasyButtons;
using UnityEngine;
using MinecraftBlockRegistry;

public class World : MonoBehaviour {
	public float offset;
	public float scale;
	public int seed;

	private Dictionary<Vector3, Chunk> chunks = new Dictionary<Vector3, Chunk>();

	// Start is called before the first frame update
	void Start() {
		BlockRegistry.Init();
		GenerateWorld();

		GameObject.Find("Player").transform.position =
			new Vector3(0, chunks[Vector3.zero].GetHighestBlockHeightAtPoint(0, 0) + 3, 0);
	}

	public void GenerateWorld() {
		int xChunks = 8;
		int zChunks = 8;

		for (int x = -xChunks / 2; x < xChunks / 2; x++) {
			for (int z = -zChunks / 2; z < zChunks / 2; z++) {
				GameObject newChunk = new GameObject("Chunk" + ((z + zChunks / 2) + (x + xChunks / 2) * zChunks));
				Chunk chunk = newChunk.AddComponent<Chunk>();
				chunk.transform.SetParent(transform);
				Vector3 origin = new Vector3(x * Constants.ChunkSize, 0, z * Constants.ChunkSize);
				chunks.Add(origin, chunk);
				chunk.SetChunkOrigin(origin);
				chunk.Generate(this);
			}
		}
	}

	public int GetBlock(int x, int y, int z) {
		if (y <= 3)
			return (int) BlockType.Bedrock;

		float noise = NoiseGenerator.Perlin2D(new Vector2(x, z), offset, scale, seed);
		int height = Mathf.FloorToInt(42 * noise);

		if (y > height)
			return (int) BlockType.Air;

		if (y > height - 3)
			return (int) BlockType.Dirt;

		return (int) BlockType.Stone;
	}

	public int GetBlock(Vector3 pos) {
		return GetBlock((int) pos.x, (int) pos.y, (int) pos.z);
	}

	[Button]
	public void RegenerateWorld() {
		if (chunks.Count > 0) {
			foreach (Chunk chunk in chunks.Values) {
				Destroy(chunk.gameObject);
			}

			chunks = new Dictionary<Vector3, Chunk>();
		}

		GenerateWorld();
	}
}
