using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalPoint : MonoBehaviour
{
    PlayerInput input;
    public enum PortalType { SameSence, DifferentSence }

    [Header("Transition Info")]
    public string sceneName;
    public PortalType portalType;
    public DestinationPoint.DestinationTag destinationTag;
    [SerializeField]
    private bool isMapPortal;
    private bool isTransition;
    [SerializeField]
    bool isDoor;

    bool canPortal;
    [SerializeField]
    private AudioData doorSfx;

    private void Start()
    {
        input = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if(canPortal && input.Interact &&!isMapPortal)
        {
            if (isDoor)
            {
                AudioManager.Instance.PlaySFX(doorSfx);
            }
            SceneController.Instance.TransitionToDestination(this);
        }
        else if(canPortal && isMapPortal && !isTransition)
        {
            SceneController.Instance.TransitionToDestination(this);
            isTransition = true;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canPortal = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canPortal = false;
        }
    }
}
