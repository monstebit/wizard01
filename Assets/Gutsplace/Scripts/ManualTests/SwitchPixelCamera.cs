using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchPixelCamera : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Camera _pixelCamera;
    [SerializeField] private bool _isMainCameraActive = true;
    
    // [SerializeField] private RenderTexture _pixelRenderTexture;
    // [SerializeField] private bool _isPixelCameraRendering = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            _isMainCameraActive = !_isMainCameraActive;
            _mainCamera.gameObject.SetActive(_isMainCameraActive);
            _pixelCamera.gameObject.SetActive(!_isMainCameraActive);
        }
    }
    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.E))
    //     {
    //         _isPixelCameraRendering = !_isPixelCameraRendering;
    //         _pixelCamera.targetTexture = _isPixelCameraRendering ? _pixelRenderTexture : null;
    //     }
    // }
}
