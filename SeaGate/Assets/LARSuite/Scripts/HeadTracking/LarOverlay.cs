using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
namespace LARSuite
{
[RequireComponent(typeof(Camera))]
public class LarOverlay : MonoBehaviour, IComparable<LarOverlay>
{
    public enum eSide
    {
        LEFT = 1,
        RIGHT = 2,
        BOTH = 3,
        [HideInInspector]
        COUNT = BOTH
    };

    public enum eType
    {
        RENDER_TEXTURE = 0,
        STANDARD_TEXTURE = 1,
        EGL_TEXTURE = 2,
    };

    public delegate void OnPostRenderCallback();
    public OnPostRenderCallback OnPostRenderListener;
    public int layerDepth = 0;
    public eType imageType = eType.RENDER_TEXTURE;
    public Texture2D imageTexture;
    public Transform imageTransform;
    public Vector4 clipLowerLeft = new Vector4(-1, -1, 0, 1);
    public Vector4 clipUpperLeft = new Vector4(-1, 1, 0, 1);
    public Vector4 clipUpperRight = new Vector4(1, 1, 0, 1);
    public Vector4 clipLowerRight = new Vector4(1, -1, 0, 1);
    public float resolutionScaleFactor = 1.0f;

    private eSide side = eSide.BOTH;
    private RenderTextureFormat format = RenderTextureFormat.Default;
    private Vector2 resolution = new Vector2(1024.0f, 1024.0f);
    private int antiAliasing = 1;
    private int depth = 24;
    private const int bufferCount = 3;
    private RenderTexture[] overlayTextures = new RenderTexture[bufferCount];
    private int[] overlayTextureIds = new int[bufferCount];
    private int currentTextureIndex = 0;
    private Camera[] mainCameras = null;
    private bool dirty = false;
    private Coroutine recreateBuffersCoroutine = null;

    public int CompareTo(LarOverlay that)
    {
        return this.layerDepth.CompareTo(that.layerDepth);
    }

    public void SetImage(Texture2D texture)
    {
        imageTexture = texture;
        InitializeBuffers();
    }

    public eType ImageType
    {
        get { return imageType; }
        set { imageType = value; }
    }

    public eSide Side
    {
        get { return side; }
        set { side = value; }
    }

    public RenderTextureFormat Format
    {
        get { return format; }
        set { SetDirty(format != value); format = value; }
    }

    public int AntiAliasing
    {
        get { return antiAliasing; }
        set { SetDirty(antiAliasing != value); antiAliasing = value; }
    }

    public int Depth
    {
        get { return depth; }
        set { SetDirty(depth != value); depth = value; }
    }

    public Vector2 Resolution
    {
        get { return resolution; }
        set { SetDirty(!Mathf.Approximately(resolution.x, value.x) || !Mathf.Approximately(resolution.y, value.y)); resolution = value; }
    }

    public float ResolutionScaleFactor
    {
        get { return resolutionScaleFactor; }
        set { SetDirty(!Mathf.Approximately(resolutionScaleFactor, value)); resolutionScaleFactor = value; }
    }

    void SetDirty(bool value)
    {
        dirty = dirty == true ? true : value;
    }

    public int TextureId
    {
#if UNITY_5_6
        get { return overlayTextureIds[(currentTextureIndex + bufferCount - 1) % bufferCount]; } // Previous buffer
#else
        get { return overlayTextureIds[currentTextureIndex]; }
#endif
        set { overlayTextureIds[currentTextureIndex] = value; }
    }
    public Texture TexturePtr
    {
        get { return (imageTexture != null ? (Texture)imageTexture : overlayTextures[currentTextureIndex]); }
    }

    void Awake()
    {
        AcquireComponents();
        InitializeCoords();
    }

    void AcquireComponents()
    {
        mainCameras = GetComponentsInChildren<Camera>();
    }

    void Start()
    {
        //Initialize(); Called by LarManager.InitializeOverlays()
    }

    void LateUpdate()
    {
        UpdateCoords();
    }

    public void Initialize()
    {
        InitializeBuffers();
        InitializeCameras();
    }

    void InitializeBuffers()
    {
        for (int i = 0; i < bufferCount; ++i)
        {
            if (overlayTextures[i] != null)
                overlayTextures[i].Release();
            switch (imageType)
            {
                case eType.RENDER_TEXTURE:
                    overlayTextures[i] = new RenderTexture((int)(resolution.x * resolutionScaleFactor), (int)(resolution.y * resolutionScaleFactor), depth, format);
                    overlayTextures[i].antiAliasing = antiAliasing;
                    overlayTextures[i].Create();
                    overlayTextureIds[i] = overlayTextures[i].GetNativeTexturePtr().ToInt32();
                    break;

                case eType.STANDARD_TEXTURE:
                    if (imageTexture) overlayTextureIds[i] = imageTexture.GetNativeTexturePtr().ToInt32();
                    break;

                case eType.EGL_TEXTURE:
                    overlayTextureIds[i] = 0;
                    break;
            }
        }
    }

