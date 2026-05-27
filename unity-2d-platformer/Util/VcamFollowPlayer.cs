using Cinemachine;
using UnityEngine;

namespace Platformer
{
	public class VcamFollowPlayer : MonoBehaviour
    {
        void Start()
        {
            PlayerController player = LevelController.Instance.Player;
            CinemachineVirtualCamera vcam = GetComponent<CinemachineVirtualCamera>();
            if (vcam != null && player != null)
			{
                vcam.Follow = player.CameraTarget.transform;
			}
        }
    }
}