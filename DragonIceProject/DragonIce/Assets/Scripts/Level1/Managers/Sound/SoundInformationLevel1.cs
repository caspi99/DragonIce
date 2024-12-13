using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundInformationLevel1 : MonoBehaviour, SoundInformation
{
    //<-------------------NARRATIVE INDEXES------------------------>
    public static SoundInfo N_1_INTRO = new SoundInfo(0, 0.795f, NARRATIVE);
    public static SoundInfo N_2_FLY = new SoundInfo(1, 0.795f, NARRATIVE);
    public static SoundInfo N_3_CHARACTERS = new SoundInfo(2, 0.795f, NARRATIVE);
    public static SoundInfo N_4_FREEZE = new SoundInfo(3, 0.795f, NARRATIVE);
    public static SoundInfo N_5_HELP_DRAGON = new SoundInfo(4, 0.795f, NARRATIVE);
    public static SoundInfo N_6_DOCKS = new SoundInfo(5, 0.795f, NARRATIVE);
    public static SoundInfo N_7_FIRST_STONE = new SoundInfo(6, 0.795f, NARRATIVE);
    public static SoundInfo N_8_FOLLOW_STONE = new SoundInfo(7, 0.795f, NARRATIVE);
    public static SoundInfo N_9_FINAL = new SoundInfo(8, 0.795f, NARRATIVE);
    public static SoundInfo ADELANTE = new SoundInfo(9, 0.795f, NARRATIVE);

    //<---------------------AMBIENT INDEXES------------------------->
    public static SoundInfo AMBIENT_START = new SoundInfo(0, 0.061f, AMBIENT);
    public static SoundInfo AMBIENT_MUSIC = new SoundInfo(1, 0.061f, AMBIENT);
    public static SoundInfo WIND = new SoundInfo(2, 0.061f, AMBIENT);

    //<----------------------WATER INDEXES-------------------------->
    public static SoundInfo SWIM_UNDER_WATER = new SoundInfo(0, 0.242f, WATER);
    public static SoundInfo INTO_WATER = new SoundInfo(1, 0.242f, WATER);
    public static SoundInfo WATER_DROP = new SoundInfo(2, 0.242f, WATER);
    public static SoundInfo WATER_DROP_WIRES = new SoundInfo(3, 0.242f, WATER);
    public static SoundInfo ENTERING_WATER = new SoundInfo(4, 0.242f, WATER);

    //<-----------------------ICE INDEXES--------------------------->
    public static SoundInfo CRACK = new SoundInfo(0, 0.6f, ICE);
    public static SoundInfo FINAL_CRACK = new SoundInfo(1, 0.6f, ICE);
    public static SoundInfo HIT = new SoundInfo(2, 0.6f, ICE);
    public static SoundInfo HITTING_ICE = new SoundInfo(3, 0.6f, ICE);
    public static SoundInfo HITTING_ICE_FINAL = new SoundInfo(4, 0.6f, ICE);
    public static SoundInfo ICE_CRACKING = new SoundInfo(5, 0.6f, ICE);
    public static SoundInfo ICE_CRACKING_2 = new SoundInfo(6, 0.6f, ICE);
    public static SoundInfo STOMP_1 = new SoundInfo(7, 0.6f, ICE);
    public static SoundInfo STOMP_2 = new SoundInfo(8, 0.6f, ICE);
    public static SoundInfo STOMP_3 = new SoundInfo(9, 0.6f, ICE);
    public static SoundInfo WATER_FREEZING = new SoundInfo(10, 0.6f, ICE);

    //<----------------------DRAGON INDEXES------------------------->
    public static SoundInfo ANGRY = new SoundInfo(0, 0.5f, DRAGON);
    public static SoundInfo DRAGON_GROWL = new SoundInfo(1, 0.5f, DRAGON);
    public static SoundInfo FLY = new SoundInfo(2, 0.5f, DRAGON);
    public static SoundInfo TRAPPED_NOISE = new SoundInfo(3, 0.5f, DRAGON);
    public static SoundInfo SWIM = new SoundInfo(4, 0.5f, DRAGON);

    //<----------------------HUMANS INDEXES------------------------->
    public static SoundInfo BUTTON_CLICK = new SoundInfo(0, 0.674f, HUMANS);
    public static SoundInfo SNOW_WALK = new SoundInfo(1, 0.674f, HUMANS);
    public static SoundInfo THROW_MOVEMENT = new SoundInfo(2, 0.674f, HUMANS);
    public static SoundInfo WATER_WALK = new SoundInfo(3, 0.674f, HUMANS);
    public static SoundInfo WALK = new SoundInfo(4, 0.674f, HUMANS);

    //<----------------------KNIGHT INDEXES------------------------->
    public static SoundInfo KNIGHT_LAUGHING = new SoundInfo(0, 0.691f, KNIGHT);

    //<----------------------SCIENTIST INDEXES------------------------->
    public static SoundInfo MAN_LAUGHING = new SoundInfo(0, 0.708f, SCIENTIST);

    //<--------------------INTERACTION INDEXES---------------------->
    public static SoundInfo CELEBRATION = new SoundInfo(0, 0.334f, INTERACTION);
    public static SoundInfo CONFIRMATION = new SoundInfo(1, 0.334f, INTERACTION);
    public static SoundInfo ROUND_PASS = new SoundInfo(2, 0.334f, INTERACTION);
    public static SoundInfo KICK_STONE_PERFECT = new SoundInfo(3, 0.334f, INTERACTION);
    public static SoundInfo STONE_THROW = new SoundInfo(4, 0.334f, INTERACTION);
    public static SoundInfo STONE_SHINE = new SoundInfo(5, 0.334f, INTERACTION);
    public static SoundInfo KICK_STONE_GOOD = new SoundInfo(6, 0.334f, INTERACTION);

    //<--------------------AUDIO SOURCE INDEXES--------------------->
    public const int NARRATIVE = 0;
    public const int AMBIENT = 1;
    public const int WATER = 2;
    public const int ICE = 3;
    public const int DRAGON = 4;
    public const int HUMANS = 5;
    public const int KNIGHT = 6;
    public const int SCIENTIST = 7;
    public const int INTERACTION = 8;

    //<--------------------------CLIPS------------------------------>

    [SerializeField]
    private List<AudioClip> narrative_clips;

    [SerializeField]
    private List<AudioClip> ambient_clips;

    [SerializeField]
    private List<AudioClip> water_clips;

    [SerializeField]
    private List<AudioClip> ice_clips;

    [SerializeField]
    private List<AudioClip> dragon_clips;

    [SerializeField]
    private List<AudioClip> humans_clips;

    [SerializeField]
    private List<AudioClip> knight_clips;

    [SerializeField]
    private List<AudioClip> scientist_clips;

    [SerializeField]
    private List<AudioClip> interaction_clips;

    public List<ClipList> GetLevelClips()
    {
        List<ClipList> level_clips = new List<ClipList>();

        level_clips.Add(new ClipList(narrative_clips));
        level_clips.Add(new ClipList(ambient_clips));
        level_clips.Add(new ClipList(water_clips));
        level_clips.Add(new ClipList(ice_clips));
        level_clips.Add(new ClipList(dragon_clips));
        level_clips.Add(new ClipList(humans_clips));
        level_clips.Add(new ClipList(knight_clips));
        level_clips.Add(new ClipList(scientist_clips));
        level_clips.Add(new ClipList(interaction_clips));

        return level_clips;
    }
}
