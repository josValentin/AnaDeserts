using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Extensions;
using Firebase.Storage;
using System;
using UnityEngine.Networking;

public class FirebaseStorageLoader : MonoBehaviour
{
    private static FirebaseStorageLoader Instance;

    FirebaseStorage storage;
    StorageReference reference;

    void Start()
    {
        Instance = this;

        storage = FirebaseStorage.DefaultInstance;
        reference = storage.GetReferenceFromUrl("gs://ana-s-skull-truffles-project.appspot.com");
    }
    //
    public static void UploadData(Action OnComplete, Action OnError)
    {
        Debug.Log("here");
        Instance.StartCoroutine(Instance.UpLoadData(OnComplete, OnError));
    }

    public static void DownloadData(Action OnComplete, Action OnError)
    {
        //
        StorageReference fileData = Instance.reference.Child("SaveData.dat");

        fileData.GetDownloadUrlAsync().ContinueWithOnMainThread(task => {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                Instance.StartCoroutine(Instance.LoadData(Convert.ToString(task.Result), OnComplete, OnError)); //Fetch file from the link
            }
            else
            {
                Debug.Log(task.Exception);
            }

        });
    }

    private IEnumerator LoadData(string url, Action OnComplete, Action OnError)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if(request.result == UnityWebRequest.Result.Success)
            {
                byte[] data = request.downloadHandler.data;

                System.IO.File.WriteAllBytes(SaveSystem.SavedDataPath, data);

                OnComplete?.Invoke();
            }
            else
            {
                OnError?.Invoke();
            }
        }
    }

    private IEnumerator UpLoadData(Action OnComplete, Action OnError)
    {
        yield return null;

        byte[] bytes = System.IO.File.ReadAllBytes(SaveSystem.SavedDataPath);

        //Create a reference to where the file needs to be uploaded
        StorageReference uploadRef = Instance.reference.Child("SaveData.dat");
        Debug.Log("File upload started");

        uploadRef.PutBytesAsync(bytes).ContinueWithOnMainThread((task) => {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception.ToString());
                OnError?.Invoke();
            }
            else
            {
                Debug.Log("File Uploaded Successfully!");
                OnComplete?.Invoke();
            }
        });
    }
}
