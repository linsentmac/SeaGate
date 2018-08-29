using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
namespace LARSuite
{
public class LarPluginWin : LarPlugin
{
    private int slamInitCount = 0;
    public static LarPluginWin Create()
    {
        return new LarPluginWin ();
    }

    private LarPluginWin() { }

    public override bool IsInitialized() { return larCamera != null; }

    public override bool IsRunning() { return eyes != null; }

    public override bool IsSlamReady()
    {
        slamInitCount++;
        if(slamInitCount <= 100)
            return false;
        else
        {
            slamInitCount = 0;
            return true;
        }
    }

    public override IEnumerator Initialize()
    {
        yield return base.Initialize();

        deviceInfo = GetDeviceInfo();

        yield break;
    }

    public override IEnumerator BeginAR(int cpuPerfLevel, int gpuPerfLevel)
    {
        yield return base.BeginAR(cpuPerfLevel, gpuPerfLevel);

        yield break;
    }

    public override void SetVSyncCount(int vSyncCount)
    {
        QualitySettings.vSyncCount = vSyncCount;
    }

    public override void GetPredictedPose(ref Quaternion orientation, ref Vector3 position, int frameIndex)
    {
        orientation = Quaternion.identity;
        position = Vector3.zero;

        if (Input.GetMouseButton(1))    // 1/RIGHT mouse button
        {
            Vector2 mouseNDC = Vector2.zero;
            mouseNDC.x = 2 * (Input.mousePosition.x / Screen.width) - 1f;
            mouseNDC.y = 2 * (Input.mousePosition.y / Screen.height) - 1f;

            Vector3 eulers = Vector3.zero;
            eulers.y = mouseNDC.x * 90f;  // +/- degrees
            eulers.x = -mouseNDC.y * 45f;  // +/- degrees

            orientation.eulerAngles = eulers;
        }

        if (Input.GetMouseButton(2))    // 2/Middle mouse button
        {
            Vector2 mouseNDC = Vector2.zero;
            mouseNDC.x = 2 * (Input.mousePosition.x / Screen.width) - 1f;
            mouseNDC.y = 2 * (Input.mousePosition.y / Screen.height) - 1f;

            position.x = mouseNDC.x;
            position.z = mouseNDC.y;
        }
    }

    public override DeviceInfo GetDeviceInfo()
    {
        DeviceInfo info             = new DeviceInfo();

        info.displayWidthPixels     = Screen.width;
        info.displayHeightPixels    = Screen.height;
        info.displayRefreshRateHz   = 60.0f;
        info.targetFovXRad          = Mathf.Deg2Rad * 90;
        info.targetFovYRad          = Mathf.Deg2Rad * 90;

        return info;
    }

