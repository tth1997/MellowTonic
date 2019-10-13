using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicControllerScript : MonoBehaviour
{
    [HideInInspector]
    public MouseController mouseController;
    [HideInInspector]
    public Camera playerCamera;

    public RectTransform forwardImgTransform;
    public RectTransform mouseAimImgTransform;

    void Awake()
    {
        mouseController = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<MouseController>();

        playerCamera = mouseController.GetComponentInChildren<Camera>();
    }

    void LateUpdate()
    {
        UpdateGraphics(mouseController);
    }

    private void UpdateGraphics(MouseController controller)
    {
        if (forwardImgTransform != null)
        {
            forwardImgTransform.position = playerCamera.WorldToScreenPoint(controller.ForwardAimPos);
            forwardImgTransform.gameObject.SetActive(forwardImgTransform.position.z > 1f);
        }

        if (mouseAimImgTransform != null)
        {
            mouseAimImgTransform.position = playerCamera.WorldToScreenPoint(controller.MouseAimPos);
            mouseAimImgTransform.gameObject.SetActive(mouseAimImgTransform.position.z > 1f);
        }
    }
}
