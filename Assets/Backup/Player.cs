using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.IO;

namespace Backup
{
    public class Player : MonoBehaviour
    {
        private Rigidbody2D rb;
        private PhysicsMaterial2D pm;


        private int verticalAxis;
        private int horizontalAxis;
        private int rotation;
        private int movementSpeed;
        private int rotationSpeed;
        private bool charging;

        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            pm = rb.sharedMaterial;
            horizontalAxis = 0;
            verticalAxis = 0;
            rotation = 0;

            movementSpeed = 2;
            rotationSpeed = 100;

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow)) { verticalAxis += 1; }
            if (Input.GetKeyDown(KeyCode.LeftArrow)) { horizontalAxis -= 1; }
            if (Input.GetKeyDown(KeyCode.DownArrow)) { verticalAxis -= 1; }
            if (Input.GetKeyDown(KeyCode.RightArrow)) { horizontalAxis += 1; }
            if (Input.GetKeyDown(KeyCode.Q)) { rotation += 1; }
            if (Input.GetKeyDown(KeyCode.E)) { rotation -= 1; }

            if (Input.GetKeyUp(KeyCode.UpArrow)) { verticalAxis -= 1; }
            if (Input.GetKeyUp(KeyCode.LeftArrow)) { horizontalAxis += 1; }
            if (Input.GetKeyUp(KeyCode.DownArrow)) { verticalAxis += 1; }
            if (Input.GetKeyUp(KeyCode.RightArrow)) { horizontalAxis -= 1; }
            if (Input.GetKeyUp(KeyCode.Q)) { rotation -= 1; }
            if (Input.GetKeyUp(KeyCode.E)) { rotation += 1; }

            if (Input.GetKeyDown(KeyCode.Space)) { pm.bounciness = 0; charging = true; }
            if (Input.GetKeyUp(KeyCode.Space)) { charging = false; }

            if (Input.GetKeyDown(KeyCode.B))
            {
                if (pm.bounciness < State.maxPadBounciness)
                {
                    pm.bounciness = State.maxPadBounciness;
                }
                else
                {
                    pm.bounciness = 0;
                }
            }
        }

        private void FixedUpdate()
        {
            rb.velocity = movementSpeed * new Vector2(horizontalAxis, verticalAxis).normalized;
            rb.angularVelocity = rotationSpeed * rotation;
            if (charging && pm.bounciness < State.maxPadBounciness)
            {
                pm.bounciness += 0.02f;
            }
        }
    }
}