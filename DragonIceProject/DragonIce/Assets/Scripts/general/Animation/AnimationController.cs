using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationInfo
{
    public string name { get; private set; }
    public int layer { get; private set; }
    public AnimationInfo(string name, int layer)
    {
        this.name = name; this.layer = layer;
    }
}

public class AnimationStatesLevel1
{
    //DRAGON PATHS
    public static AnimationInfo FIRST_SEQUENCE = new AnimationInfo("First Sequence", 0);
    public static AnimationInfo SECOND_SEQUENCE = new AnimationInfo("Second Sequence", 0);
    public static AnimationInfo THIRD_SEQUENCE = new AnimationInfo("Third Sequence", 0);
    public static AnimationInfo SWIM_PATH = new AnimationInfo("Swim", 0);
    public static AnimationInfo SWIRL = new AnimationInfo("Swirl", 0);
    public static AnimationInfo STAY_IN_THE_MIDDLE = new AnimationInfo("StayInTheMiddle", 0);
    public static AnimationInfo FOURTH_SEQUENCE = new AnimationInfo("Fourth Sequence", 0);
    public static AnimationInfo BREAK_ICE_PATH_IMPROVED = new AnimationInfo("BreakIce", 0);
    public static AnimationInfo FIFTH_SEQUENCE = new AnimationInfo("Fifth Sequence", 0);
    public static AnimationInfo SIXTH_SEQUENCE = new AnimationInfo("Sixth Sequence", 0);

    //DRAGON ACTIONS
    public static AnimationInfo FLY_UP = new AnimationInfo("FlyUp", 2);
    public static AnimationInfo ENTER_WATER = new AnimationInfo("EnterWater", 2);
    public static AnimationInfo TURN_LEFT = new AnimationInfo("TurnLeft", 2);
    public static AnimationInfo TURN_RIGHT = new AnimationInfo("TurnRight", 2);
    public static AnimationInfo SWIM_DOWN = new AnimationInfo("SwimDown", 2);
    public static AnimationInfo SWIM_UP = new AnimationInfo("SwimUp", 2);
    public static AnimationInfo FLY_SKILL = new AnimationInfo("FlySkill", 2);
    public static AnimationInfo SHELL_POSITION = new AnimationInfo("ShellPosition", 2);
    public static AnimationInfo SWIM = new AnimationInfo("Swim", 2);
    public static AnimationInfo DOWN_TO_FORWARD = new AnimationInfo("DownToForward", 2);
    public static AnimationInfo FORWARD_TO_DOWN = new AnimationInfo("ForwardToDown", 2);
    public static AnimationInfo UP_TO_FORWARD = new AnimationInfo("UpToForward", 2);
    public static AnimationInfo FORWARD_TO_UP = new AnimationInfo("ForwardToUp", 2);
    public static AnimationInfo SWIM_RIGHT = new AnimationInfo("SwimRight", 2);
    public static AnimationInfo SWIM_LEFT = new AnimationInfo("SwimLeft", 2);
    public static AnimationInfo SWIM_LEFT_SLOW = new AnimationInfo("SwimLeftSlow", 2);
    public static AnimationInfo FLY = new AnimationInfo("Fly", 3);
    public static AnimationInfo HIT = new AnimationInfo("Hit", 4);
    public static AnimationInfo FINAL_HIT = new AnimationInfo("FinalHit", 5);

    //IDLE
    public static AnimationInfo IDLE = new AnimationInfo("Idle", -1);
}

public class AnimationInformationLevel1
{
    //<-------------------OLD DRAGON INDEXES------------------------>
    public const string TURN_LEFT = "TurnLeft";
    public const string TURN_DOWN = "TurnDown";

    //<-------------------ANIMATORS INDEXES------------------------>
    public const int DRAGON = 0;
    public const int KNIGHT = 1;
    public const int SCIENTIST = 2;

    //<-------------------DRAGON ANIMATIONS LAYERS INDEXES------------------------>
    public const int NEW_ANIMATION = 6;
    public const int TURNS = 2;
    public const int HIT_ICE = 3;
    public const int FLY_LAYER = 4;
    public const int FINAL_HIT_ICE = 5;

    //<-------------------DRAGON ANIMATIONS TRIGGER PARAMETERS INDEXES------------------------>
    public const string FIRST_SEQUENCE = "First_Sequence";
    public const string SECOND_SEQUENCE = "Second_Sequence";
    public const string THIRD_SEQUENCE = "Third_Sequence";
    public const string INGAME = "Ingame";
    public const string INGAME_STAY = "IngameStay";
    public const string FOURTH_SEQUENCE = "Fourth_Sequence";
    public const string FIFTH_SEQUENCE = "Fifth_Sequence";
    public const string SIXTH_SEQUENCE = "Sixth_Sequence";

    public const string HIT_ICE_TRIGGER = "HitIce";
    public const string FINAL_HIT_TRIGGER = "FinalHit";

    public const string SWIRL = "Swirl";

    public const string SHELL_POSITION = "ShellPosition";
    public const string FLY_SKILL = "FlySkill";

    public const string DOWN_TO_FORWARD = "DownToForward";
    public const string UP_TO_FORWARD = "UpToForward";
    public const string FORWARD_TO_UP = "ForwardToUp";
    public const string FORWARD_TO_DOWN = "ForwardToDown";

    //<-------------------DRAGON ANIMATIONS BOOL PARAMETERS INDEXES------------------------>
    public const string OUTGAME = "Outgame";
    public const string BREAKICE = "BreakIce";

