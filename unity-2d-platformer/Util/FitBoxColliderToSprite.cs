using UnityEngine;

namespace Platformer
{
	public class FitBoxColliderToSprite : MonoBehaviour
	{
		[SerializeField]
		private bool _fitX = true;

		[SerializeField]
		private bool _fitY = true;

		private void Awake()
		{
			BoxCollider2D collider = GetComponent<BoxCollider2D>();
			SpriteRenderer sprite = GetComponent<SpriteRenderer>();

			Vector2 newSize;
			newSize.x = _fitX ? sprite.size.x : collider.size.x;
			newSize.y = _fitY ? sprite.size.y : collider.size.y;

			collider.size = newSize;
		}
	}
}