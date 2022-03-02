using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NoiseMapCreator))]
public class NoiseMapCreatorEditor : Editor {
	public override void OnInspectorGUI() {
		NoiseMapCreator noiseMapCreator = (NoiseMapCreator) target;

		if (DrawDefaultInspector()) {
			if (noiseMapCreator.autoUpdate) {
				noiseMapCreator.SetNoiseTexture();
			}
		}

		if (GUILayout.Button("Generate")) {
			noiseMapCreator.SetNoiseTexture();
		}
	}
}
