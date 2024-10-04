using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera CMPlayerBack;
    [SerializeField] private CinemachineVirtualCamera CMPlayerFront;

    // This method switches the priority of the cameras
    public void SwitchToPlayerFrontCamera()
    {
        if (CMPlayerBack != null && CMPlayerFront != null)
        {
            CMPlayerBack.Priority = 0;
            CMPlayerFront.Priority = 10;
        }
    }
}
