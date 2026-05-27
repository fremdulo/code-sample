using System.Collections.Generic;
using UnityEngine;

namespace Platform
{
	public class SpriteGroup : MonoBehaviour
	{
		[SerializeField]
		private Color _color;
		public Color Color { get { return _color; } set { _color = value; } }

		[SerializeField]
		private bool _onlyAlpha;

		private Color _prevColor;
		private List<SpriteRenderer> _sprites = new List<SpriteRenderer>();

		private void Update()
		{
			if (_color != _prevColor)
			{
				_sprites.Clear();
				gameObject.GetComponentsInChildren(false, _sprites);

				foreach (SpriteRenderer sprite in _sprites)
				{
					if (_onlyAlpha)
					{
						Color c = sprite.color;
						c.a = _color.a;
						sprite.color = c;
					}
					else
					{
						sprite.color = _color;
					}
				}
			}
		}
	}
}