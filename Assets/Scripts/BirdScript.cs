using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdScript : MonoBehaviour
{
    public GameObject birdPrefab;         // The bird prefab
    public Transform spawnPoint;          // The empty GameObject where the bird will spawn
    public float minSpawnTime = 3f;       // Minimum time interval to spawn a bird
    public float maxSpawnTime = 10f;      // Maximum time interval to spawn a bird
    public float flightDistance = 5f;     // Distance the bird will travel
    public float birdSpeed = 2f;          // Speed at which the bird moves forward
    public float maxTiltAngle = 20f;      // Maximum upward tilt angle

    void Start()
    {
        // Start the spawning coroutine
        spawnPoint = gameObject.transform;
        StartCoroutine(SpawnBird());
    }

    IEnumerator SpawnBird()
    {
        while (true)
        {
            // Wait for a random time between minSpawnTime and maxSpawnTime
            yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));

            // Instantiate the bird at the spawnPoint's position and rotation
            GameObject bird = Instantiate(birdPrefab, spawnPoint.position, spawnPoint.rotation);

            // Apply a random upward tilt to the bird
            float randomTilt = Random.Range(0f, maxTiltAngle);
            bird.transform.Rotate(Vector3.right, -randomTilt);  // Rotate around the X-axis (upward)

            // Start moving the bird
            StartCoroutine(MoveBird(bird, randomTilt));
        }
    }

    IEnumerator MoveBird(GameObject bird, float tiltAngle)
    {
        // Calculate the direction the bird should fly based on its tilt
        Vector3 moveDirection = Quaternion.Euler(-tiltAngle, 0, 0) * bird.transform.forward;

        float distanceTraveled = 0f;

        // Move the bird in the tilted direction until it reaches the flight distance
        while (distanceTraveled < flightDistance)
        {
            float step = birdSpeed * Time.deltaTime;
            bird.transform.position += moveDirection * step;
            distanceTraveled += step;

            yield return null;
        }

        // Destroy the bird once it has traveled the full distance
        Destroy(bird);
    }
}
