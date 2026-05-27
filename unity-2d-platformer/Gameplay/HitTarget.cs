using UnityEngine;

namespace Platformer
{
	public class HitTarget : MonoBehaviour
    {
		[SerializeField]
		private bool _canPogo;
		public bool CanPogo { get { return _canPogo; } }

		// TODO - remove this?
		[SerializeField]
		private Dir4 _hitDirection;

		[SerializeField]
		private GameObject _hitEffect;

		[SerializeField]
		private bool _hitEffectRandomRotate;

		[SerializeField]
		private Vector2 _pushBackDistance = Vector2.zero;
		public Vector2 PushBackDistance { get { return _pushBackDistance; } }

		[SerializeField]
		private float _pushBackTime;
		public float PushBackTime { get { return _pushBackTime; } }

		[SerializeField]
		private SoundEffect _sound;

		private void Start()
		{
			_sound.Initialize(transform, "HitTargetSound");
		}

		public void Hit(HitSource hitSource)
		{
			if (_hitDirection != Dir4.None && _hitDirection != hitSource.Direction)
				return;

			if (hitSource != null)
			{
				Vector3 spawnPos = hitSource.HitEffectLocation.position;
				SpawnEffect(spawnPos);
				_sound?.Play();
			}
		}

		private void SpawnEffect(Vector3 spawnPos)
		{
			if (_hitEffect != null)
			{
				GameObject newObj = Instantiate(_hitEffect, spawnPos, Quaternion.identity);

				if (_hitEffectRandomRotate)
				{
					float degrees = Random.Range(0f, 359.9f);
					newObj.transform.Rotate(new Vector3(0, 0, 1), degrees);
				}
			}
		}

		private static Vector2 FindClosestPerimeterPoint(Bounds bounds, Vector2 source)
		{
			Vector2 n = new Vector2(source.x, bounds.center.y - bounds.extents.y);
			Vector2 s = new Vector2(source.x, bounds.center.y + bounds.extents.y);
			Vector2 e = new Vector2(bounds.center.x - bounds.extents.x, source.y);
			Vector2 w = new Vector2(bounds.center.x + bounds.extents.x, source.y);

			Vector2 nDiff = n - source;
			Vector2 sDiff = s - source;
			Vector2 eDiff = e - source;
			Vector2 wDiff = w - source;

			float nMag = Mathf.Abs(nDiff.magnitude);
			float sMag = Mathf.Abs(sDiff.magnitude);
			float eMag = Mathf.Abs(eDiff.magnitude);
			float wMag = Mathf.Abs(wDiff.magnitude);

			Vector2 result = n;
			float mag = nMag;
			if (sMag < mag)
			{
				result = s;
				mag = sMag;
			}
			if (eMag < mag)
			{
				result = e;
				mag = eMag;
			}
			if (wMag < mag)
			{
				result = w;
			}

			return result;
		}
	}
}