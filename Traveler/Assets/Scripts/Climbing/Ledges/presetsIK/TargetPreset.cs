using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ledges/TargetPreset")]
public class TargetPreset : ScriptableObject {

    //public Transform root;
    public ClimbTarget m_presetSource;

    //private Vector3 m_rootPos = m_presetSource.GetRootTarget().position;
    private Vector3 m_otherPos;
    private float somefloat;


}
