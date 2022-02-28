using System;
using Newtonsoft.Json;
using UnityEngine;

public class SerializedVector2 {

	private float _x;
	private float _y;


	public float x {
		get => _x;
		set => _x = (float) Math.Round(value, Constants.ConfigDecimalPrecision);
	}

	public float y {
		get => _y;
		set => _y = (float) Math.Round(value, Constants.ConfigDecimalPrecision);
	}


	[JsonConstructor]
	public SerializedVector2(float x, float y) {
		this.x = x;
		this.y = y;
	}

	public SerializedVector2(Vector2 vector2) {
		this.x = vector2.x;
		this.y = vector2.y;
	}

	public static SerializedVector2 operator +(SerializedVector2 a, SerializedVector2 b) =>
		new SerializedVector2(a.x + b.x, a.y + b.y);

	public static SerializedVector2 operator -(SerializedVector2 a, SerializedVector2 b) =>
		new SerializedVector2(a.x - b.x, a.y - b.y);

	public static SerializedVector2 operator *(SerializedVector2 vec, float a) =>
		new SerializedVector2(vec.x * a, vec.y * a);

	public Vector2 ToVector2() {
		return new Vector2(x, y);
	}
}
