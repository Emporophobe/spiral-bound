using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private Rigidbody2D planeRigidbody;
    private Collider2D planeCollider;
    public float liftCoefficient = 0.05f;
    public float dragCoefficient = 0.05f;
    public Vector2 initialVelocity = new Vector2(5, 0);
    public bool useSimpleControls = true;
    public float basicSpeed = 5;

    public bool IsAlive { get; set; }

    void Awake () {
        planeRigidbody = GetComponent<Rigidbody2D>();
        planeCollider = GetComponent<Collider2D>();
        IsAlive = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Die();
    }

    // Physics update
    private void FixedUpdate()
    {
        if (!IsAlive)
        {
            return;
        }

        if (useSimpleControls)
        {
            SimpleControls();
        }
        else
        {
            AerodynamicControls();
        }
    }

    private void SimpleControls()
    {
        transform.Rotate(0, 0, -Input.GetAxis("Horizontal") * basicSpeed);
        Vector3 locVel = transform.InverseTransformDirection(planeRigidbody.velocity).normalized;
        locVel.x = basicSpeed;
        planeRigidbody.velocity = transform.TransformDirection(locVel);
    }

    private void AerodynamicControls()
    {
        Vector2 currentVelocity = planeRigidbody.velocity;
        float angleOfAttack = planeRigidbody.rotation;
        float ySize = Mathf.Pow(Mathf.Sin(angleOfAttack) * planeCollider.bounds.size.y, 2);
        float xSize = Mathf.Pow(Mathf.Cos(angleOfAttack) * planeCollider.bounds.size.x, 2);

        // Apply lift
        //Vector2 lift = new Vector2(0, xSize * liftCoefficient * currentVelocity.x * Mathf.Cos(angleOfAttack));
        Vector2 liftDirection = (new Vector2(Mathf.Abs(currentVelocity.y), Mathf.Abs(currentVelocity.x))).normalized;
        Vector2 lift = liftDirection * currentVelocity.magnitude * liftCoefficient;
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
        //print("gravity: " + gravity);

        // Apply torque
        float input = Input.GetAxis("Horizontal");
        if (input == 0)
        {
            planeRigidbody.angularVelocity = 0;
        }
        else
        {
            planeRigidbody.AddTorque(input);
        }
         

        // Apply corrective torque
        //float angleBetween = Mathf.Atan2(currentVelocity.y, currentVelocity.x) - angleOfAttack;
        //print("angle between: " + angleBetween);
        //float corrective = angleBetween * currentVelocity.magnitude * 2;
        //print("corrective: " + corrective);
        //planeRigidbody.AddTorque(corrective);

        //if (currentVelocity != Vector2.zero)
        //{
        //    float angle = Mathf.Atan2(currentVelocity.y, currentVelocity.x) * Mathf.Rad2Deg;
        //    transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        //}
    }

    private void Die()
    {
        IsAlive = false;
    }
}
