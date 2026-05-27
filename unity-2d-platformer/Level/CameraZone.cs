using Cinemachine;
using UnityEngine;

namespace Platformer
{
	public class CameraZone : MonoBehaviour
	{
		[SerializeField]
		private CinemachineVirtualCamera _zoneCamera;
		public CinemachineVirtualCamera ZoneCamera { get { return _zoneCamera; } }
	}
}
