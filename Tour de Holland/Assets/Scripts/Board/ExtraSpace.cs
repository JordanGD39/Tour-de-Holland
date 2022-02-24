using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraSpace : MonoBehaviour
{
    [SerializeField] private int spaceBeforeThisIndex = -1;
    public int SpaceBeforeThisIndex { get { return spaceBeforeThisIndex; } }
    [SerializeField] private bool cutsceneSpace = false;
    public bool CutsceneSpace { get { return cutsceneSpace; } }
    public delegate void PlayCutscene(Transform player);
    public PlayCutscene OnPlayCutscene;
    public delegate void CutsceneDone();
    public CutsceneDone OnCutsceneDone;
}
