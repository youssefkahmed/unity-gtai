using UnityEngine;
using Unity.Cinemachine;

namespace GTAI.Player
{
	public partial class PlayerController
	{
		public bool IsAiming { get; private set; }

		[Header("Camera:")]
		[SerializeField] private CinemachineThirdPersonFollow cameraFollow;
		[SerializeField] private float cameraAimSmoothness = 5f;

		[Header("Camera Regular Mode:")]
		[SerializeField] private Vector3 camShoulderOffset = Vector3.zero;
		[SerializeField] private float camVerticalArmLength = 0.5f;
		[SerializeField] private float camDistance = 3.5f;

		[Header("Camera Aiming Mode:")]
		[SerializeField] private float camAimVerticalArmLength = 0.4f;
		[SerializeField] private float camAimDistance = 1.3f;
		[SerializeField] private Vector3 camAimShoulderOffset = new(0.75f, 0f, 0f);

		private void UpdateAiming()
		{
			if (IsAiming)
			{
				npc.OnLookAt(cameraFollow.transform.position + cameraFollow.transform.forward * 30f);
			}

			var targetShoulderOffset = IsAiming ? camAimShoulderOffset : camShoulderOffset;
			float targetCamDistance = IsAiming ? camAimDistance : camDistance;
			float targetVerticalArmLength = IsAiming ? camAimVerticalArmLength : camVerticalArmLength;

			float t = cameraAimSmoothness * Time.deltaTime;

			if (Mathf.Abs(cameraFollow.CameraDistance - targetCamDistance) <= 0.05f)
			{
				cameraFollow.ShoulderOffset = targetShoulderOffset;
				cameraFollow.CameraDistance = targetCamDistance;
				cameraFollow.VerticalArmLength = targetVerticalArmLength;
			}
			else
			{
				cameraFollow.ShoulderOffset = Vector3.Lerp(cameraFollow.ShoulderOffset, targetShoulderOffset, t);
				cameraFollow.CameraDistance = Mathf.Lerp(cameraFollow.CameraDistance, targetCamDistance, t);
				cameraFollow.VerticalArmLength = Mathf.Lerp(cameraFollow.VerticalArmLength, targetVerticalArmLength, t);
			}
		}
	}
}