using UnityEngine;

namespace Platformer
{
	public class TransitionController : MonoBehaviour
	{
		[SerializeField]
		private BuildIndex _scene;

		[SerializeField]
		private string _waypointName;

		[SerializeField]
		private TransitionAsset _transitionAsset;

		public void ExecuteTransition()
		{
			int waypointId = GameUtil.HashString(_waypointName);
			TransitionInfo transition = new TransitionInfo((int)_scene, waypointId);
			TransitionManager.Instance.ExecuteTransition(transition, _transitionAsset);
		}
	}
}
