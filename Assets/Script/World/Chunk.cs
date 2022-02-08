using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Chunk : MonoBehaviour {
	private int[] blocks = new int[Constants.ChunkSize * Constants.ChunkSize * Constants.WorldHeight];
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
	
	public void Generate() {
		Populate();
		InitMesh();
		CreateMesh();
	}

	void Populate() {
		for (int x = 0; x < Constants.ChunkSize; x++) {
			for (int z = 0; z < Constants.ChunkSize; z++) {
				int y = Mathf.CeilToInt(Mathf.PerlinNoise(x * 0.3f, z * 0.3f) * 2f);
				Vector3 blockPos = new Vector3(x, y, z);
				SetBlock(blockPos, BlockRegistry.GetBlock((int) BlockRegistry.BLOCKS.DIRT));
			}
		}
	}

	void CreateMesh() {
		for (int i = 0; i < Constants.ChunkSize * Constants.ChunkSize * Constants.WorldHeight; i++) {
			if (blocks[i] == (int) BlockRegistry.BLOCKS.AIR)
				continue;

			for (int j = 0; j < 6; j++) {
				CreateQuad((Direction) j, IndexToPos(i) + chunkOrigin);
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

	void InitMesh() {
		if (!meshFilter)
			meshFilter = gameObject.AddComponent<MeshFilter>();
		if (!meshRenderer) {
			meshRenderer = gameObject.AddComponent<MeshRenderer>();
			meshRenderer.material = GetComponent<Renderer>().material;
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

	void CreateQuad(Direction direction, Vector3 pos) {
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

		uv.AddRange(new[] {new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 0), new Vector2(0, 1)});
	}

	Block GetBlock(Vector3 chunkPos) {
		return BlockRegistry.GetBlock(blocks[PosToIndex(chunkPos)]);
	}

	void SetBlock(Vector3 chunkPos, Block block) {
		blocks[PosToIndex(chunkPos)] = block.GetIndex();
	}

	public void SetChunkOrigin(Vector3 chunkOrigin) {
		this.chunkOrigin = chunkOrigin;
	}

	// https://stackoverflow.com/questions/7367770/how-to-flatten-or-index-3d-array-in-1d-array
	static Vector3 IndexToPos(int index) {
		int y = index / (Constants.ChunkSize * Constants.ChunkSize);
		index -= y * Constants.ChunkSize * Constants.ChunkSize;
		int z = index / Constants.ChunkSize;
		int x = index % Constants.ChunkSize;

		return new Vector3(x, y, z);
	}

	static int PosToIndex(Vector3 pos) {
		return (int) (pos.y * Constants.ChunkSize * Constants.ChunkSize + pos.z * Constants.ChunkSize + pos.x);
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