using UnityEngine;

public abstract class Panel_Base : MonoBehaviour
{
    [SerializeField] public Canvas canvas;

    public virtual void Close(System.Action OnClose = null)
    {

    }
}
