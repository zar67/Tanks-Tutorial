﻿using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float DampTime = 0.2f;                 // Approximate time for the camera to refocus.
    public float ScreenEdgeBuffer = 4f;           // Space between the top/bottom most target and the screen edge.
    public float MinSize = 6.5f;                  // The smallest orthographic size the camera can be.
    public Transform[] Targets;                   // All the targets the camera needs to encompass.

    private float m_zoomSpeed;                    // Reference speed for the smooth damping of the orthographic size.
    private Vector3 m_moveVelocity;               // Reference velocity for the smooth damping of the position.
    private Vector3 m_desiredPosition;            // The position the camera is moving towards.

    private void Awake()
    {
        SetStartPositionAndSize();
    }

    private void FixedUpdate()
    {
        // Move the camera towards a desired position.
        Move();

        // Change the size of the camera based.
        Zoom();
    }

    private void Move()
    {
        // Find the average position of the targets.
        FindAveragePosition();

        // Smoothly transition to that position.
        transform.position = Vector3.SmoothDamp(transform.position, m_desiredPosition, ref m_moveVelocity, DampTime);
    }

    private void FindAveragePosition()
    {
        var averagePos = new Vector3 ();
        int numTargets = 0;

        // Go through all the targets and add their positions together.
        for (int i = 0; i < Targets.Length; i++)
        {
            // If the target isn't active, go on to the next one.
            if (!Targets[i].gameObject.activeSelf)
            {
                continue;
            }

            // Add to the average and increment the number of targets in the average.
            averagePos += Targets[i].position;
            numTargets++;
        }

        // If there are targets divide the sum of the positions by the number of them to find the average.
        if (numTargets > 0)
        {
            averagePos /= numTargets;
        }

        // Keep the same y value.
        averagePos.y = transform.position.y;

        // The desired position is the average position;
        m_desiredPosition = averagePos;
    }

    private void Zoom()
    {
        // Find the required size based on the desired position and smoothly transition to that size.
        float requiredSize = FindRequiredSize();
        Camera.main.orthographicSize = Mathf.SmoothDamp (Camera.main.orthographicSize, requiredSize, ref m_zoomSpeed, DampTime);
    }

    private float FindRequiredSize()
    {
        // Find the position the camera rig is moving towards in its local space.
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_desiredPosition);

        // Start the camera's size calculation at zero.
        float size = 0f;

        // Go through all the targets...
        for (int i = 0; i < Targets.Length; i++)
        {
            // ... and if they aren't active continue on to the next target.
            if (!Targets[i].gameObject.activeSelf)
            {
                continue;
            }

            // Otherwise, find the position of the target in the camera's local space.
            Vector3 targetLocalPos = transform.InverseTransformPoint(Targets[i].position);

            // Find the position of the target from the desired position of the camera's local space.
            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

            // Choose the largest out of the current size and the distance of the tank 'up' or 'down' from the camera.
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));

            // Choose the largest out of the current size and the calculated size based on the tank being to the left or right of the camera.
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / Camera.main.aspect);
        }

        // Add the edge buffer to the size.
        size += ScreenEdgeBuffer;

        // Make sure the camera's size isn't below the minimum.
        size = Mathf.Max (size, MinSize);

        return size;
    }

    private void SetStartPositionAndSize()
    {
        // Find the desired position.
        FindAveragePosition();

        // Set the camera's position to the desired position without damping.
        transform.position = m_desiredPosition;

        // Find and set the required size of the camera.
        Camera.main.orthographicSize = FindRequiredSize();
    }
}