using Unity.Cinemachine;
using UnityEngine;

public class CameraTargetSetter : MonoBehaviour
{
    private CinemachineCamera vcam;

    void Awake()
    {
        vcam = GetComponent<CinemachineCamera>();
    }

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            vcam.Follow = player.transform;
        }
    }
}