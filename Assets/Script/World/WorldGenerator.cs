using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;

public class WorldGenerator : MonoBehaviour {
	public int octaves = 5;
	public float persistence = 0.5f;
	public int seed = 1234;
	public bool eased = true;

	private List<Chunk> chunks = new List<Chunk>();

	// Start is called before the first frame update
	void Start() {
		BlockRegistry.Init();
		GenerateWorld();
	}

	// Update is called once per frame
	void Update() { }
	
	void GenerateWorld() {
		for (int i = -2; i < 2; i++) {
			for (int j = -2; j < 2; j++) {
				GameObject newChunk = new GameObject("Chunk");
				Chunk chunk = newChunk.AddComponent<Chunk>();
				chunks.Add(chunk);
				chunk.SetChunkOrigin(new Vector3(i * Constants.ChunkSize, 0, j * Constants.ChunkSize));
				chunk.Generate();
			}
		}
	}

	[Button]
	void RegenerateWorld() {
		if (chunks.Count > 0) {
			foreach (Chunk t in chunks) {
				Destroy(t.gameObject);
			}
			chunks = new List<Chunk>();
		}
		GenerateWorld();
	}
}