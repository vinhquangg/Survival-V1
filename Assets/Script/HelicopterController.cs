using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterController : MonoBehaviour
{

    public float maxSpeed;
    public float processSpeed = 0f;
    public float minSpeed;
    public Transform rotor;
    public Transform backRotor;
    public Transform helicopterBody;
    public float liftSpeed;

    public float groundCheckDistance;
    public LayerMask groundMask;
    public float moveSpeed = 5f;


    private bool isGrounded = false;
    public float tiltAngle;
    public float tiltSpeed;
    private float currentTilt = 0f;
    private float moveInput = 0f;
    private float targetPitch = 0f;
    private float currentPitch = 0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        rotor.Rotate(Vector3.up * processSpeed * Time.deltaTime);
        UpdateRotorSpeed();
        UpdateVerticalMovement();
        BackRotor();
        Tilt();
        UpdateMoveDirection();
    }

    void HandleInput()
    {
        if (Input.GetKey(KeyCode.UpArrow))
            moveInput = 1f;
        else if (Input.GetKey(KeyCode.DownArrow))
            moveInput = -1f;
        else
            moveInput = 0f;
    }

    bool IsAboveGround(float heigh)
    {
        RaycastHit hit;
        if (Physics.Raycast(helicopterBody.position, Vector3.down, out hit, heigh, groundMask))
        {
            float distanceToGround = hit.distance;
            return distanceToGround > heigh;
        }
        return true;
    }

    void UpdateVerticalMovement()
    {
        if (moveInput > 0f && processSpeed >= maxSpeed)
        {
            Vector3 newPos = helicopterBody.position;
            newPos.y = Mathf.MoveTowards(newPos.y, newPos.y + 1f, liftSpeed * Time.deltaTime);
            helicopterBody.position = newPos;
            isGrounded = false;
        }
        else if (moveInput < 0f && !isGrounded && processSpeed <= maxSpeed * 0.9f && IsAboveGround(0f))
        {
            Vector3 newPos = helicopterBody.position;
            newPos.y = Mathf.MoveTowards(newPos.y, newPos.y - 1f, liftSpeed * Time.deltaTime);
            helicopterBody.position = newPos;
        }
    }

    void UpdateRotorSpeed()
    {
        if (moveInput > 0f)
        {
            processSpeed += 10f;
            processSpeed = Mathf.Clamp(processSpeed, minSpeed, maxSpeed);
        }
        else if (moveInput < 0f)
        {
            processSpeed -= 5f;
            processSpeed = Mathf.Clamp(processSpeed, minSpeed, maxSpeed);
        }
    }

    public void BackRotor()
    {
        if (processSpeed >= maxSpeed * 0.2f)
        {
            backRotor.Rotate(Vector3.right * processSpeed * Time.deltaTime);
        }
    }
    public void Tilt()
    {
        if (processSpeed >= maxSpeed * 0.8f && !isGrounded)
        {
            float tiltInput = 0f;
            float yawInput = 0f;
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                tiltInput = 1f;
                yawInput = -1f;
            }

            else if (Input.GetKey(KeyCode.RightArrow))
            {
                tiltInput = -1f;
                yawInput = 1f;
            }

            if (tiltInput != 0f)
            {
                currentTilt = Mathf.Clamp(currentTilt + tiltInput * tiltSpeed * Time.deltaTime, -tiltAngle, tiltAngle);
            }
            else
            {
                currentTilt = Mathf.Lerp(currentTilt, 0f, Time.deltaTime * 2f);
            }


            if (yawInput != 0f)
            {
                helicopterBody.Rotate(Vector3.up * yawInput * tiltSpeed * Time.deltaTime);
            }

            currentPitch = Mathf.Lerp(currentPitch, targetPitch, Time.deltaTime * 3f);

            Quaternion targetRotation = Quaternion.Euler(currentPitch, helicopterBody.rotation.eulerAngles.y, currentTilt);
            helicopterBody.rotation = Quaternion.Lerp(helicopterBody.rotation, targetRotation, Time.deltaTime * 5f);
        }
        else
        {

            currentTilt = Mathf.Lerp(currentTilt, 0f, Time.deltaTime * 2f);
            currentPitch = Mathf.Lerp(currentPitch, 0f, Time.deltaTime * 3f);

            Quaternion targetRotation = Quaternion.Euler(currentPitch, helicopterBody.rotation.eulerAngles.y, currentTilt);
            helicopterBody.rotation = Quaternion.Lerp(helicopterBody.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    void UpdateMoveDirection()
    {
        if (!isGrounded && processSpeed >= maxSpeed * 0.8f && moveInput != 0f && IsAboveGround(10f))
        {
            Vector3 direction = helicopterBody.forward * moveInput;
            direction.y = 0f;
            helicopterBody.position += direction.normalized * moveSpeed * Time.deltaTime;

            if (moveInput > 0f)
            {
                targetPitch = 30f;
                helicopterBody.position += helicopterBody.forward * (moveSpeed * 0.1f) * Time.deltaTime;
            }
            else if (moveInput < 0f)
            {

                targetPitch = -30f;
                helicopterBody.position -= helicopterBody.forward * (moveSpeed * 0.1f) * Time.deltaTime;
            }
        }
        else
        {
            targetPitch = 0f;
        }
    }

    

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;

            if (processSpeed > minSpeed)
            {
                Debug.Log("Landed. Still spinning...");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            Debug.Log("Left ground. Helicopter can now fly.");
        }
    }

}
