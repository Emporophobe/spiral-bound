using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane : MonoBehaviour {

    private Rigidbody2D planeRigidbody;
    private Collider2D planeCollider;
    public float liftCoefficient = 0.05f;
    public float dragCoefficient = 0.05f;
    public Vector2 initialVelocity = new Vector2(5, 0);
    public bool useSimpleControls = true;
    public float basicSpeed = 5;

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
        Vector2 currentVelocity = planeRigidbody.velocity;

        //planeRigidbody.angularVelocity = (Input.GetAxis("Horizontal") * 20);
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

        float angle = planeRigidbody.rotation;
        planeRigidbody.velocity = new Vector2(basicSpeed * Mathf.Cos(angle),
            basicSpeed * Mathf.Sin(angle));

        if (planeRigidbody.velocity != Vector2.zero)
        {
            //float angle = Mathf.Atan2(currentVelocity.y, currentVelocity.x) * Mathf.Rad2Deg;
            planeRigidbody.rotation = Mathf.Atan2(planeRigidbody.velocity.y, planeRigidbody.velocity.x);
            //transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        //if (currentVelocity != Vector2.zero)
        //{
        //    float angle = Mathf.Atan2(currentVelocity.y, currentVelocity.x) * Mathf.Rad2Deg;
        //    print(angle);
        //    planeRigidbody.velocity = new Vector2(basicSpeed * planeRigidbody.rotation * Mathf.Cos(angle),
        //                                          basicSpeed * planeRigidbody.rotation * Mathf.Sin(angle));
        //    print(planeRigidbody.velocity);
        //    planeRigidbody.velocity += new Vector2(0, -9.81f * Time.deltaTime * Mathf.Sin(angle));
        //    //transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        //    print(transform.rotation);
        //}
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
}
