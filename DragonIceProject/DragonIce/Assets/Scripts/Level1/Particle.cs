using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    private TimeManager timeManager;
    private ParticleSystem particleSystem;

    public float sign = 1f;
    public Vector3 direction = new Vector3(1f,0f,0f);
    public Color particleColor;
    public Material particleMaterial;
    private bool particleMaterialChanged;
    public static float speed = 35f;
    private int timer;
    private float movingTime = 3f;
    private float destroyTime = 5f;

    private Vector2 particleRateRange = new Vector2(15, 2);
    private Vector2 particleSizeRange = new Vector2(1f, 0.1f);

    // Start is called before the first frame update
    void Start()
    {
        speed = CheckboxManager.particleSpeed;
        movingTime = CheckboxManager.particleMovingTime;
        destroyTime = CheckboxManager.particleDestroyTime;
        particleRateRange = CheckboxManager.particleRateRange;
        particleSizeRange = CheckboxManager.particleSizeRange;

        Destroy(this.gameObject, destroyTime);

        timeManager = GameObject.Find("TimeManager").GetComponent<TimeManager>();
        timer = timeManager.CreateTimer();

        particleSystem = this.GetComponent<ParticleSystem>();

        movingTime = movingTime * 10f / speed;
        destroyTime = destroyTime * 10f / speed;

        particleMaterialChanged = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!timeManager.WaitTime(timer, movingTime))
        {
            //<---ANGLE CALCULATION--->
            Vector3 start_vector = new Vector3(0.0f, 0.0f, -1.0f);

            // Calculate the angle between the start_vector and directionToCenter
            float angle = Vector3.Angle(start_vector, direction);

            // Calculate the rotation axis (perpendicular to start_vector and directionToCenter)
            Vector3 rotationAxis = Vector3.Cross(start_vector, direction);
            rotationAxis.Normalize(); // Normalize the rotation axis

            // You can now use this angle and rotationAxis to adjust your particle's movement.
            // For example, you can use Quaternion.AngleAxis to rotate your movement vector:
            float timeFactor = timeManager.GetTime(timer) / movingTime;
            Quaternion rotation = Quaternion.AngleAxis(angle, rotationAxis);
            Vector3 movementDirection = rotation * new Vector3(sign * speed * Mathf.Cos(timeFactor) * Time.deltaTime, 0f, speed * Mathf.Sin(timeFactor) * Time.deltaTime - 4.9f * (Mathf.Pow(Time.deltaTime, 2)));

            // Apply the movement direction
            this.transform.position += movementDirection;

            /*

            //<---ANGLE CALCULATION--->
            Vector3 start_vector = new Vector3(-1.0f, 0.0f, 0.0f);

            float dot_vector = Vector3.Dot(start_vector, direction);
            float cos_angle = dot_vector / Vector3.Magnitude(start_vector) * Vector3.Magnitude(direction);
            float angle = Mathf.Acos(cos_angle);

            float timeFactor = timeManager.GetTime(timer) / movingTime;
            this.transform.position += new Vector3(sign * speed * Mathf.Cos(timeFactor) * Time.deltaTime, 0f, speed * Mathf.Sin(timeFactor) * Time.deltaTime - 4.9f * (Mathf.Pow(Time.deltaTime, 2)));

            */

            var emission = particleSystem.emission;
            emission.rateOverDistance = GetCurrentRate();

            var main = particleSystem.main;
            main.startSize = GetCurrentSize();

            var colorOverLifeTime = particleSystem.colorOverLifetime;

            Gradient grad = new Gradient();
            grad.SetKeys(new GradientColorKey[] { new GradientColorKey(particleColor, 0.0f), new GradientColorKey(particleColor, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) });

            colorOverLifeTime.color = grad;

            if (CheckboxManager.brightParticles && !particleMaterialChanged)
            {
                var renderer = particleSystem.GetComponent<ParticleSystemRenderer>();
                renderer.material = particleMaterial;
                particleMaterialChanged = true;
            }
        }
    }

    private float GetCurrentRate()
    {
        float timeFactor = timeManager.GetTime(timer) / movingTime;

        return particleRateRange.x - timeFactor * (particleRateRange.x - particleRateRange.y);
    }

    private float GetCurrentSize()
    {
        float timeFactor = timeManager.GetTime(timer) / movingTime;

        return particleSizeRange.x - timeFactor * (particleSizeRange.x - particleSizeRange.y);
    }
}
