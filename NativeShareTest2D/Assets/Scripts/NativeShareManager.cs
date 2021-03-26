using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class NativeShareManager : SingletonMonoBehaviour<NativeShareManager>
{
    private readonly string DirectoryName = "parallel";
    private bool _isSharing = false;
    public bool IsSharing => this._isSharing;

    private UnityAction _shareCompleteListener = null;
    public UnityAction ShareCompleteListener => this._shareCompleteListener;

    private string GetFileName() {
        var dt = DateTime.Now;
        return dt.ToString($"{dt:yyyyMMddHHmmss}") + ".png";
    }

    private string GetSavePath(string directoryName, string fileName) {
        string path = Path.Combine(Application.temporaryCachePath, directoryName);
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }
        path = Path.Combine(path, fileName);
        return path;
    }

    public void Share(Texture2D texture, bool isSaveGaralley = false, UnityAction onComplete = null) {
        if (this.IsSharing)
            return;
        StartCoroutine(this.ShareCore(texture, isSaveGaralley, onComplete));
    }
        
    public IEnumerator ShareCore(Texture2D texture, bool isSaveGaralley = false, UnityAction onComplete = null) {
        this._isSharing = true;
        yield return new WaitForEndOfFrame();

        string fileName = this.GetFileName();
        string filePath = this.GetSavePath(DirectoryName, fileName);
        
        //Pngに変換
        byte[] bytes = texture.EncodeToPNG();
        //保存
        File.WriteAllBytes(filePath, bytes);

        if (isSaveGaralley) {
            var permission = NativeGallery.SaveImageToGallery(texture, DirectoryName, fileName);
            Debug.Log("Permision result:" + permission);
        }
        
        new NativeShare().AddFile( filePath )
            .SetSubject( "Subject goes here" ).SetText( "" )
            .SetCallback( ( result, shareTarget ) =>
            {
                this._isSharing = false;
                Debug.Log("Share result: " + result + ", selected app: " + shareTarget);
                if (!ReferenceEquals(null, onComplete)) {
                    onComplete();
                }
            })
            .Share();
    }
}
