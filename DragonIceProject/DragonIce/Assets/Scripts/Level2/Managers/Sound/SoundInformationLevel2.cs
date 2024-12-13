using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundInformationLevel2 : MonoBehaviour, SoundInformation
{
    //<--------------------INTERACTION INDEXES---------------------->
    public static SoundInfo GRAB_SOUND = new SoundInfo(0, 0.499f, INTERACTION);
    public static SoundInfo POP_SOUND = new SoundInfo(1, 0.499f, INTERACTION);
    public static SoundInfo DROP_SOUND = new SoundInfo(2, 0.499f, INTERACTION);
    public static SoundInfo WOOD_PLANK_SOUND = new SoundInfo(3, 0.499f, INTERACTION);
    public static SoundInfo SUCCESS = new SoundInfo(4, 0.499f, INTERACTION);
    public static SoundInfo STONE_DRAGGING = new SoundInfo(5, 0.499f, INTERACTION);
    public static SoundInfo MOVING_LOG = new SoundInfo(6, 0.499f, INTERACTION);
    public static SoundInfo END_SECOND_LEVEL = new SoundInfo(7, 0.499f, INTERACTION);

    //<--------------------AMBIENCE INDEXES---------------------->
    public static SoundInfo FOREST_1_SOUND = new SoundInfo(0, 0.58f, AMBIENCE);
    public static SoundInfo FOREST_2_SOUND = new SoundInfo(1, 0.58f, AMBIENCE);

    //<--------------------VOLCANO INDEXES---------------------->
    public static SoundInfo VOLCANO_SOUND = new SoundInfo(0, 0.849f, VOLCANO);
    public static SoundInfo WOOD_DESTRUCTION_1_SOUND = new SoundInfo(1, 0.849f, VOLCANO);
    public static SoundInfo WOOD_DESTRUCTION_2_SOUND = new SoundInfo(2, 0.849f, VOLCANO);
    public static SoundInfo VOLCANO_2_SOUND = new SoundInfo(3, 0.849f, VOLCANO);

    //<--------------------CAR INDEXES---------------------->
    public static SoundInfo CAR_CLAXON_SOUND = new SoundInfo(0, 0.187f, CAR);
    public static SoundInfo CAR_WAITING = new SoundInfo(1, 0.187f, CAR);
    public static SoundInfo JEEP_IN_MOUNTAIN_1_SOUND = new SoundInfo(2, 0.187f, CAR);
    public static SoundInfo JEEP_IN_MOUNTAIN_2_SOUND = new SoundInfo(3, 0.187f, CAR);
    public static SoundInfo CAR_STOPS = new SoundInfo(4, 0.187f, CAR);
    public static SoundInfo CAR_TURNS_ON = new SoundInfo(5, 0.187f, CAR);

    //<--------------------RIVER INDEXES--------------------->

    public static SoundInfo RIVER_SOUND = new SoundInfo(0, 0.099f, RIVER);

    //<--------------------DRAGON INDEXES--------------------->

    public static SoundInfo DRAGON_GROWL = new SoundInfo(0, 0.5f, DRAGON);
    public static SoundInfo FLY = new SoundInfo(1, 0.5f, DRAGON);
    public static SoundInfo DRAGON_LOOP_END = new SoundInfo(2, 0.5f, DRAGON);

    //<--------------------HUMAN INDEXES--------------------->

    public static SoundInfo HELPING_CHARACTER = new SoundInfo(0, 0.674f, HUMAN);
    public static SoundInfo CELEBRATION_CHARACTERS = new SoundInfo(1, 0.674f, HUMAN);

    //<--------------------SQUIRREL INDEXES--------------------->

    public static SoundInfo SQUIRREL_WALKING = new SoundInfo(0, 0.708f, SQUIRREL);

    //<--------------------NARRATIVE INDEXES--------------------->

    public static SoundInfo NARRATIVE_1 = new SoundInfo(0, 0.795f, NARRATIVE);
    public static SoundInfo NARRATIVE_2 = new SoundInfo(1, 0.795f, NARRATIVE);
    public static SoundInfo NARRATIVE_3 = new SoundInfo(2, 0.795f, NARRATIVE);
    public static SoundInfo NARRATIVE_4 = new SoundInfo(3, 0.795f, NARRATIVE);
    public static SoundInfo NARRATIVE_5 = new SoundInfo(4, 0.795f, NARRATIVE);

    //<--------------------AUDIO SOURCE INDEXES--------------------->
    public const int INTERACTION = 0;
    public const int AMBIENCE = 1;
    public const int VOLCANO = 2;
    public const int CAR = 3;
    public const int RIVER = 4;
    public const int DRAGON = 5;
    public const int HUMAN = 6;
    public const int SQUIRREL = 7;
    public const int NARRATIVE = 8;

    //<--------------------------CLIPS------------------------------>
    [SerializeField]
    private List<AudioClip> interaction_clips;

    [SerializeField]
    private List<AudioClip> ambience_clips;

    [SerializeField]
    private List<AudioClip> volcano_clips;

    [SerializeField]
    private List<AudioClip> car_clips;

    [SerializeField]
    private List<AudioClip> river_clips;

    [SerializeField]
    private List<AudioClip> dragon_clips;

    [SerializeField]
    private List<AudioClip> human_clips;

    [SerializeField]
    private List<AudioClip> squirrel_clips;

    [SerializeField]
    private List<AudioClip> narrative_clips;

    public List<ClipList> GetLevelClips()
    {
        List<ClipList> level_clips = new List<ClipList>();

        level_clips.Add(new ClipList(interaction_clips));
        level_clips.Add(new ClipList(ambience_clips));
        level_clips.Add(new ClipList(volcano_clips));
        level_clips.Add(new ClipList(car_clips));
        level_clips.Add(new ClipList(river_clips));
        level_clips.Add(new ClipList(dragon_clips));
        level_clips.Add(new ClipList(human_clips));
        level_clips.Add(new ClipList(squirrel_clips));
        level_clips.Add(new ClipList(narrative_clips));

        return level_clips;
    }
}
