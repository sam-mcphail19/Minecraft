using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block {
	private int index;
	private string blockName;
	private Texture2D texture;
	private int textureId;

	public Block(int index, string name, string texturePath, int textureId) {
		this.index = index;
		this.blockName = name;
		if (textureId >= 0) {
			Texture2D texture = Resources.Load<Texture2D>(texturePath);
			this.texture = new Texture2D(texture.width, texture.height, TextureFormat.RGB24, false) {
				name = name
			};
			this.texture.SetPixels(texture.GetPixels());
			this.texture.Apply();
			this.textureId = textureId;
		}
	}

	public int GetIndex() {
		return index;
	}

	public string GetName() {
		return blockName;
	}

	public int GetTextureId() {
		return textureId;
	}

	public Texture2D GetTexture() {
		return texture;
	}
}
