using UnityEngine;


namespace Platformer
{
	public class ProximityIntensity : MonoBehaviour
    {
        [SerializeField]
        [Min(0f)]
        private float _minDistance;

        [SerializeField]
        [Min(0.1f)]
        private float _maxDistance;

        [SerializeField]
        private float _minIntensity;

        [SerializeField]
        private float _maxIntensity;

        private UnityEngine.Rendering.Universal.Light2D _light;

		private void Start()
		{
            _light = GetComponent<UnityEngine.Rendering.Universal.Light2D>();
		}

		private void Update()
		{
            PlayerController player = LevelController.Instance.Player;
            if (player == null || _light == null)
                return;

            Vector2 playerPos = player.transform.position;
            Vector2 myPos = transform.position;

            float distance = Vector2.Distance(playerPos, myPos);
            float p = (Mathf.Clamp(distance, _minDistance, _maxDistance) - _minDistance) / (_maxDistance - _minDistance);

            float intensity = ((_maxIntensity - _minIntensity) * (1 - p)) + _minIntensity;
            _light.intensity = intensity;
		}
	}
}
