using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ClimbTarget))]
public class ClimbTargetEditor : Editor {

    private ClimbTarget m_target;

    public override void OnInspectorGUI()
    {
        m_target = (ClimbTarget)target;
        m_target.AssignPositions();
        m_target.ShowTargets = true;
        DrawDefaultInspector();
    }
}
