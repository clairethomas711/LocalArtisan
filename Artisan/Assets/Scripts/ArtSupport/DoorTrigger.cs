using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] Animator doorAnimator;
    [SerializeField] string doorParameterName;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerStateManager>())
        {
            doorAnimator.SetBool(doorParameterName, true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerStateManager>())
        {
            doorAnimator.SetBool(doorParameterName, false);       
        }      
    }
}
