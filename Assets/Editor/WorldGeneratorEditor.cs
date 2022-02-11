using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(World))]
public class WorldGeneratorEditor : Editor {
	public override void OnInspectorGUI() {
		World world = (World) target;
		if (DrawDefaultInspector()) {
			if (GUILayout.Button("Generate"))
				world.RegenerateWorld();
		}
	}
}
