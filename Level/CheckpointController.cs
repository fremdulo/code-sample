using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
	public class CheckpointController : MonoBehaviour
	{
		[SerializeField]
		private WaypointController _waypoint;
		public WaypointController Waypoint { get { return _waypoint; } }

		private void OnTriggerEnter2D(Collider2D collision)
		{
			PlayerController player = collision.gameObject.GetComponent<PlayerController>();
			if (player != null && _waypoint != null)
			{
				TransitionManager.Instance.SaveCheckpoint(this);
			}
		}
	}
}
