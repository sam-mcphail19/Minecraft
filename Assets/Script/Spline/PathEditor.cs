using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PathCreator))]
public class PathEditor : Editor {
	private PathCreator creator;
	private Path path;

	private bool onlyShowCurve = false;

	private const int yAxisHorizontalOffset = -10;
	private const int yAxisVerticalOffset = 3;
	private const int xAxisStretchFactor = 200;

	private void OnSceneGUI() {
		Tools.current = Tool.None;
		Handles.ScaleHandle(new Vector3(xAxisStretchFactor, 1, 1), Vector3.zero, Quaternion.identity, 1f);
		DrawAxes();
		Input();
		Draw();
	}

	void DrawAxes() {
		Handles.color = Color.white;

		Handles.DrawLine(new Vector3(0, 0), new Vector3(0, 128), 2);
		Handles.DrawLine(new Vector3(0, 0), new Vector3(1 * xAxisStretchFactor, 0), 2);

		Handles.Label(new Vector3(yAxisHorizontalOffset, 0 + yAxisVerticalOffset), "0");
		Handles.Label(new Vector3(yAxisHorizontalOffset, 25 + yAxisVerticalOffset), "25");
		Handles.Label(new Vector3(yAxisHorizontalOffset, 50 + yAxisVerticalOffset), "50");
		Handles.Label(new Vector3(yAxisHorizontalOffset, 75 + yAxisVerticalOffset), "75");
		Handles.Label(new Vector3(yAxisHorizontalOffset, 100 + yAxisVerticalOffset), "100");
		Handles.Label(new Vector3(yAxisHorizontalOffset, 128 + yAxisVerticalOffset), "128");

		Handles.Label(new Vector3(0, 0 - yAxisVerticalOffset), "0");
		Handles.Label(new Vector3(1 * xAxisStretchFactor, 0 - yAxisVerticalOffset), "1");
	}

	void Input() {
		Event guiEvent = Event.current;
		Vector2 mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;

		if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0) {
			if (guiEvent.shift) {
				Undo.RecordObject(creator, "Add segment");
				path.AddSegment(mousePos);
			}

			if (guiEvent.control) {
				Undo.RecordObject(creator, "Remove segment");
				path.RemoveSegment();
			}
		}

		if (guiEvent.type == EventType.KeyDown && guiEvent.keyCode == KeyCode.P) {
			JsonUtil.PrintToJson(path.WithScale(new Vector2(1f/xAxisStretchFactor, 1f)));
		}

		if (guiEvent.type == EventType.KeyDown && guiEvent.keyCode == KeyCode.C) {
			onlyShowCurve = !onlyShowCurve;
		}
	}

	void Draw() {
		for (int i = 0; i < path.NumSegments; i++) {
			Vector2[] points = path.GetPointsInSegment(i);
			Handles.color = Color.black;
			Handles.DrawBezier(points[0], points[3], points[1], points[2], Color.green, null, 2);

			if (onlyShowCurve) continue;

			Handles.DrawLine(points[1], points[0], 2);
			Handles.DrawLine(points[2], points[3], 2);
		}

		if (onlyShowCurve) return;

		Handles.color = Color.red;
		for (int i = 0; i < path.NumPoints; i++) {
			Vector2 newPos = Handles.FreeMoveHandle(path[i], Quaternion.identity, 2f, Vector2.zero,
				Handles.CylinderHandleCap);
			if (path[i] != newPos) {
				Undo.RecordObject(creator, "Move point");
				path.MovePoint(i, newPos);
			}
		}
	}

	private void OnEnable() {
		creator = (PathCreator) target;
		if (creator.path == null)
			creator.CreatePath();

		path = creator.path;
	}
}
