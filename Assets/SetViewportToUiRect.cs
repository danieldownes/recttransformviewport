using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Note ensure editor resolution matches Canvas fixed resolution (1024 x 768)
//

/// <summary>
/// The intention is to allow the viewport to be move around the screen as controlled by a RectTransform, which it does.
/// 
/// Howevever there is a stretching distortion effect when the viewport is set to go beyond the screen-bounds.
/// In which case the camera projection matrix is adjust to compensiate. This is almost perfect when the screen aspect ratio#
/// is set to 1:1, however there a problems on other resolutions which is prooving problomatic to recify.
/// 
/// There is an alternative method to use render textures and simply apply the render texture to a UI imaging, however this
/// render textures are very memory hungry, especially when considering multiple viewports are involved.
/// </summary>
public class SetViewportToUiRect : MonoBehaviour
{
    public RectTransform UiRect;

    public bool CameraPerspectiveAdjust = false;

    private Camera cam;

    public float aspect = 1;

    public float left = -0.2F;
    public float right = 0.2F;
    public float top = 0.2F;
    public float bottom = -0.2F;

    void Start()
    {
        cam = this.GetComponent<Camera>();
        
        cam.fieldOfView = (CameraPerspectiveAdjust ? 22.8f : 46.8f);
        
        DoFit();
    }

    void LateUpdate()
    {
        DoFit();
    }

    private void DoFit()
    {
        // Get RectTransform rect in screen-space
        Rect rectScreen = RectTransformToScreenSpace(UiRect);

        cam.rect = new Rect(rectScreen.x / Screen.width,
                            rectScreen.y / Screen.height,
                            rectScreen.width / Screen.width,
                            rectScreen.height / Screen.height);

        // Apply camera perspective off-setting when viewport going beyond the screen-bounds
        if (CameraPerspectiveAdjust)
        {
            float offsetBottom = (rectScreen.y) / Screen.height;
            if (offsetBottom > 0)
                offsetBottom = 0;
            float offsetTop = ((rectScreen.y + rectScreen.height) / Screen.height) - 1;
            if (offsetTop < 0)
                offsetTop = 0;
            float offsetLeft = (rectScreen.x / Screen.width);
            if (offsetLeft > 0)
                offsetLeft = 0;
            float offsetRight = ((rectScreen.x + rectScreen.width) / Screen.width) - 1;
            if (offsetRight < 0)
                offsetRight = 0;

            Matrix4x4 m = PerspectiveOffCenter(left - offsetLeft, right - offsetRight, bottom - offsetBottom, top - offsetTop, cam.nearClipPlane, cam.farClipPlane);
            cam.projectionMatrix = m;
        }
    }

    // Thanks: http://answers.unity3d.com/questions/1013011/convert-recttransform-rect-to-screen-space.html
    private static Rect RectTransformToScreenSpace(RectTransform transform)
    {
        Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
        Rect rect = new Rect(transform.position.x, transform.position.y, size.x, size.y);
        rect.x -= (transform.pivot.x * size.x);
        rect.y -= (transform.pivot.y * size.y);

        return rect;
    }

    // https://docs.unity3d.com/ScriptReference/Camera-projectionMatrix.html
    private static Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
    {
        float x = 2.0F * near / (right - left);
        float y = 2.0F * near / (top - bottom);
        float a = (right + left) / (right - left);
        float b = (top + bottom) / (top - bottom);
        float c = -(far + near) / (far - near);
        float d = -(2.0F * far * near) / (far - near);
        float e = -1.0F;

        Matrix4x4 m = new Matrix4x4();
        m[0, 0] = x;
        m[0, 1] = 0;
        m[0, 2] = a;
        m[0, 3] = 0;

        m[1, 0] = 0;
        m[1, 1] = y;
        m[1, 2] = b;
        m[1, 3] = 0;

        m[2, 0] = 0;
        m[2, 1] = 0;
        m[2, 2] = c;
        m[2, 3] = d;

        m[3, 0] = 0;
        m[3, 1] = 0;
        m[3, 2] = e;
        m[3, 3] = 0;
        return m;
    }
}
