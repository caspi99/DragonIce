using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugManager : MonoBehaviour
{
    [SerializeField]
    private List<Transform> cameras;

    private bool isInterfaceActive = false;

    [SerializeField]
    private GameObject cameraDebug;

    [SerializeField]
    private List<TextMeshProUGUI> camerasPositionsText;

    private int selectedCamera;

    [SerializeField]
    private Button camera1Button;

    [SerializeField]
    private Button camera2Button;

    // Start is called before the first frame update
    void Start()
    {
        selectedCamera = 0;
    }

    // Update is called once per frame
    void Update()
    {
        ToggleCameraDebugInterface();
        ChangeCamera();
        MoveCamera();
        UpdateInterfaceValues();
    }

    private void ToggleCameraDebugInterface()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            isInterfaceActive = !isInterfaceActive;
            cameraDebug.SetActive(isInterfaceActive);
        }
    }

    private void ChangeCamera()
    {
        camera1Button.onClick.AddListener(() => ChangeSelectedCameraValue(0));
        camera2Button.onClick.AddListener(() => ChangeSelectedCameraValue(1));
    }

    private void ChangeSelectedCameraValue(int value) { selectedCamera = value; }

    private void MoveCamera()
    {
        if (isInterfaceActive)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow)) { cameras[selectedCamera].position += new Vector3(-0.1f, 0.0f, 0.0f); }

            if (Input.GetKeyDown(KeyCode.RightArrow)) { cameras[selectedCamera].position += new Vector3(0.1f, 0.0f, 0.0f); }

            if (Input.GetKeyDown(KeyCode.UpArrow)) { cameras[selectedCamera].position += new Vector3(0.0f, 0.0f, 0.1f); }

            if (Input.GetKeyDown(KeyCode.DownArrow)) { cameras[selectedCamera].position += new Vector3(0.0f, 0.0f, -0.1f); }
        }
    }

    private void UpdateInterfaceValues()
    {
        for(int i = 0; i < camerasPositionsText.Count; i++)
        {
            camerasPositionsText[i].text = vector3ToString(cameras[i].position);
        }
    }


    //returns a string from a given vector3 (used for display in the interface)
    private string vector3ToString(Vector3 vector)
    {
        //show only 2 decimal points
        float x = Mathf.Round(vector.x * 100f) / 100f;
        float y = Mathf.Round(vector.y * 100f) / 100f;
        float z = Mathf.Round(vector.z * 100f) / 100f;

        return $"({x} , {y} , {z})";
    }
}
