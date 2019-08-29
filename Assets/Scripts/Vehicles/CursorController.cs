using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    public MouseController mouseCtrl;

    public RectTransform cursorTransform;
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
        if (cursorTransform != null)
        {
            cursorTransform.position = playerCamera.WorldToScreenPoint(controller.CursorPos);
            cursorTransform.gameObject.SetActive(cursorTransform.position.z > 1f);
        }

        if (mouseTransform != null)
        {
            mouseTransform.position = playerCamera.WorldToScreenPoint(controller.MouseAimPos);
            mouseTransform.gameObject.SetActive(mouseTransform.position.z > 1f);
        }
    }
}
