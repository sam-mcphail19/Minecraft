using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using MinecraftBlockRegistry;

public static class BlockRegistry {
	private static Dictionary<int, Block> blocks = new Dictionary<int, Block>();
	private static Texture2D textureAtlas;
	private static int textureRes = 32;
	private static int textureAtlasSize;

	public static void Init() {
		AddBlock(new Block((int) BlockType.Air, "Air", "", -1));
		AddBlock(new Block((int) BlockType.Bedrock, "Bedrock", "Texture/bedrock", 0));
		AddBlock(new Block((int) BlockType.Stone, "Stone", "Texture/stone", 1));
		AddBlock(new Block((int) BlockType.Dirt, "Dirt", "Texture/dirt", 2));

		CreateTextureAtlas();
	}

	static void CreateTextureAtlas() {
		// no texture for air
		textureAtlasSize = Mathf.CeilToInt(Mathf.Sqrt(blocks.Count - 1));
		int atlasPixelSize = textureAtlasSize * textureRes;
		textureAtlas = new Texture2D(atlasPixelSize, atlasPixelSize) {
			filterMode = FilterMode.Point
		};

		List<Texture2D> textures = new List<Texture2D>();
		for (int i = 1; i < blocks.Count; i++) {
			textures.Add(blocks[i].GetTexture());
		}

		Color[] atlas = new Color[atlasPixelSize * atlasPixelSize];

		for (int i = 0; i < textures.Count; i++) {
			Color[] tex = textures[i].GetPixels();

			int texStartRow = (i / textureAtlasSize) * textureRes;
			int texStartCol = (textureRes * i) % atlasPixelSize;

			for (int j = 0; j < textureRes * textureRes; j++) {
				int row = j / textureRes + texStartRow;
				int col = j % textureRes + texStartCol;
				atlas[col + row * atlasPixelSize] = tex[j];
			}
		}

		textureAtlas.SetPixels(atlas.ToArray());
		textureAtlas.Apply();

		string destinationPath = Directory.GetCurrentDirectory() + "/Assets/Resources/Texture/textureAtlas.png";
		FileStream file = File.Exists(destinationPath) ? File.OpenWrite(destinationPath) : File.Create(destinationPath);
		byte[] newPng = textureAtlas.EncodeToPNG();
		file.Write(newPng, 0, newPng.Length);
		file.Close();
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

	public static Texture2D GetTextureAtlas() {
		return textureAtlas;
	}

	public static int GetTextureId(int index) {
		return index - 1;
	}

	public static Vector2[] GetTextureVertices(int textureId) {
		int textureAtlasSizeInPixels = textureAtlasSize * textureRes;
		int texStartRow = (textureId / textureAtlasSize) * textureRes;
		int texStartCol = (textureRes * textureId) % textureAtlasSizeInPixels;
		

		return new[] {
			new Vector2(
				texStartCol / (float) textureAtlasSizeInPixels,
				texStartRow / (float) textureAtlasSizeInPixels),
			new Vector2(
				texStartCol / (float) textureAtlasSizeInPixels,
				(texStartRow + textureRes) / (float) textureAtlasSizeInPixels),
			new Vector2(
				(texStartCol + textureRes) / (float) textureAtlasSizeInPixels,
				texStartRow / (float) textureAtlasSizeInPixels),
			new Vector2(
				(texStartCol + textureRes) / (float) textureAtlasSizeInPixels,
				(texStartRow + textureRes) / (float) textureAtlasSizeInPixels)
		};
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
