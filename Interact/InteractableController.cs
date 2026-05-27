using UnityEngine;
using UnityEngine.Events;

namespace Platformer
{
	public class InteractableController : MonoBehaviour, ITriggerEventListener
	{
		[SerializeField]
		private UnityEvent<GameObject> _action;

		[SerializeField]
		private InteractionType _interactionType;

		[SerializeField]
		private LayerMask _layers;

		public void Interact(InteractionType type, GameObject interacter)
		{
			if (interacter == null)
				return;

			if (type != _interactionType)
				return;

			if (_layers.value != 0 && ((1 << interacter.layer) & _layers.value) == 0)
				return;

			_action?.Invoke(interacter);
		}

		public void OnTriggerEnter2D(Collider2D collision)
		{
			Interact(InteractionType.Enter, collision.gameObject);
		}

		public void OnTriggerExit2D(Collider2D collision)
		{
			Interact(InteractionType.Exit, collision.gameObject);
		}
	}
}