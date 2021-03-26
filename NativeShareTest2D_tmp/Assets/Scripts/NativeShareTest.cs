using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NativeShareTest : MonoBehaviour {
    [SerializeField] private Camera _camA;

    [SerializeField] private Camera _camB;

    private bool _block = false;
    public void OnClickShare() {
        if (this._block)
            return;
        this._block = true;
        if (!NativeShareManager.Instance.IsSharing) {
            var coroutine = TakeScreenShotCombine(this._camA, this._camB, (combineTexture) => {
                    NativeShareManager.Instance.Share(combineTexture, true, () => {
                        this._block = false;
                        Destroy(combineTexture);
                    });
                });
            StartCoroutine(coroutine);
        }
    }
    
    private IEnumerator TakeScreenShotCombine(Camera camA, Camera camB, UnityAction<Texture2D> onComplete) {
        yield return new WaitForEndOfFrame();
        var texA = common.util.CameraRender.TakeScreenShot(camA, TextureFormat.RGB24, Screen.width, Screen.height);
        var texB = common.util.CameraRender.TakeScreenShot(camB, TextureFormat.RGBA32, Screen.width, Screen.height);
        
        for (int x = 0; x < texB.width; x++) {
            for (int y = 0; y < texB.height; y++) {
                var color = texB.GetPixel(x, y);
                var c = Color.Lerp(texA.GetPixel(x, y), color, color.a);
                texA.SetPixel(x, y, c);
            }
        }
        Destroy(texB);
        texA.Apply();
        onComplete(texA);
    }
}
