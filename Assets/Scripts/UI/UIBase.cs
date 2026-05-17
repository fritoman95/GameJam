using UnityEngine;

public class UIBase : MonoBehaviour
{
    [SerializeField]
    protected GameObject _uiParentObject;

    public virtual void Initialize()
    {
    }

    public virtual void EnablePanel()
    {
        gameObject.SetActive(true);
    }

    public virtual void DisablePanel()
    {
        gameObject.SetActive(false);
    }
}