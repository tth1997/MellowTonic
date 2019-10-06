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

    void LateUpdate()
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
            
            Vector3 newMousePos;
            newMousePos.x = Mathf.Lerp(mouseTransform.position.x, playerCamera.WorldToScreenPoint(controller.MouseAimPos).x, 1f);
            newMousePos.y = Mathf.Lerp(mouseTransform.position.y, playerCamera.WorldToScreenPoint(controller.MouseAimPos).y, 1f);
            newMousePos.z = Mathf.Lerp(mouseTransform.position.z, playerCamera.WorldToScreenPoint(controller.MouseAimPos).z, 1f);

            mouseTransform.position = newMousePos;
            

            //mouseTransform.position = playerCamera.WorldToScreenPoint(controller.MouseAimPos);

            mouseTransform.gameObject.SetActive(mouseTransform.position.z > 1f);
        }
    }
}
