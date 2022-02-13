using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using MinecraftBlockRegistry;

public class Chunk : MonoBehaviour {
	private int[,,] blocks = new int[Constants.ChunkSize, Constants.WorldHeight, Constants.ChunkSize];
	private Vector3 chunkOrigin;

	private MeshFilter meshFilter;
	private MeshRenderer meshRenderer;
	private MeshCollider meshCollider;
	private Mesh mesh;

	private int vertexCount = 0;
	private List<Vector3> vertices = new List<Vector3>();
	private List<int> triangles = new List<int>();
	private List<Vector3> normals = new List<Vector3>();
	private List<Vector2> uv = new List<Vector2>();

	// Start is called before the first frame update
	void Start() {
		// Ground layer
		gameObject.layer = 6;
	}

	// Update is called once per frame
	void Update() { }

	public void Generate(World world) {
		Populate(world);
		InitMesh();
		CreateMesh();
	}

	public void Populate(World world) {
		for (int x = 0; x < Constants.ChunkSize; x++) {
			for (int z = 0; z < Constants.ChunkSize; z++) {
				for (int y = 0; y < Constants.WorldHeight; y++) {
					Vector3 pos = new Vector3(x, y, z);
					SetBlock(pos, BlockRegistry.GetBlock(world.GetBlock(pos + chunkOrigin)));
				}
			}
		}
	}

	public void InitMesh() {
		if (!meshFilter)
			meshFilter = gameObject.AddComponent<MeshFilter>();
		if (!meshRenderer) {
			meshRenderer = gameObject.AddComponent<MeshRenderer>();
			meshRenderer.material = new Material(Shader.Find("Standard")) {
				mainTexture = BlockRegistry.GetTextureAtlas()
			};
		}

		if (!meshCollider)
			meshCollider = gameObject.AddComponent<MeshCollider>();

		mesh = new Mesh();

		vertices = new List<Vector3>();
		triangles = new List<int>();
		normals = new List<Vector3>();
		uv = new List<Vector2>();

		vertexCount = 0;
	}

	public void CreateMesh() {
		for (int x = 0; x < Constants.ChunkSize; x++) {
			for (int z = 0; z < Constants.ChunkSize; z++) {
				for (int y = 0; y < Constants.WorldHeight; y++) {
					if (blocks[x, y, z] == (int) BlockType.Air)
						continue;

					List<BlockType> neighbours = GetNeighbours(x, y, z);
					if (neighbours.Contains(BlockType.Air) || neighbours.Contains(BlockType.Null)) {
						for (int j = 0; j < 6; j++) {
							CreateQuad(
								(Direction) j,
								new Vector3(x, y, z) + chunkOrigin,
								BlockRegistry.GetTextureId(blocks[x, y, z])
							);
						}
					}
				}
			}
		}

		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.normals = normals.ToArray();
		mesh.uv = uv.ToArray();

		mesh.RecalculateBounds();
		meshFilter.mesh = mesh;
		meshCollider.sharedMesh = mesh;
	}

	public void UpdateMesh() {
		InitMesh();
		CreateMesh();
	}

	void CreateQuad(Direction direction, Vector3 pos, int textureId) {
		vertices.Add(Constants.BlockVertexData2[Constants.BlockTriangleData2[0 + (int) direction * 4]] + pos);
		vertices.Add(Constants.BlockVertexData2[Constants.BlockTriangleData2[1 + (int) direction * 4]] + pos);
		vertices.Add(Constants.BlockVertexData2[Constants.BlockTriangleData2[2 + (int) direction * 4]] + pos);
		vertices.Add(Constants.BlockVertexData2[Constants.BlockTriangleData2[3 + (int) direction * 4]] + pos);

		triangles.Add(vertexCount);
		triangles.Add(vertexCount + 1);
		triangles.Add(vertexCount + 2);
		triangles.Add(vertexCount + 2);
		triangles.Add(vertexCount + 1);
		triangles.Add(vertexCount + 3);

		vertexCount += 4;

		Vector3 normal = DirectionToVector(direction);
		normals.AddRange(new[] {normal, normal, normal, normal});

		uv.AddRange(BlockRegistry.GetTextureVertices(textureId));
	}

	public int GetHighestBlockHeightAtPoint(int x, int z) {
		if (x < 0 || x >= Constants.ChunkSize || z < 0 || z >= Constants.ChunkSize) {
			Debug.LogError($"({x},{z}) is not within the chunk");
			return -1;
		}

		for (int y = Constants.WorldHeight - 1; y >= 0; y--) {
			if (blocks[x, y, z] != (int) BlockType.Air) {
				return y;
			}
		}

		Debug.LogError($"No block found in chunk at: ({x},{z})");
		return -1;
	}

	public List<BlockType> GetNeighbours(int x, int y, int z) {
		// Order is right, left, up, down, front, back
		return new List<BlockType> {
			x < Constants.ChunkSize - 1 ? (BlockType) blocks[x + 1, y, z] : BlockType.Null,
			x > 0 ? (BlockType) blocks[x - 1, y, z] : BlockType.Null,
			y < Constants.WorldHeight - 1 ? (BlockType) blocks[x, y + 1, z] : BlockType.Null,
			y > 0 ? (BlockType) blocks[x, y - 1, z] : BlockType.Null,
			z < Constants.ChunkSize - 1 ? (BlockType) blocks[x, y, z + 1] : BlockType.Null,
			z > 0 ? (BlockType) blocks[x, y, z - 1] : BlockType.Null
		};
	}

	public Block GetBlock(int x, int y, int z) {
		return BlockRegistry.GetBlock(blocks[x, y, z]);
	}

	public Block GetBlock(Vector3 chunkPos) {
		return GetBlock((int) chunkPos.x, (int) chunkPos.y, (int) chunkPos.z);
	}

	public void SetBlock(int x, int y, int z, Block block) {
		blocks[x, y, z] = block.GetIndex();
	}

	public void SetBlock(Vector3 chunkPos, Block block) {
		SetBlock((int) chunkPos.x, (int) chunkPos.y, (int) chunkPos.z, block);
	}

	public Vector3 GetChunkOrigin() {
		return chunkOrigin;
	}

	public void SetChunkOrigin(Vector3 chunkOrigin) {
		this.chunkOrigin = chunkOrigin;
	}

	Vector3 DirectionToVector(Direction direction) {
		switch (direction) {
			case Direction.Forward: return Vector3.forward;
			case Direction.Top: return Vector3.up;
			case Direction.Right: return Vector3.right;
			case Direction.Back: return Vector3.back;
			case Direction.Left: return Vector3.left;
			case Direction.Bottom: return Vector3.down;
			default: throw new InvalidEnumArgumentException("Invalid direction");
		}
	}

	enum Direction {
		Back,
		Forward,
		Top,
		Bottom,
		Left,
		Right
	}
}
