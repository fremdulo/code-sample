using UnityEngine;

namespace Platformer
{
	public class OnHitMessage : Message
	{
		public HitSource Source { get { return _sender as HitSource; } }
		public HitTarget Target { get; }

		public OnHitMessage(HitSource source, HitTarget target)
			: base(source)
		{
			Target = target;
		}
	}

	public class HitSource : MonoBehaviour
    {
		[SerializeField]
		private bool _canPogo;
		public bool CanPogo { get { return _canPogo; } }

		[SerializeField]
		private Dir4 _direction;
		public Dir4 Direction { get { return _direction; } set { _direction = value; } }

		[SerializeField]
		private Transform _hitEffectLocation;
		public Transform HitEffectLocation { get { return _hitEffectLocation; } }

		[SerializeField]
		private LayerMask _layers;

		private void OnTriggerEnter2D(Collider2D collision)
		{
			HitTarget target = collision.gameObject.GetComponent<HitTarget>();
			if (target == null)
				return;

			if ((_layers.value == 0) || ((1 << collision.gameObject.layer) & _layers.value) != 0)
			{
				GameController.Instance.MessageManager.SendMessage(new OnHitMessage(this, target));
			}

			target.Hit(this);
		}
	}
}