using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalCrackPosition : MonoBehaviour
{
    [SerializeField] private Transform parent;
    // Start is called before the first frame update
    void Start()
    {
        transform.SetParent(parent, false);
        transform.localScale = new Vector3(0.0f, 0.0f, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.localScale.x <20.0f && transform.localScale.y <20.0f)
        {
        transform.localScale += new Vector3(0.025f, 0.025f, 0.0f);
        }
    }
}
