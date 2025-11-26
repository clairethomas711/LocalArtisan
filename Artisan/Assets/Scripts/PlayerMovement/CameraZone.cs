using UnityEngine;

public class CameraZone : MonoBehaviour
{
    public bool requiresInteriorTransition = false;
    [SerializeField] Animator transitionObject;

    public void OpenModelInterior()
    {
        transitionObject.SetBool("FacadeOpen", true);
    }

    public void CloseModelInterior()
    {
        transitionObject.SetBool("FacadeOpen", false);
    }
}
