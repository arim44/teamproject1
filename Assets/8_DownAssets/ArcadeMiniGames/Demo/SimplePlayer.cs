using UnityEngine;
using System.Collections;

namespace ArcadeMiniGames
{
    /// <summary>
    /// For Demo purposes, not intended to be used as is, but you're free to reuse any of the code here
    /// </summary>
    public class SimplePlayer : MonoBehaviour
    {
        public static SimplePlayer Instance;

        public float speed = 5;

        private SimpleMouseLook cam;
        private new Rigidbody rigidbody;

        void Awake()
        {
            Instance = this;
            cam = GetComponentInChildren<SimpleMouseLook>();
            rigidbody = GetComponent<Rigidbody>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void FixedUpdate()
        {
            Vector3 forward = cam.transform.forward;
            forward.y = 0;
            forward.Normalize();
            Vector3 right = cam.transform.right;
            right.y = 0;
            forward.Normalize();

            var direction = right * Input.GetAxis("Horizontal") + forward * Input.GetAxis("Vertical");
            if (direction.sqrMagnitude > 1)
            {
                direction.Normalize();
            }

            rigidbody.linearVelocity = direction * speed;
        }

        private void OnDisable()
        {
            rigidbody.linearVelocity = Vector3.zero;
        }

        public void LockCamera()
        {
            cam.enabled = false;
        }
        public void UnlockCamera(Quaternion rotation)
        {
            cam.enabled = true; 
            cam.rotationY = rotation.eulerAngles.x;
        }
    }
}
