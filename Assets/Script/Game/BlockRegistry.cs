using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRegistry : MonoBehaviour {
	private static Dictionary<int, Block> blocks = new Dictionary<int, Block>();

	// Start is called before the first frame update
	void Start() { }

	// Update is called once per frame
	void Update() { }

	public static void Init() {
		AddBlock(new Block((int) BLOCKS.AIR, "Air"));
		AddBlock(new Block((int) BLOCKS.DIRT, "Dirt"));
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

	public enum BLOCKS {
		AIR = 0,
		DIRT = 1
	}
}