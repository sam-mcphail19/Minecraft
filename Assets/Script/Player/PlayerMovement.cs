using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
	public CharacterController controller;

	public float speed = 12f;
	public float gravity = -9.81f;
	public float maxFallSpeed = -2f;
	public float jumpHeight = 2f;

	public Transform groundCheck;
	public float groundDistance = 0.4f;
	public LayerMask groundMask;

	private Vector3 velocity;
	private bool isGrounded;
	private float distToGround;

	// Start is called before the first frame update
	void Start() {
		distToGround = controller.center.y + controller.height / 2f + 0.1f;
	}

	// Update is called once per frame
	void Update() {
		CheckIfGrounded();

		if (isGrounded && velocity.y < 0)
			velocity.y = maxFallSpeed;

		float x = Input.GetAxis("Horizontal");
		float z = Input.GetAxis("Vertical");

		Vector3 move = transform.right * x + transform.forward * z;
		controller.Move(move * speed * Time.deltaTime);

		if (Input.GetButtonDown("Jump") && isGrounded)
			velocity.y = Mathf.Sqrt(jumpHeight * gravity * -2f);

		velocity.y += gravity * Time.deltaTime;

		controller.Move(velocity * Time.deltaTime);
	}

	void CheckIfGrounded() {
		Vector3 pos = controller.transform.position;
		float radius = controller.radius;
		
		Vector3 right = pos + new Vector3(radius, 0, 0);
		Vector3 left = pos - new Vector3(radius, 0, 0);
		Vector3 forward = pos - new Vector3(0, 0, radius);
		Vector3 back = pos + new Vector3(0, 0, radius);
		isGrounded = Physics.Raycast(right, Vector3.down, distToGround) ||
		             Physics.Raycast(left, Vector3.down, distToGround) ||
		             Physics.Raycast(forward, Vector3.down, distToGround) ||
		             Physics.Raycast(back, Vector3.down, distToGround);
	}
}
