using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class Panel_Main : MonoBehaviour
{

    private static Panel_Main Instance;

    [SerializeField] private Image imgCover;

    [SerializeField] private Button btnUploadBackup;

    [SerializeField] private Button btnDownloadBackup;

    [SerializeField] private TMPro.TextMeshProUGUI txtCurrentRequests;

    private float originalHeight;

    [SerializeField] private RectTransform requestsContainer;

    [SerializeField] private Button btnAgregarPedido;

    [SerializeField] private RequestButton requestButtonPrefab;
    float heightPrefab;

    private List<RequestButton> requestButtonList = new List<RequestButton>();

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        heightPrefab = requestButtonPrefab.GetComponent<RectTransform>().sizeDelta.y;

        foreach (var item in AppManager.RequestsDic)
        {
            AddRequestButton(item.Key, item.Value);
        }

        CheckCountAndUpdateText();

        btnAgregarPedido.onClick.AddListener(AgregarPedido_Click);

        Helpers.CoroutinesHelper.ActionAfterTime(this, 0.15f, () =>
        {
            imgCover.DOFade(0f, 0.65f).OnComplete(() => { imgCover.gameObject.SetActive(false); });

        });

        btnUploadBackup.onClick.AddListener(() => {

            Popup_Choose.Show("Crear Backup", "Estas a punto de crear y subir un backup de tus datos guardados a la nube, esto reemplazará el backup anterior.\n¿Deseas continuar?", AppManager.CreateDataBackupOnDrive);
        });
        btnDownloadBackup.onClick.AddListener(() => {

            Popup_Choose.Show("Cargar Backup", "Estas a punto de cargar el último backup creado, esto reemplazará tus datos guardados actuales.\n¿Deseas continuar?", AppManager.LoadLastBackupFromFrive);
        });
    }

    public static void ReloadScene()
    {
        Instance.imgCover.gameObject.SetActive(true);
        Instance.imgCover.DOFade(1f, 0.65f).OnComplete(() => {

            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        });

    }

    private void CheckCountAndUpdateText()
    {
        if (requestButtonList.Count > 1)
        {
            txtCurrentRequests.text = $"Tienes {requestButtonList.Count} pedidos en curso";
        }
        else if (requestButtonList.Count == 1)
        {
            txtCurrentRequests.text = $"Tienes 1 pedido en curso";

        }
        else
        {
            txtCurrentRequests.text = $"No tienes pedidos en curso";
        }
    }

    public static void AddRequestButton(string costumer, RequestData requestData)
    {
        RequestButton btnRequest = Instantiate(Instance.requestButtonPrefab, Instance.requestsContainer);

        btnRequest.Init(costumer, requestData);

        Instance.requestButtonList.Add(btnRequest);

        Instance.CheckCountAndUpdateText();

        Instance.requestsContainer.AddAnchorHeight(Instance.heightPrefab);
    }

    private void AgregarPedido_Click()
    {
        Panel_AddRequest.Show();
    }

    public static void ChangeButtonValue(string requestName, RequestData requestData)
    {
        for (int i = 0; i < Instance.requestButtonList.Count; i++)
        {
            if (Instance.requestButtonList[i].GetCostumerName() == requestName)
            {
                Instance.requestButtonList[i].UpdateButton(requestName, requestData);
                return;
            }
        }
    }

    public static void RemoveButton(string requestName)
    {
        for (int i = 0; i < Instance.requestButtonList.Count; i++)
        {
            if (Instance.requestButtonList[i].GetCostumerName() == requestName)
            {
                Destroy(Instance.requestButtonList[i].gameObject);
                Instance.requestButtonList.RemoveAt(i);

                Instance.requestsContainer.AddAnchorHeight(-Instance.heightPrefab);
                Debug.Log("ELIMINADO");
                Instance.CheckCountAndUpdateText();
                return;
            }
        }
    }
}
