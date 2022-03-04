using System.Collections.Generic;
using System.ComponentModel;
using MinecraftBlockRegistry;
using UnityEngine;

public class Chunk : MonoBehaviour {
	private int[,,] blocks = new int[Constants.ChunkSize, Constants.WorldHeight, Constants.ChunkSize];
	private Vector3 chunkOrigin;

	private MeshFilter meshFilter;
	private MeshRenderer meshRenderer;
	private MeshCollider meshCollider;
	private Mesh mesh;

	public int vertexCount;
	private List<Vector3> vertices = new List<Vector3>();
	private List<int> triangles = new List<int>();
	private List<int> waterTriangles = new List<int>();
	private List<Vector3> normals = new List<Vector3>();
	private List<Vector2> uv = new List<Vector2>();

	// Start is called before the first frame update
	void Start() {
		// Ground layer
		gameObject.layer = 6;
	}

	public void Generate() {
		Populate();
		InitMesh();
		CreateMesh();
	}

	public void Populate() {
		for (int x = 0; x < Constants.ChunkSize; x++) {
			for (int z = 0; z < Constants.ChunkSize; z++) {
				for (int y = 0; y < Constants.WorldHeight; y++) {
					Vector3 pos = new Vector3(x, y, z);
					SetBlock(pos, BlockRegistry.GetBlock(World.GetBlock(pos + chunkOrigin).GetIndex()));
				}
			}
		}
	}

	public void InitMesh() {
		if (!meshFilter)
			meshFilter = gameObject.AddComponent<MeshFilter>();
		if (!meshRenderer) {
			Material[] materials = new Material[2];
			materials[0] = new Material(Shader.Find("Standard")) {
				mainTexture = BlockRegistry.GetTextureAtlas()
			};
			materials[1] = new Material(Shader.Find("Standard")) {
				mainTexture = BlockRegistry.GetTextureAtlas()
			};
			StandardShaderUtils.ChangeRenderMode(materials[1], StandardShaderUtils.BlendMode.Transparent);

			meshRenderer = gameObject.AddComponent<MeshRenderer>();
			meshRenderer.materials = materials;
		}

		if (!meshCollider)
			meshCollider = gameObject.AddComponent<MeshCollider>();

		mesh = new Mesh {
			subMeshCount = 2
		};

		vertices = new List<Vector3>();
		triangles = new List<int>();
		waterTriangles = new List<int>();
		normals = new List<Vector3>();
		uv = new List<Vector2>();

		vertexCount = 0;
	}

	public void CreateMesh() {
		for (int x = 0; x < Constants.ChunkSize; x++) {
			for (int z = 0; z < Constants.ChunkSize; z++) {
				for (int y = 0; y < Constants.WorldHeight; y++) {
					if (blocks[x, y, z] == BlockType.Air.GetIndex())
						continue;

					List<BlockType> neighbours = GetNeighbours(x, y, z);
					for (int j = 0; j < 6; j++) {
						Block block = BlockRegistry.GetBlock(blocks[x, y, z]);
						if (block.GetBlockState() == BlockState.Transparent) {
							if (neighbours[j] == BlockType.Null || neighbours[j] == BlockType.Air)
								CreateQuad((Direction) j, new Vector3(x, y, z) + chunkOrigin, block.GetTextureId());
						} else {
							if (neighbours[j] == BlockType.Null ||
							    BlockRegistry.GetBlockState(neighbours[j]) == BlockState.Transparent)
								CreateQuad((Direction) j, new Vector3(x, y, z) + chunkOrigin, block.GetTextureId());
						}
					}
				}
			}
		}
	}

	public void ApplyMesh() {
		mesh.vertices = vertices.ToArray();
		mesh.SetTriangles(triangles.ToArray(), 0);
		mesh.SetTriangles(waterTriangles.ToArray(), 1);
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

	public void DestroyMesh() {
		InitMesh();
		ApplyMesh();
	}

	void CreateQuad(Direction direction, Vector3 pos, int textureId) {
		vertices.Add(Constants.BlockVertexData[Constants.BlockTriangleData[0 + (int) direction * 4]] + pos);
		vertices.Add(Constants.BlockVertexData[Constants.BlockTriangleData[1 + (int) direction * 4]] + pos);
		vertices.Add(Constants.BlockVertexData[Constants.BlockTriangleData[2 + (int) direction * 4]] + pos);
		vertices.Add(Constants.BlockVertexData[Constants.BlockTriangleData[3 + (int) direction * 4]] + pos);

		if (textureId != BlockRegistry.GetTextureId(BlockType.Water)) {
			triangles.Add(vertexCount);
			triangles.Add(vertexCount + 1);
			triangles.Add(vertexCount + 2);
			triangles.Add(vertexCount + 2);
			triangles.Add(vertexCount + 1);
			triangles.Add(vertexCount + 3);
		} else {
			waterTriangles.Add(vertexCount);
			waterTriangles.Add(vertexCount + 1);
			waterTriangles.Add(vertexCount + 2);
			waterTriangles.Add(vertexCount + 2);
			waterTriangles.Add(vertexCount + 1);
			waterTriangles.Add(vertexCount + 3);
		}

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
			if (blocks[x, y, z] != BlockType.Air.GetIndex()) {
				return y;
			}
		}

		Debug.LogError($"No block found in chunk at: ({x},{z})");
		return -1;
	}

	public List<BlockType> GetNeighbours(int x, int y, int z) {
		// Order is front, back, top, bottom, left, right
		return new List<BlockType> {
			z > 0 ? (BlockType) blocks[x, y, z - 1] : BlockType.Null,
			z < Constants.ChunkSize - 1 ? (BlockType) blocks[x, y, z + 1] : BlockType.Null,
			y < Constants.WorldHeight - 1 ? (BlockType) blocks[x, y + 1, z] : BlockType.Null,
			y > 0 ? (BlockType) blocks[x, y - 1, z] : BlockType.Null,
			x > 0 ? (BlockType) blocks[x - 1, y, z] : BlockType.Null,
			x < Constants.ChunkSize - 1 ? (BlockType) blocks[x + 1, y, z] : BlockType.Null
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
