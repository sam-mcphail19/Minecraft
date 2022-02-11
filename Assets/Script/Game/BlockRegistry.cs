using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MinecraftBlockRegistry;

public class BlockRegistry : MonoBehaviour {
	private static Dictionary<int, Block> blocks = new Dictionary<int, Block>();

	// Start is called before the first frame update
	void Start() { }

	// Update is called once per frame
	void Update() { }

	public static void Init() {
		AddBlock(new Block((int) BlockType.Air, "Air"));
		AddBlock(new Block((int) BlockType.Bedrock, "Bedrock"));
		AddBlock(new Block((int) BlockType.Stone, "Stone"));
		AddBlock(new Block((int) BlockType.Dirt, "Dirt"));
	}

	public static void AddBlock(Block block) {
		blocks.Add(block.GetIndex(), block);
	}

	public static Block GetBlock(int index) {
		if (blocks.TryGetValue(index, out Block block))
			return block;

		Debug.LogError("Failed to find index: " + index + " in block registry");

		blocks.TryGetValue(0, out block);
		return block;
	}
}

namespace MinecraftBlockRegistry {
	public enum BlockType {
		Null = -1,
		Air = 0,
		Bedrock = 1,
		Stone = 2,
		Dirt = 3
	}
}
