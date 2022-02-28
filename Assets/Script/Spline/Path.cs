using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class Path {
	[SerializeField, HideInInspector] private List<Vector2> points;

	public string MapName = "";
	public List<SerializedVector2> Points => points.Select(x => new SerializedVector2(x)).ToList();

	public Path(Vector2 centre) {
		points = new List<Vector2> {
			centre + Vector2.left,
			centre + (Vector2.left + Vector2.up) * 0.5f,
			centre + (Vector2.right + Vector2.down) * 0.5f,
			centre + Vector2.right
		};
	}

	public Path(Vector2[] points) {
		this.points = new List<Vector2>(points);
	}

	[JsonConstructor]
	public Path(string mapName, List<SerializedVector2> points) {
		this.MapName = mapName;
		this.points = points.Select(x => x.ToVector2()).ToList();
	}

	public Vector2 this[int i] => points[i];

	public int NumPoints => points.Count;

	public int NumSegments => (points.Count - 4) / 3 + 1;

	public void AddSegment(Vector2 anchorPos) {
		points.Add(points[points.Count - 1] * 2 - points[points.Count - 2]);
		points.Add((points[points.Count - 1] + anchorPos) * 0.5f);
		points.Add(anchorPos);
	}

	public float Evaluate(float x) {
		Vector2[] segmentPoints = Array.Empty<Vector2>();
		for (int i = 0; i < NumSegments; i++) {
			segmentPoints = GetPointsInSegment(i);
			if (segmentPoints[0].x <= x && segmentPoints[3].x >= x)
				break;
		}

		float t = (x - segmentPoints[0].x) / (segmentPoints[3] - segmentPoints[0]).x;
		return MathUtil.EvaluateCubic(segmentPoints, t).y;
	}

	public Vector2[] GetPointsInSegment(int i) {
		return new[] {
			points[i * 3],
			points[i * 3 + 1],
			points[i * 3 + 2],
			points[i * 3 + 3]
		};
	}

	public void MovePoint(int i, Vector2 pos) {
		Vector2 deltaMove = pos - points[i];
		points[i] = pos;

		if (i % 3 == 0) {
			if (i + 1 < points.Count)
				points[i + 1] += deltaMove;
			if (i - 1 >= 0)
				points[i - 1] += deltaMove;
		} else {
			bool nextPointIsAnchor = (i + 1) % 3 == 0;
			int correspondingControlIndex = nextPointIsAnchor ? i + 2 : i - 2;
			int anchorIndex = nextPointIsAnchor ? i + 1 : i - 1;

			if (correspondingControlIndex > 0 && correspondingControlIndex < points.Count) {
				float distance = (points[anchorIndex] - points[correspondingControlIndex]).magnitude;
				Vector2 dir = (points[anchorIndex] - pos).normalized;
				points[correspondingControlIndex] = points[anchorIndex] + dir * distance;
			}
		}
	}

	public void RemoveSegment() {
		if (points.Count < 7) return;

		points.RemoveAt(points.Count - 1);
		points.RemoveAt(points.Count - 2);
		points.RemoveAt(points.Count - 3);
	}

	public void PrintPoints() {
		string str = "[";
		for (int i = 0; i < NumPoints; i++) {
			str += points[i].ToString();

			if (i < NumPoints - 1) {
				str += ",\n";
			}
		}

		str += "\n]";
		Debug.Log(str);
	}

	public Path WithScale(Vector2 scale) {
		return new Path(points.Select(vec => new Vector2(vec.x * scale.x, vec.y * scale.y)).ToArray());
	}
}
