using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEditor;
using AdvancedInputFieldPlugin;
using System;
using System.Text;
using Unity.Notifications.Android;

public class AppManager : MonoBehaviour
{
    private static AppManager Instance;
    public static Dictionary<string, RequestData> RequestsDic = new Dictionary<string, RequestData>();

    public static Dictionary<DessertKey, DessertData> TrufflesTasteDic = new Dictionary<DessertKey, DessertData>();

    [SerializeField]
    private Panel_Base[] panels;

    public static float KeyBoardHeight;

    [SerializeField] private Canvas mainCanvas;

    private void Start()
    {
        Instance = this;

        Application.targetFrameRate = 60;

        NativeKeyboardManager.AddKeyboardHeightChangedListener(OnKeyboardHeightChanged);
    }

    private void OnKeyboardHeightChanged(int keyBoard_ScreenHeight) => KeyBoardHeight = keyBoard_ScreenHeight / mainCanvas.scaleFactor;

    private void OnDestroy()
    {
        NativeKeyboardManager.RemoveKeyboardHeightChangedListener(OnKeyboardHeightChanged);
    }

    public static void SaveTaste(DessertKey key, DessertData trufflesData)
    {
        TrufflesTasteDic.Add(key, trufflesData);
        SaveAppData();
    }

    public static void OverwriteTaste(DessertKey key, DessertData trufflesData)
    {

        foreach (var savedKey in TrufflesTasteDic.Keys)
        {
            if (savedKey.Equals(key))
            {
                TrufflesTasteDic[savedKey] = trufflesData;
                SaveAppData();
                Debug.Log("SOBREESCRITO");
                return;
            }

        }
    }

    public static void DeleteTaste(DessertKey key)
    {
        foreach (var savedKey in TrufflesTasteDic.Keys)
        {
            if (savedKey.Equals(key))
            {
                TrufflesTasteDic.Remove(savedKey);
                SaveAppData();
                Debug.Log("ELIMINADO");
                return;
            }

        }

    }


    public static void SaveRequest(string name, RequestData requestData)
    {
        RequestsDic.Add(name, requestData);
        SaveAppData();
    }

    public static void OverwriteRequest(string name, RequestData requestData)
    {
        RequestsDic[name] = requestData;
        SaveAppData();
    }

    public static void FinishRequest(string name)
    {
        RequestsDic.Remove(name);
        SaveAppData();
    }


    public static void SaveAppData()
    {
        Helpers.CoroutinesHelper.ActionAfterReturnedNull(Instance, () =>
        {
            AppData data = new AppData(TrufflesTasteDic, RequestsDic);
            SaveSystem.SaveData(data);
        });

    }

    private void Awake()
    {
        LoadData();
    }

    public static void LoadData()
    {
        AppData data = SaveSystem.LoadData();
        if (data != null)
        {
            TrufflesTasteDic = data.TrufflesTasteDic_Data;
            RequestsDic = data.RequestsDic_Data;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Panel_Base panelShown = GetTheCurrentAnimPanelShown();

            if (panelShown != null)
                panelShown.Close();
        }
    }



    public Panel_Base GetTheCurrentAnimPanelShown()
    {
        List<Panel_Base> CurrentPanels = new List<Panel_Base>();
        for (int i = 0; i < panels.Length; i++)
        {
            if (panels[i].canvas.enabled)
            {
                CurrentPanels.Add(panels[i]);

            }
        }

        if (CurrentPanels.Count == 0)
            return null;

        Panel_Base currentPanelActive = CurrentPanels[CurrentPanels.Count - 1];

        CurrentPanels.Clear();

        return currentPanelActive;
    }


    public static void CreateDataBackupOnDrive()
    {
        LoadingBlock.ShowLoading("Creando Backup...");
        FirebaseStorageLoader.UploadData(() =>
        {
            LoadingBlock.ShowLoading("Finalizando...");
            Notification.Show("Backup creado Exitosamente");

            Helpers.CoroutinesHelper.ActionAfterTime(Instance, 0.7f, () =>
            {
                LoadingBlock.CloseLoading();
            });

        }, () =>
        {

            LoadingBlock.CloseLoading();
            Notification.Show("No se pudo crear el Backup");

        });

    }

    public static void LoadLastBackupFromFrive()
    {
        LoadingBlock.ShowLoading("Descargando Backup...");


        FirebaseStorageLoader.DownloadData(() =>
        {
            LoadingBlock.ShowLoading("Finalizando...");
            Notification.Show("Backup Cargado Exitosamente");

            Helpers.CoroutinesHelper.ActionAfterTime(Instance, 1f, () =>
            {
                LoadingBlock.CloseLoading(() =>
                {

                    AppData data = SaveSystem.LoadData();

#if UNITY_ANDROID && !UNITY_EDITOR
                    AndroidNotificationCenter.CancelAllNotifications();
                    AndroidNotificationCenter.CancelAllScheduledNotifications();

                    int index = 0;
                    foreach (var request in data.RequestsDic_Data)
                    {
                        AndroidNotificationChannel channel = new AndroidNotificationChannel();
                        channel.Id = request.Key + "_channel";
                        channel.Name = "Default Channel";
                        channel.Description = "For Generic Notifications";
                        channel.Importance = Importance.Default;

                        AndroidNotificationCenter.RegisterNotificationChannel(channel);

                        AndroidNotification notification = new AndroidNotification();




                        notification.Title = $"(Recordatorio) Pedido para {request.Key}";
                        notification.SmallIcon = "icon_app_small";

                        System.DateTime selectedTime = ConvertStrToDateTime(request.Value.date, request.Value.time);

                        if (selectedTime == System.DateTime.MinValue)
                            Debug.Log("Is Min Value");

                        if (System.DateTime.Now.Day == selectedTime.Day)
                        {
                            notification.FireTime = System.DateTime.Now.AddSeconds(1);
                            notification.Text = $"Este pedido se entrega hoy a las {request.Value.time}";

                        }
                        else
                        {
                            notification.FireTime = selectedTime.AddDays(-1);
                            notification.Text = $"Este pedido se entrega ma�ana a las {request.Value.time}";

                        }




                        AndroidNotificationCenter.SendNotificationWithExplicitID(notification, "default_channel", index);
                        index++;
                    }
#endif
                    Panel_Main.ReloadScene();
                });
            });
        }, () =>
        {
            LoadingBlock.CloseLoading();
            Notification.Show("No se pudo cargar el Backup");
        });

    }


    public static System.DateTime ConvertStrToDateTime(string date, string time)
    {
        string[] dateStrArray = date.Split('/');
        string[] timeStrArray = time.Split(':');

        if (dateStrArray.Length < 3 || dateStrArray[0] == "--")
            return System.DateTime.MinValue;

        if (timeStrArray.Length < 2 || timeStrArray[0] == "--")
            return System.DateTime.MinValue;

        int year = int.Parse(dateStrArray[0]);
        int month = int.Parse(dateStrArray[1]);
        int day = int.Parse(dateStrArray[2]);

        int hour = int.Parse(timeStrArray[0]);
        int minute = int.Parse(timeStrArray[1]);

        System.DateTime dateTime = new System.DateTime(year, month, day, hour, minute, 0);

        return dateTime;
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(AppManager))]
public class AppManagerEditor : Editor
{
    AppManager manager;

    private void OnEnable()
    {
        manager = (AppManager)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Delete Data"))
        {
            SaveSystem.DeleteData();
        }

        // Apply changes to the serializedProperty - always do this at the end of OnInspectorGUI.
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
