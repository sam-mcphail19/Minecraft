using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block {
	private int index;
	private string blockName;

	public Block(int index, string name) {
		this.index = index;
		this.blockName = name;
	}

	public int GetIndex() {
		return index;
	}

	public string GetName() {
		return blockName;
	}
}