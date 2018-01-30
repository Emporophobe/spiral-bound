using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane : MonoBehaviour {

    private Rigidbody2D planeRigidbody;
    private Collider2D planeCollider;
    public float liftCoefficient = 0.05f;
    public float dragCoefficient = 0.05f;
    public Vector2 initialVelocity = new Vector2(5, 0);

    void Awake () {
        planeRigidbody = GetComponent<Rigidbody2D>();
        planeCollider = GetComponent<Collider2D>();

        planeRigidbody.velocity = initialVelocity;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Physics update
    private void FixedUpdate()
    {
        Vector2 currentVelocity = planeRigidbody.velocity;
        float angleOfAttack = planeRigidbody.rotation;
        float ySize = Mathf.Pow(Mathf.Sin(angleOfAttack) * planeCollider.bounds.size.y, 2);
        float xSize = Mathf.Pow(Mathf.Cos(angleOfAttack) * planeCollider.bounds.size.x, 2);

        // Apply lift
        Vector2 lift = new Vector2(0, xSize * liftCoefficient * currentVelocity.x * Mathf.Cos(angleOfAttack));
        print("lift: " + lift);
        planeRigidbody.velocity += lift;
        // Apply drag
        Vector2 xDrag = new Vector2(ySize * dragCoefficient * Mathf.Pow(currentVelocity.x, 1), 0);
        planeRigidbody.velocity -= xDrag;
        Vector2 yDrag = new Vector2(0, xSize * dragCoefficient * Mathf.Pow(currentVelocity.y, 1));
        planeRigidbody.velocity -= yDrag;
        print("drag: " + xDrag + " " + yDrag);
        // Apply gravity
        Vector2 gravity = new Vector2(0, (float) -9.81 * Time.deltaTime);
        planeRigidbody.velocity += gravity;
        print("gravity: " + gravity);

        // Apply torque
        planeRigidbody.AddTorque(Input.GetAxis("Horizontal") / 1);

        // Apply corrective torque
        planeRigidbody.AddTorque((Mathf.Atan2(currentVelocity.y, currentVelocity.x) - angleOfAttack) * currentVelocity.magnitude / 8);
    }
}
