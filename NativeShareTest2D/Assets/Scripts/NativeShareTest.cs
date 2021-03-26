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
        var texture = common.util.CameraRender.RenderCombine(camA, camB, TextureFormat.RGB24, Screen.width, Screen.height);
        onComplete(texture);

        /*
        var renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
        //common.util.CameraRender.TakeScreenShot(renderTexture, camA, TextureFormat.RGB24, Screen.width, Screen.height);
        renderTexture = common.util.CameraRender.RenderCombine(renderTexture, camB, TextureFormat.RGBA32, Screen.width, Screen.height);
        
        
        var texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        // RenderTexture.activeから読み込み
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        // テクスチャの保存
        texture.Apply();
        
        for (int x = 0; x < texB.width; x++) {
            for (int y = 0; y < texB.height; y++) {
                var color = texB.GetPixel(x, y);
                var c = Color.Lerp(texA.GetPixel(x, y), color, color.a);
                texA.SetPixel(x, y, c);
            }
        }
        */

        //Destroy(texB);
        //texture.Apply();
        //onComplete(texture);
    }
}