    public override void SubmitFrame(int frameIndex)
    {
        RenderTexture.active = null;
        GL.Clear (false, true, Color.black);

        float cameraFov = 0.4536f;
        float fovMarginX = (cameraFov / deviceInfo.targetFovXRad) - 1;
        float fovMarginY = (cameraFov / deviceInfo.targetFovYRad) - 1;
        Rect textureRect = new Rect(fovMarginX * 0.5f, fovMarginY * 0.5f, 1 - fovMarginX, 1 - fovMarginY);

        Vector2 leftCenter = new Vector2(Screen.width * 0.25f, Screen.height * 0.5f);
        Vector2 rightCenter = new Vector2(Screen.width * 0.75f, Screen.height * 0.5f);
        Vector2 eyeExtent = new Vector3(Screen.width * 0.25f, Screen.height * 0.5f);
        eyeExtent.x -= 10.0f;
        eyeExtent.y -= 10.0f;

        Rect leftScreen = Rect.MinMaxRect(
            leftCenter.x - eyeExtent.x,
            leftCenter.y - eyeExtent.y,
            leftCenter.x + eyeExtent.x,
            leftCenter.y + eyeExtent.y);
        Rect rightScreen = Rect.MinMaxRect(
            rightCenter.x - eyeExtent.x,
            rightCenter.y - eyeExtent.y,
            rightCenter.x + eyeExtent.x,
            rightCenter.y + eyeExtent.y);

        if (eyes != null) for (int i = 0; i < eyes.Length; i++)
        {
            if (eyes[i].isActiveAndEnabled == false) continue;
            if (eyes[i].TexturePtr == null) continue;
            if (eyes[i].imageTransform != null && eyes[i].imageTransform.gameObject.activeSelf == false) continue;
            if (eyes[i].imageTransform != null && !eyes[i].imageTransform.IsChildOf(larCamera.transform)) continue;   // lar only

            var eyeRectMin = eyes[i].clipLowerLeft; eyeRectMin /= eyeRectMin.w;
            var eyeRectMax = eyes[i].clipUpperRight; eyeRectMax /= eyeRectMax.w;
            if (eyes[i].Side == LarEye.eSide.LEFT || eyes[i].Side == LarEye.eSide.BOTH)
            {
                leftScreen = Rect.MinMaxRect(
                    leftCenter.x + eyeExtent.x * eyeRectMin.x,
                    leftCenter.y + eyeExtent.y * eyeRectMin.y,
                    leftCenter.x + eyeExtent.x * eyeRectMax.x,
                    leftCenter.y + eyeExtent.y * eyeRectMax.y);
                Graphics.DrawTexture(leftScreen, eyes[i].TexturePtr, textureRect, 0, 0, 0, 0);
            }
            if (eyes[i].Side == LarEye.eSide.RIGHT || eyes[i].Side == LarEye.eSide.BOTH)
            {
                rightScreen = Rect.MinMaxRect(
                    rightCenter.x + eyeExtent.x * eyeRectMin.x,
                    rightCenter.y + eyeExtent.y * eyeRectMin.y,
                    rightCenter.x + eyeExtent.x * eyeRectMax.x,
                    rightCenter.y + eyeExtent.y * eyeRectMax.y);
                Graphics.DrawTexture(rightScreen, eyes[i].TexturePtr, textureRect, 0, 0, 0, 0);
            }
        }

        if (overlays != null) for (int i = 0; i < overlays.Length; i++)
        {
            if (overlays[i].isActiveAndEnabled == false) continue;
            if (overlays[i].TexturePtr == null) continue;
            if (overlays[i].imageTransform != null && overlays[i].imageTransform.gameObject.activeSelf == false) continue;
            if (overlays[i].imageTransform != null && !overlays[i].imageTransform.IsChildOf(larCamera.transform)) continue;   // lar only

            var eyeRectMin = overlays[i].clipLowerLeft; eyeRectMin /= eyeRectMin.w;
            var eyeRectMax = overlays[i].clipUpperRight; eyeRectMax /= eyeRectMax.w;
            if (overlays[i].Side == LarOverlay.eSide.LEFT || overlays[i].Side == LarOverlay.eSide.BOTH)
            {
                leftScreen = Rect.MinMaxRect(
                    leftCenter.x + eyeExtent.x * eyeRectMin.x,
                    leftCenter.y + eyeExtent.y * eyeRectMin.y,
                    leftCenter.x + eyeExtent.x * eyeRectMax.x,
                    leftCenter.y + eyeExtent.y * eyeRectMax.y);
                Graphics.DrawTexture(leftScreen, overlays[i].TexturePtr);
            }
            if (overlays[i].Side == LarOverlay.eSide.RIGHT || overlays[i].Side == LarOverlay.eSide.BOTH)
            {
                rightScreen = Rect.MinMaxRect(
                    rightCenter.x + eyeExtent.x * eyeRectMin.x,
                    rightCenter.y + eyeExtent.y * eyeRectMin.y,
                    rightCenter.x + eyeExtent.x * eyeRectMax.x,
                    rightCenter.y + eyeExtent.y * eyeRectMax.y);
                Graphics.DrawTexture(rightScreen, overlays[i].TexturePtr);
            }
        }

    }

    public override void Shutdown()
    {
        base.Shutdown();
    }
}
}
