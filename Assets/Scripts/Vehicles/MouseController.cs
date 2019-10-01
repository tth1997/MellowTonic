using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    public Vector3 rigOffset;
    public Transform playerCarTransform;
    public Transform mouseAimTransform;
    public Transform camRigTransform;
    public Transform cameraTransform;
    float angleLimit = 150f;

    public bool paused;
    public bool relativeControl;

    float mouseX;
    float mouseY;
    float mouseAimSmoothCoef = 10f;

    float snapLookAngleX;
    float snapLookAngleY;

    [SerializeField]
    float camSmoothSpeed = 3f;
    float mouseSens = 0.5f;
    float aimDistance = 200f;

    /// <summary>
    /// Get a point projected out to aimDistance meters along the forward direction of the player's car.
    /// </summary>
    public Vector3 ForwardAimPos
    {
        get
        {
            /// if 'playerCarTransform' is null, then '?' will return; otherwise, ':' will be returned
            return playerCarTransform == null
                ? transform.forward * aimDistance
                : (playerCarTransform.forward * aimDistance) + playerCarTransform.position;
        }
    }

    /// <summary>
    /// Get the position that the mouse is indicating the car should drive towards, projected
    /// out to aimDistance meters. Also meant to be used to draw a mouse cursor.
    /// </summary>
    public Vector3 MouseAimPos
    {
        get
        {
            return mouseAimTransform == null
                ? transform.forward * aimDistance
                : (mouseAimTransform.forward * aimDistance) + mouseAimTransform.position;
        }
    }

    /// <summary>
    /// The desired rotation angle of the mouseAimTransform (in degrees).
    /// </summary>
    public Vector2 LookAngle
    {
        get
        {
            mouseX += Input.GetAxis("Mouse X") * mouseSens;
            mouseY += -Input.GetAxis("Mouse Y") * mouseSens;

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                snapLookAngleX = mouseAimTransform.localEulerAngles.x;
                snapLookAngleY = mouseAimTransform.localEulerAngles.y;

            }
            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                mouseAimTransform.localEulerAngles = new Vector3(snapLookAngleX, snapLookAngleY, 0);
            }

            if (Mathf.DeltaAngle(transform.eulerAngles.y, mouseX) > angleLimit)
            {
                mouseX -= Mathf.DeltaAngle(transform.eulerAngles.y, mouseX) - angleLimit;
            }
            else if (Mathf.DeltaAngle(transform.eulerAngles.y, mouseX) < -angleLimit)
            {
                mouseX -= Mathf.DeltaAngle(transform.eulerAngles.y, mouseX) + angleLimit;
            }

            return new Vector2(mouseY, mouseX);
        }
    }

    private void Awake()
    {
        if (relativeControl)
        {
            mouseY = mouseAimTransform.localEulerAngles.x;
            mouseX = mouseAimTransform.localEulerAngles.y;
        }
        else
        {
            mouseY = mouseAimTransform.eulerAngles.x;
            mouseX = mouseAimTransform.eulerAngles.y;
        }

    }

    private void Start()
    {
        playerCarTransform = GameObject.FindGameObjectWithTag("Player").transform;
        if (!playerCarTransform)
            Debug.LogError("playerCarTransform is not specified. Make sure that the PlayerCar prefab is in the scene.");
        else
            transform.localPosition = rigOffset;
    }

    void Update()
    {
        if (!paused)
        {
            RotateRig();
        }
    }

    private void RotateRig()
    {
        if (mouseAimTransform == null || cameraTransform == null || camRigTransform == null)
            return;

        if (relativeControl)
        {
            mouseAimTransform.localEulerAngles = new Vector3(Mathf.LerpAngle(mouseAimTransform.localEulerAngles.x, LookAngle.x, mouseAimSmoothCoef),
                                                             Mathf.LerpAngle(mouseAimTransform.localEulerAngles.y, LookAngle.y, mouseAimSmoothCoef),
                                                             mouseAimTransform.localEulerAngles.z);
        }
        else
        
        {
            mouseAimTransform.eulerAngles = new Vector3(Mathf.LerpAngle(mouseAimTransform.eulerAngles.x, LookAngle.x, mouseAimSmoothCoef),
                                                        Mathf.LerpAngle(mouseAimTransform.eulerAngles.y, LookAngle.y, mouseAimSmoothCoef),
                                                        mouseAimTransform.eulerAngles.z);
            /*
            mouseAimTransform.eulerAngles = new Vector3(mouseAimTransform.eulerAngles.x,
                                                        Mathf.Clamp(mouseAimTransform.eulerAngles.y, transform.eulerAngles.y - angleLimit, transform.eulerAngles.y + angleLimit),
                                                        mouseAimTransform.eulerAngles.z);
            */
        }
    }

    private void FixedUpdate()
    {
        if (!paused)
        {
            RotateCamera();
        }
    }

    void RotateCamera()
    {
        // The up vector of the camera normally is aligned to the horizon. However, when
        // looking straight up/down this can feel a bit weird. At those extremes, the camera
        // stops aligning to the horizon and instead aligns to itself.
        Vector3 upVec = (Mathf.Abs(mouseAimTransform.forward.y) > 0.99f)
            ? camRigTransform.up
            : Vector3.up;

        // Smoothly rotate the camera to face the mouse aim.
        camRigTransform.rotation = Damp(camRigTransform.rotation,
                                        Quaternion.LookRotation(MouseAimPos - camRigTransform.position, upVec),
                                        camSmoothSpeed,
                                        Time.deltaTime);
    }

    private Quaternion Damp(Quaternion a, Quaternion b, float lambda, float dt)
    {
        return Quaternion.Slerp(a, b, 1 - Mathf.Exp(-lambda * dt));
    }
}
