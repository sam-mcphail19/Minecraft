using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NoiseMap))]
public class NoiseMapEditor : Editor {
	public override void OnInspectorGUI() {
		NoiseMap noiseMap = (NoiseMap) target;

		if (DrawDefaultInspector()) {
			if (noiseMap.autoUpdate) {
				noiseMap.SetNoiseTexture();
			}
		}

		if (GUILayout.Button("Generate")) {
			noiseMap.SetNoiseTexture();
		}
	}
}
