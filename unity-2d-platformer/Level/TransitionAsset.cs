using UnityEngine;
namespace Platformer
{
	[CreateAssetMenu(fileName = "TransitionAsset", menuName = "Platformer/TransitionAsset", order = 1)]
	public class TransitionAsset : ScriptableObject
	{
		public string AnimTriggerIn;
		public string AnimTriggerOut;
	}
}