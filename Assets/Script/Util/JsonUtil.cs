using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public static class JsonUtil {

	public static List<T> Load<T>(string path) {
		using (StreamReader r = new StreamReader(path)) {
			string json = r.ReadToEnd();
			return JsonConvert.DeserializeObject<List<T>>(json);
		}
	}

	public static void PrintToJson<T>(T obj) {
		Debug.Log(JsonConvert.SerializeObject(obj));
	}

	public static List<NoiseMap> LoadNoiseMaps() {
		return Load<NoiseMap>("Assets/Config/Noise.json");
	}
	
	public static List<Path> LoadNoiseMapHeights() {
		return Load<Path>("Assets/Config/NoiseMapHeight.json");
	}
}
