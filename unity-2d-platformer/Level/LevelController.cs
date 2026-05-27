using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
	public class LevelController : MonoBehaviour
	{
		private PlayerController _player;
		public PlayerController Player { get { return _player; } }

		private static LevelController _instance;
		public static LevelController Instance { get { return _instance; } }

		private Dictionary<int, WaypointController> _waypoints;
		public IReadOnlyDictionary<int, WaypointController> Waypoints { get { return _waypoints; } }

		private void Awake()
		{
			if (_instance != null)
			{
				GameController.Instance.DebugLog("Multiple instance of LevelController singleton detected!");
			}
			_instance = this;
			GameController.Instance.DebugLog("LevelController Awake");

			_player = FindObjectOfType<PlayerController>();
			Debug.Assert(_player != null);

			_waypoints = new Dictionary<int, WaypointController>();
		}

		private void OnDestroy()
		{
			_instance = null;
			GameController.Instance.DebugLog("LevelController OnDestroy");
		}

		public void AddWaypoint(WaypointController waypoint)
		{
			if (_waypoints.TryGetValue(waypoint.Id, out WaypointController checkWaypoint))
			{
				GameController.Instance.DebugLog(string.Format("Duplicate Checkpoint Id detected: {0}[{1}] and {2}[{3}]", waypoint.Name, waypoint.Id, checkWaypoint.Name, checkWaypoint.Id));
				return;
			}
			_waypoints.Add(waypoint.Id, waypoint);
		}

		public WaypointController FindWaypoint(int waypointId)
		{
			return _waypoints.TryGetValue(waypointId, out WaypointController wp) ? wp : null;
		}

		public static int GetWaypointId(string waypointName)
		{
			return GameUtil.HashString(waypointName);
		}

	}
}