    void InitializeCameras()
    {
        var fovDeg = LarPlugin.Instance.deviceInfo.targetFovXRad * Mathf.Rad2Deg;
        foreach (var mainCamera in mainCameras)
        {
            mainCamera.fieldOfView = Mathf.Min(mainCamera.fieldOfView, fovDeg);
            mainCamera.aspect = resolution.x / resolution.y;
            //mainCamera.depth = (int)side;
        }
    }

    void InitializeCoords()
    {
        //clipLowerLeft.Set(-1, -1, 0, 1);
        //clipUpperLeft.Set(-1, 1, 0, 1);
        //clipUpperRight.Set(1, 1, 0, 1);
        //clipLowerRight.Set(1, -1, 0, 1);
    }

    void UpdateCoords()
    {
        if (imageTransform == null)
            return;

        var viewCamera = mainCameras[0];
        if (viewCamera == null)
            return;

        var extents = 0.5f * Vector3.one;
        var center = Vector3.zero;

        var worldLowerLeft = new Vector4(center.x - extents.x, center.y - extents.y, 0, 1);
        var worldUpperLeft = new Vector4(center.x - extents.x, center.y + extents.y, 0, 1);
        var worldUpperRight = new Vector4(center.x + extents.x, center.y + extents.y, 0, 1);
        var worldLowerRight = new Vector4(center.x + extents.x, center.y - extents.y, 0, 1);

        Matrix4x4 MVP = viewCamera.projectionMatrix * viewCamera.worldToCameraMatrix * imageTransform.localToWorldMatrix;

        clipLowerLeft = MVP * worldLowerLeft;
        clipUpperLeft = MVP * worldUpperLeft;
        clipUpperRight = MVP * worldUpperRight;
        clipLowerRight = MVP * worldLowerRight;
    }

    void OnPreRender()
    {
        SwapBuffers();
    }

    void SwapBuffers()
    {
        currentTextureIndex = ++currentTextureIndex % bufferCount;
        var targetTexture = overlayTextures[currentTextureIndex];
        if (targetTexture == null) return;

        for (int i = 0; i < mainCameras.Length; i++)
        {
            mainCameras[i].targetTexture = targetTexture;
        }
        targetTexture.DiscardContents();
    }

    void OnPostRender()
    {
        RecreateBuffersIfDirty();
        if (OnPostRenderListener != null)
        {
            OnPostRenderListener();
        }
    }

    void RecreateBuffersIfDirty()
    {
        if (dirty)
        {
            if (recreateBuffersCoroutine != null)
            {
                StopCoroutine(recreateBuffersCoroutine);
                recreateBuffersCoroutine = null;
            }

            recreateBuffersCoroutine = StartCoroutine(RecreateBuffersDeferred());
            dirty = false;
        }
    }

    IEnumerator RecreateBuffersDeferred()
    {
        int i = 0;
        while (i < bufferCount)
        {
            int index = currentTextureIndex - 1;
            index = index >= 0 ? index : bufferCount - 1;

            switch (imageType)
            {
                case eType.RENDER_TEXTURE:
                    overlayTextures[index] = new RenderTexture((int)(resolution.x * resolutionScaleFactor), (int)(resolution.y * resolutionScaleFactor), depth, format);
                    overlayTextures[index].antiAliasing = antiAliasing;
                    overlayTextures[index].Create();
                    overlayTextureIds[index] = overlayTextures[index].GetNativeTexturePtr().ToInt32();
                    Debug.Log("Recreated Render Texture with ID: " + overlayTextureIds[index] + " Width: " + overlayTextures[index].width + " Height: " + overlayTextures[index].height + "AA: " + overlayTextures[index].antiAliasing);
                    break;

                case eType.STANDARD_TEXTURE:
                    if (imageTexture) overlayTextureIds[index] = imageTexture.GetNativeTexturePtr().ToInt32();
                    break;

                case eType.EGL_TEXTURE:
                    overlayTextureIds[index] = 0;
                    break;
            }

            int prevTextureIndex = currentTextureIndex;
            yield return new WaitUntil(() => currentTextureIndex != prevTextureIndex);

            i++;
        }

        yield break;
    }

}
}
