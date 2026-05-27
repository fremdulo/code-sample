using UnityEngine;

namespace Platformer
{
	[System.Serializable]
	public class SoundEffect
	{
		[SerializeField]
		private AudioClip[] _clips;

		[SerializeField]
		[Range(0f, 1f)]
		private float _volume = 1.0f;

		[SerializeField]
		private float _pitch = 1.0f;

		[SerializeField]
		private Vector2 _randomVolumeRange = new Vector2(1.0f, 1.0f);

		[SerializeField]
		private Vector2 _randomPitchRange = new Vector2(1.0f, 1.0f);

		[SerializeField]
		private bool _loop = false;

		private AudioSource _source;

		public void Initialize(Transform parent, string label)
		{
			GameObject go = new GameObject("SFX_" + label);
			go.transform.SetParent(parent);
			_source = go.AddComponent<AudioSource>();

			if (_clips.Length > 0)
			{
				int randomClip = Random.Range(0, _clips.Length - 1);
				_source.clip = _clips[randomClip];
			}
			_source.loop = _loop;
		}

		public void Play()
		{
			if (_clips.Length > 1)
			{
				int randomClip = Random.Range(0, _clips.Length - 1);
				_source.clip = _clips[randomClip];
			}
			_source.volume = _volume * Random.Range(_randomVolumeRange.x, _randomVolumeRange.y);
			_source.pitch = _pitch * Random.Range(_randomPitchRange.x, _randomPitchRange.y);
			_source.Play();
		}

		public void Stop()
		{
			_source.Stop();
		}

		public bool IsPlaying()
		{
			if (_source.isPlaying)
				return true;
			else
				return false;
		}
	}
}
