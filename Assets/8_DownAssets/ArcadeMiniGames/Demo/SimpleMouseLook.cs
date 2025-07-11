using UnityEngine;
using System.Collections;

namespace ArcadeMiniGames
{
	/// <summary>
	/// For Demo purposes, not intended to be used as is, but you're free to reuse any of the code here.
	/// MouseLook rotates the transform based on the mouse delta.
	/// Minimum and Maximum values can be used to constrain the possible rotation
	/// </summary>
	public class SimpleMouseLook : MonoBehaviour
	{
		public float sensitivityX = 2;
		public float sensitivityY = 2;

		public float minimumY = -80;
		public float maximumY = 80;

		[HideInInspector]
		public float rotationY = 0F;

		void Update()
		{
			float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

			rotationY -= Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

			transform.localEulerAngles = new Vector3(rotationY, rotationX, 0);
		}
	}
}