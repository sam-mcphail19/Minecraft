using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WorldGenerator))]
public class WorldGeneratorEditor : Editor {
	public override void OnInspectorGUI() {
		WorldGenerator worldGenerator = (WorldGenerator) target;
		if (DrawDefaultInspector()) {
			if (worldGenerator.autoUpdate)
				worldGenerator.RegenerateWorld();

			if (GUILayout.Button("Generate"))
				worldGenerator.RegenerateWorld();
		}
	}
}
