using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bat : MonoBehaviour
{
    public static bat instance;

    public Transform batTranform;  // Reference to the bat or hand object
    public float swingSpeed = 500f;  // How fast the bat swings
    public float swingAngle = 90f;  // Total swing angle
    public bool isSwinging = false;
    public float currentSwing = 0f;
    public Transform ResetTrans;
    public Rigidbody BallRB;
    public bool BallMove;


    public void Awake()
    {
        instance = this;
    }
    void Update()
    {

        if (BallRB.velocity.magnitude < 0.05f && BallMove)  // 0.01f is a small threshold to avoid floating point issues
        {
            ResetBat();
            Ballscript.instance.Reset();
            stumpscript.instance.Reset();
            BallMove = false;
        }

        // Detect button press (you can link this to a UI button if needed)
        if (Input.GetKeyDown(KeyCode.Space))  // Use UI button in real scenario
        {
            StartSwing();
        }
        if (Input.GetKeyDown(KeyCode.C))  // Use UI button in real scenario
        {
            if (BallRB.velocity.magnitude < 0.01)
                Ballscript.instance.Throw();
        }
        if (isSwinging)
        {
            SwingBat();
        }
    }

    public void ThrowBall()
    {
        if (BallRB.velocity.magnitude < 0.01)
            Ballscript.instance.Throw();
    }

    public void BatSwing()
    {
        StartSwing();
    }


    void StartSwing()
    {
        isSwinging = true;
        currentSwing = 0f;
    }
    void SwingBat()
    {
        Invoke("UseGravity", 2.5f);
        BallMove = true;
        // Rotate the bat to swing
        float swingStep = swingSpeed * Time.deltaTime;
        batTranform.Rotate(Vector3.right, swingStep);

        currentSwing += swingStep;

        // Stop swinging after reaching the swing angle
        if (currentSwing >= swingAngle)
        {
            isSwinging = false;
            ResetBat();
        }
    }

    public void UseGravity()
    {
        BallRB.GetComponent<Rigidbody>().useGravity = true;

    }

    public void ResetBat()
    {
        batTranform.position = ResetTrans.position;
        batTranform.localRotation = ResetTrans.localRotation;

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            // Add force to the ball (optional)
            Rigidbody ballRb = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 hitDirection = batTranform.forward + Vector3.up;  // Slightly upward
            ballRb.AddForce(hitDirection * 500f);
        }
    }
}
