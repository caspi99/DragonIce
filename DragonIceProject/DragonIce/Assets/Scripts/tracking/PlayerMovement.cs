using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    public Quaternion q;
    public bool manual;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setPosition(Vector3 pos)
    {
        //swith playerIndex
        transform.position = pos;

        if (SceneManager.GetActiveScene().name == "Level2") { ClampPositionInsideEnvironment(); }
    }

    public void setRotation(Quaternion quat)
    {
        Matrix4x4 mat = Matrix4x4.Rotate(quat);
        Debug.Log(mat);
        transform.localRotation = quat;
    }

    private void ClampPositionInsideEnvironment()
    {
        Vector2 player_pos = new Vector2(this.transform.position.x, this.transform.position.z);
        Vector2 map_center = GameConstants.MAP_CENTER;
        float map_radius = GameConstants.MAP_RADIUS;

        if (Vector2.Distance(player_pos, map_center) > map_radius)
        {
            float theta = Mathf.Atan2(player_pos.y - map_center.y, player_pos.x - map_center.x);

            float cos = Mathf.Cos(theta);
            float sin = Mathf.Sin(theta);

            this.transform.position = new Vector3(map_center.x + map_radius * cos, 0.0f, map_center.y + map_radius * sin);
        }
    }
}
