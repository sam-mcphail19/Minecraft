using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;

public class WorldGenerator : MonoBehaviour {
	public int width;
	public int height;
	public int seed;
	public float scale;
	public int octaves;
	public float persistance;
	public float lacunarity;
	public Vector2 offset = Vector2.zero;
	public bool autoUpdate;

	private List<Chunk> chunks = new List<Chunk>();

	// Start is called before the first frame update
	void Start() {
		BlockRegistry.Init();
		GenerateWorld();
	}

	// Update is called once per frame
	void Update() { }

	public void GenerateWorld() {
		NoiseGenerator.Init(width, height, seed, scale, octaves, persistance, lacunarity, offset);
		for (int i = 0; i < 5; i++) {
			for (int j = 0; j < 5; j++) {
				GameObject newChunk = new GameObject("Chunk" + (j + i * 5));
				Chunk chunk = newChunk.AddComponent<Chunk>();
				chunks.Add(chunk);
				chunk.SetChunkOrigin(new Vector3(i * Constants.ChunkSize, 0, j * Constants.ChunkSize));
				chunk.Generate();
			}
		}
	}

	[Button]
	public void RegenerateWorld() {
		if (chunks.Count > 0) {
			foreach (Chunk t in chunks) {
				Destroy(t.gameObject);
			}

			chunks = new List<Chunk>();
		}

		GenerateWorld();
	}
}
