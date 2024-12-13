using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface SoundInformation
{
    List<ClipList> GetLevelClips();
}

public interface SequenceInformation
{
    List<Sequence> GetLevelSequences();
}

public interface GeneratorInformationLevel2
{
    List<PositionList> GetPositionLists();
    List<RotationList> GetRotationLists();
    List<PrefabList> GetPrefabLists();
    int[] GetNumberOfInstances();
}