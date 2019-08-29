using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    public Transform playerCarTransform;
    public Transform mouseAimTransform;
    public Transform camRigTransform;
    public Transform cameraTransform;

    float mouseX;
    float mouseY;
    float mouseAimSmoothCoef = 2f;

    [SerializeField]
    float camSmoothSpeed = 3f;
    float mouseSens = 2f;
    float aimDistance = 500f;

    /// <summary>
    /// Get a point along the aircraft's boresight projected out to aimDistance meters.
    /// Useful for drawing a crosshair to aim fixed forward guns with, or to indicate what
    /// direction the aircraft is pointed.
    /// </summary>
    public Vector3 CursorPos
    {
        get
        {
            /// if 'playerCarTransform' is null, then '?' will return; otherwise, ':' will be return
            return playerCarTransform == null
                ? transform.forward * aimDistance
                : (playerCarTransform.forward * aimDistance) + playerCarTransform.position;
        }
    }

    /// <summary>
    /// Get the position that the mouse is indicating the aircraft should fly, projected
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

    private void Awake()
    {
        mouseX = 0;
        mouseY = 0;
    }

    void Update()
    {
        RotateRig();

        UpdateCameraPos();
    }

    private void RotateRig()
    {
        if (mouseAimTransform == null || cameraTransform == null || camRigTransform == null)
            return;

        // Mouse input. 
        mouseX += Input.GetAxis("Mouse X") * mouseSens;
        mouseY += -Input.GetAxis("Mouse Y") * mouseSens;

        // Rotate the aim target that the plane is meant to fly towards.
        // Use the camera's axes in world space so that mouse motion is intuitive.
        //mouseAimTransform.Rotate(cameraTransform.right, mouseY, Space.World);
        //mouseAimTransform.Rotate(cameraTransform.up, mouseX, Space.World);

        mouseAimTransform.eulerAngles = new Vector3(Mathf.LerpAngle(mouseAimTransform.eulerAngles.x, mouseY, mouseAimSmoothCoef), Mathf.LerpAngle(mouseAimTransform.eulerAngles.y, mouseX, mouseAimSmoothCoef), mouseAimTransform.eulerAngles.z);

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

    private void UpdateCameraPos()
    {
        if (playerCarTransform != null)
        {
            // Move the whole rig to follow the aircraft.
            transform.position = playerCarTransform.position;
        }
    }

    private Quaternion Damp(Quaternion a, Quaternion b, float lambda, float dt)
    {
        return Quaternion.Slerp(a, b, 1 - Mathf.Exp(-lambda * dt));
    }
}
