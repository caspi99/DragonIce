using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipStorySequences : MonoBehaviour
{
    [SerializeField]
    private GameObject Smoke;

    [SerializeField]
    private GameObject ScreenEffects;

    [SerializeField]
    private MapController map_controller;

    // Start is called before the first frame update
    void Start()
    {
        StopScreenEffects();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void StopScreenEffects()
    {
        /*
        if (CheckboxManagerLevel2.skip_story_sequences)
        {
            Smoke.SetActive(false);
            ScreenEffects.SetActive(false);

            map_controller.ChangeMapPosition(GameConstants.MAP_AFTER_EXPLOSION, GameConstants.map_positions[GameConstants.PHASE_1_TRANSFORM]);
            map_controller.ChangeMapScale(GameConstants.MAP_AFTER_EXPLOSION, GameConstants.map_scales[GameConstants.PHASE_1_TRANSFORM]);
        }
        */
    }
}
