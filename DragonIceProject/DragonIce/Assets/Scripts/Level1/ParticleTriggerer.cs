using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParticleTriggerer : MonoBehaviour
{
    [SerializeField] private GameObject particle;
    [SerializeField] private Text speedText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //GenerateParticleTrial();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Particle.speed -= 1f;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Particle.speed += 1f;
        }

        speedText.text = "Speed: " + ((int)Particle.speed).ToString();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            //GenerateParticleTrial();
        }
    }

    /*
    private void GenerateParticleTrial()
    {
        Instantiate(particle, this.transform.position, Quaternion.identity);
        GameObject instantiated = Instantiate(particle, this.transform.position, Quaternion.identity);
        instantiated.GetComponent<Particle>().sign = -1f;
    }
    */
}