    public const string FLY = "Fly";
    public const string FLY_UP = "FlyUp";
    public const string SWIM_DOWN = "SwimDown";
    public const string SWIM_UP = "SwimUp";
    public const string SWIM_LEFT = "SwimLeft";
    public const string SWIM_RIGHT = "SwimRight";
    public const string SWIM_LEFT_SLOW = "SwimLeftSlow";
    public const string SWIM = "Swim";
    public const string ENTER_WATER = "EnterWater";

    //<-------------KNIGHT/SCIENTIST ANIMATIONS TRIGGER PARAMETERS INDEXES----------------->
    public const string START = "Start";
    public const string THROW = "Throw";
    public const string BUTTON = "Button";
    public const string ON_KNEES = "OnKnees";

    //<--------------KNIGHT/SCIENTIST ANIMATIONS BOOL PARAMETERS INDEXES------------------->
    public const string WALK = "Walk";
}

public class AnimationInformationLevel2
{
    //<-------------------ANIMATORS INDEXES------------------------>
    public const int DRAGON_1 = 0;
    public const int DRAGON_2 = 1;
    public const int JEEP = 2;
    public const int ARDILLA_PHASE1_IND = 3;
    public const int ARDILLA_PHASE1_COL = 4;
    public const int ARDILLA_PHASE2_IND = 5;
    public const int ARDILLA_PHASE2_COL = 6;
    public const int ARDILLA_PHASE2_IND2 = 7;
    public const int ARDILLA_PHASE2_COL2 = 8;

    //<-------------------DRAGON ANIMATIONS TRIGGER PARAMETERS INDEXES------------------------>
    public const string DRAGON_PATH_START = "PathStart";
    public const string DRAGON_PATH_END = "PathEnd";

    //<-------------------DRAGON ANIMATIONS BOOL PARAMETERS INDEXES------------------------>
    public const string FLY = "Fly";
    public const string BACKFLIP = "Backflip";
    public const string CLOSED_WINGS = "ClosedWings";

    //<-------------JEEP ANIMATIONS TRIGGER PARAMETERS INDEXES----------------->
    public const string PHASE_1_TRIGGER = "Phase1";
    public const string PHASE_2_TRIGGER = "Phase2";
    public const string PHASE_3_TRIGGER = "Phase3";
    public const string PHASE_2_SHORTER_TRIGGER = "Phase2Shorter";
    public const string PHASE_3_SHORTER_TRIGGER = "Phase3Shorter";

    //<--------------JEEP ANIMATIONS BOOL PARAMETERS INDEXES------------------->
    public const string RIGGING = "Rigging";
    public const string ARRANCAR = "Arrancar";
    public const string FRENADA = "Frenada";

    //<-------------ARDILLA_TRONCOS ANIMATIONS TRIGGER PARAMETERS INDEXES----------------->
    public const string ARDILLA_PIEDRA_INDIVIDUAL = "ArdillaPiedraIndividual";
    public const string ARDILLA_PIEDRA_COLECTIVO = "ArdillaPiedraColectivo";
    public const string ARDILLA_TRONCO_INDIVIDUAL = "ArdillaTroncoIndividual";
    public const string ARDILLA_TRONCO_COLECTIVO = "ArdillaTroncoColectivo";
    public const string ARDILLA_TRONCO_INDIVIDUAL_2 = "ArdillaTroncoIndividual2";
    public const string ARDILLA_TRONCO_COLECTIVO_2 = "ArdillaTroncoColectivo2";
}

public class AnimationController : MonoBehaviour
{
    //<------------------------ANIMATORS--------------------------->
    public List<Animator> animators;

    //<----------------------METHODS THAT USES THE ANIMATION CONTROLLER-------------------------->

    //method to change a bool of an animator
    private void ChangeBool(int animator_idx, string bool_name, bool status) { animators[animator_idx].SetBool(bool_name, status); }

    //<------------------------METHODS TO USE THE ANIMATION CONTROLLER--------------------------->

    public void SetTrigger(int animator_idx, string trigger) { animators[animator_idx].SetTrigger(trigger); } //method to set a trigger of an animator

    public void SetBoolTrue(int animator_idx, string bool_name) { ChangeBool(animator_idx, bool_name, true); } //method to set a bool of an animator to true
    public void SetBoolFalse(int animator_idx, string bool_name) { ChangeBool(animator_idx, bool_name, false); } //method to set a bool of an animator to false

    public void CrossFade(int animator_idx, AnimationInfo animInfo, float normalizedTransitionDuration, int layer=-1)
    {
        if (layer < 0) { layer = animInfo.layer; }
        animators[animator_idx].CrossFade(animInfo.name, normalizedTransitionDuration, layer);
    }

    public void SetAllToIdle()
    {
        for (int i = 0; i < animators.Count; i++)
        {
            animators[i].CrossFade("Idle", 0.1f, 0);
        }
    }

    public bool CheckIfCurrentState(int animator_idx, string state_name)
    {
        return animators[animator_idx].GetCurrentAnimatorStateInfo(0).IsName(state_name);
    }

    public void ChangeSpeed(int animator_idx, float speed)
    {
        animators[animator_idx].speed = speed;
    }

    public bool CheckIfAnimationIsNearToFinish(int animator_idx, int layer = 0)
    {
        return (animators[animator_idx].GetCurrentAnimatorStateInfo(layer).normalizedTime % 1) > 0.95f;
    }
}
