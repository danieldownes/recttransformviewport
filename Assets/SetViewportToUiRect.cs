using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class SetViewportToUiRect : MonoBehaviour
{
    public RectTransform UiRect;
    private Camera cam;

    public float aspectt = 1;

    public float left = -0.2F;
    public float right = 0.2F;
    public float top = 0.2F;
    public float bottom = -0.2F;

    void Start()
    {
        cam = this.GetComponent<Camera>();
        DoFit();
    }

    void LateUpdate()
    {
        DoFit();
    }

    private void DoFit()
    {
        cam.rect = RectTransformToScreenSpace(UiRect);

        //Rect offset = new Rect( )

        // Compensate ratio scaling when viewport off-screen
        float offsetBottom = (cam.rect.y / Screen.height);
        if (offsetBottom > 0)
            offsetBottom = 0;

        float offsetTop = ((cam.rect.y + cam.rect.height) / Screen.height) - 1;
        if (offsetTop < 0)
            offsetTop = 0;

        float offsetLeft = (cam.rect.x + cam.rect.width) / Screen.width;
        if (offsetLeft > 0)
            offsetLeft = 0;

        float offsetRight = ((cam.rect.x + cam.rect.width) / Screen.width) - 1;
        if (offsetRight < 0)
            offsetRight = 0;

        //TODO: When rect over half off screen, Perspective should start to follow centre of rect
        //    
        cam.rect = new Rect(cam.rect.x / Screen.width,
            cam.rect.y / Screen.height,
            cam.rect.width / Screen.width,
            cam.rect.height / Screen.height);
        
        Matrix4x4 m = PerspectiveOffCenter(left - offsetLeft, right - offsetRight, bottom - offsetBottom, top - offsetTop, cam.nearClipPlane, cam.farClipPlane);
        cam.projectionMatrix = m;
    }

    // Thanks: http://answers.unity3d.com/questions/1013011/convert-recttransform-rect-to-screen-space.html
    public static Rect RectTransformToScreenSpace(RectTransform transform)
    {
        Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
        Rect rect = new Rect(transform.position.x, transform.position.y, size.x, size.y);
        rect.x -= (transform.pivot.x * size.x);
        rect.y -= (transform.pivot.y * size.y);

        return rect;
    }

    // https://docs.unity3d.com/ScriptReference/Camera-projectionMatrix.html
    static Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
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
