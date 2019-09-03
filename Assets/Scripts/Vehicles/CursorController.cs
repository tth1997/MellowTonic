using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    public MouseController mouseCtrl;

    public RectTransform forwardAimTransform;
    public RectTransform mouseTransform;

    public Camera playerCamera;

    void Awake()
    {
        playerCamera = mouseCtrl.GetComponentInChildren<Camera>();
    }

    void Update()
    {
        UpdateGraphics(mouseCtrl);
    }

    private void UpdateGraphics(MouseController controller)
    {
        if (forwardAimTransform != null)
        {
            forwardAimTransform.position = playerCamera.WorldToScreenPoint(controller.ForwardAimPos);
            forwardAimTransform.gameObject.SetActive(forwardAimTransform.position.z > 1f);
        }

        if (mouseTransform != null)
        {
            mouseTransform.position = playerCamera.WorldToScreenPoint(controller.MouseAimPos);
            mouseTransform.gameObject.SetActive(mouseTransform.position.z > 1f);
        }
    }
}
