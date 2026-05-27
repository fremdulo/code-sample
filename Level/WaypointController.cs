using UnityEngine;

namespace Platformer
{
	public class WaypointController : MonoBehaviour
	{
		[SerializeField]
		private LR _facing;
		public LR Facing { get { return _facing; } }

		[SerializeField]
		private string _name;
		public string Name { get { return _name; } }

		private int _id;
		public int Id { get { return _id; } }

		private void Reset()
		{
			_name = "checkpoint" + Random.Range(0, int.MaxValue);
		}

		private void Start()
		{
			_id = LevelController.GetWaypointId(_name);
			LevelController.Instance.AddWaypoint(this);
		}

	}
}
