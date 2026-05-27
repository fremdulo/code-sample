using UnityEngine;

namespace Platformer
{
	public class SoundPlayer : MonoBehaviour
	{
		[SerializeField]
		private SoundEffect _sound;

		private void Start()
		{
			_sound.Initialize(transform, "SoundPlayerSound");
		}

		public void Play()
		{
			_sound?.Play();
		}
	}
}