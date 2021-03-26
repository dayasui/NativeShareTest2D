using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace common.util {
    public class CameraRender : MonoBehaviour {

        public static Texture2D TakeScreenShot(RenderTexture renderTexture, Camera camera, TextureFormat textureFormat, int width, int height) {
            // 今のフレームでのカメラのレンダリングを待つ
            //yield return new WaitForEndOfFrame();
            var renderTexture2 = new RenderTexture(Screen.width, Screen.height, 0);
            camera.targetTexture = renderTexture2;
            
            // カメラの描画をテクスチャーに書き込み
            camera.Render();
            // 現在のアクティブなRenderTextureをキャッシュ
            var cache = RenderTexture.active;
            // Pixel情報を読み込むためにアクティブに指定
            RenderTexture.active = renderTexture2;
            var texture = new Texture2D(width, height, textureFormat, false);
            // RenderTexture.activeから読み込み
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            // テクスチャの保存
            texture.Apply();
            
            RenderTexture.active = cache;
            camera.targetTexture = null;

            return texture;
        }
    }
}