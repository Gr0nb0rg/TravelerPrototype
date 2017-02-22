using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/*
    Author: Ludvig Grönborg
    Email: ludviggronborg@hotmail.com
    Phone: 0730654281 
    Last Edited: 2017/02/21
*/

/*
    SUMMARY
    Does all the things 
*/
[System.SerializableAttribute]
[CustomEditor(typeof(Ledge))]
[CanEditMultipleObjects]
public class LedgeEditor : Editor {
    // Change these to prefab names
    private const string m_nameOfLedgeVertex = "LedgeVertex";
    private const string m_nameOfLedgeEdge = "LedgeEdge";

    // Selection in editor
    private Ledge m_target;

    // Settings
    private GlobalLedgeSettings m_ledgeSettings;
    private const string m_ledgeSettingsPath = "Assets/scripts/Climbing/Ledges/Global Ledge Settings.asset";

    private void OnEnable()
    {
        m_ledgeSettings = (GlobalLedgeSettings)AssetDatabase.LoadAssetAtPath(m_ledgeSettingsPath, typeof(GlobalLedgeSettings));
    }

    public override void OnInspectorGUI()
    {
        if(m_ledgeSettings == null)
            m_ledgeSettings = (GlobalLedgeSettings)AssetDatabase.LoadAssetAtPath(m_ledgeSettingsPath, typeof(GlobalLedgeSettings));

        // Assign selection
        m_target = (Ledge)target;

        // Apply updates from editor
        TrimEmptyListEntries();
        UpdateEdges();

        // All that is seen in the inspector
        GUILayout.Label("Ledge options", EditorStyles.boldLabel, GUILayout.Width(EditorGUIUtility.currentViewWidth - 19));
        DrawGlobalVariableFields();
        DrawSettingButtons();

        GUILayout.Space(10);
        GUILayout.Label("Add buttons", EditorStyles.boldLabel, GUILayout.Width(EditorGUIUtility.currentViewWidth - 19));
        DrawVertexButtons();

        GUILayout.Space(10);
        GUILayout.Label("View of components in ledge", EditorStyles.boldLabel, GUILayout.Width(EditorGUIUtility.currentViewWidth - 19));
        DrawDefaultInspector();

        EditorUtility.SetDirty(m_target);
    }

    // Applies cnanges from editor
    private void UpdateEdges()
    {
        List<LedgeEdge> edges = m_target.Edges;
        if (edges.Count > 0)
        {
            for (int i = 0; i < edges.Count; i++)
            {
                if (edges[i] != null)
                {
                    edges[i].PlaceEdgeBetweenVertices(1);
                    edges[i].FitTriggerBox(1, m_ledgeSettings.TriggerHeight, m_ledgeSettings.TriggerDepth);
                    edges[i].UpdateClimbTargets(m_ledgeSettings.DistanceBetweenPoints);
                }
            }
        }
    }

    // Makes sure ledge has no empty entries
    private void TrimEmptyListEntries()
    {
        bool correctionRequired = false;

        List<LedgeEdge> edges = m_target.Edges;
        if (edges.Count > 0)
        {
            for (int i = edges.Count - 1; i >= 0; i--)
            {
                if (edges[i] == null)
                {
                    edges.RemoveAt(i);
                    correctionRequired = true;
                }
            }
        }

        List<LedgeVertex> vertices = m_target.Vertices;
        if (vertices.Count > 0)
        {
            for (int i = vertices.Count - 1; i >= 0; i--)
            {
                if (vertices[i] == null)
                {
                    vertices.RemoveAt(i);
                    correctionRequired = true;
                }
            }
        }

        if (correctionRequired)
            CorrectLedgeComponents();
    }

    // Corrects names and connections to represent editor Ledge
    private void CorrectLedgeComponents()
    {
        Debug.Log("Corrected");
        List<LedgeEdge> edges = m_target.Edges;
        List<LedgeVertex> vertices = m_target.Vertices;

        if(vertices.Count > 0)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                // Create new names
                vertices[i].name = "LedgeVertex " + (i + 1);

                // All vertices that should have edges
                /*================EDGES================*/
                if (vertices.Count >= 2 && (i < vertices.Count-1))
                {
                    // There is no valid edge from this vertex
                    if((vertices[i].EdgeFurthestFromOrigin) == null)
                    {
                        CreateEdge(vertices[i], vertices[i + 1], i);
                    }
                    // There is a valid edge from this vertex
                    else
                    {
                        edges[i].name = "LedgeEdge " + i;
                        edges[i].VertexFurthestFromOrigin = vertices[i + 1];
                        vertices[i + 1].EdgeClosestToOrigin = edges[i];
                    }
                }

                /*================VERTICES================*/
                // Look for right
                if(i < vertices.Count-1)
                {
                    vertices[i].RightNeighbour = vertices[i + 1];
                }
                // Look for left
                if (i > 0)
                {
                    vertices[i].LeftNeighbour = vertices[i - 1];
                }
                
            }
        }
    }

    /* FUNCTIONS THAT BUILD THE LEDGE SYSTEM */
    #region Instantiation

    // Instantiates and adds a vertex to the ledge 
    private void CreateVertex()
    {
        List<LedgeVertex> vertices = m_target.Vertices;
        
        GameObject instance = Instantiate(Resources.Load("LedgeVertex", typeof(GameObject))) as GameObject;
        LedgeVertex instanceVertex = instance.GetComponent<LedgeVertex>();
        instance.name = "LedgeVertex " + (vertices.Count + 1);
        instance.transform.parent = m_target.transform;
        if (vertices.Count == 0)
        {
            // Add first
            instance.transform.rotation = m_target.transform.rotation;
            instance.transform.position = m_target.transform.position;
        }
        else
        {
            instance.transform.position = vertices[vertices.Count - 1].transform.position;
            instance.transform.rotation = vertices[vertices.Count - 1].transform.rotation;
            LedgeVertex lastVertex = vertices[vertices.Count - 1];
            // Create edge to previous
            CreateEdge(lastVertex, instanceVertex, m_target.Edges.Count + 1);
            // Neighbour this to previous
            lastVertex.RightNeighbour = instanceVertex;
            instanceVertex.LeftNeighbour = lastVertex;

            // Switch selection
            GameObject[] selection = new GameObject[1];
            selection[0] = instance;
            Selection.objects = selection;
        }

        // Add LedgeVertex to list
        m_target.AddVertex(instanceVertex);
        EditorUtility.SetDirty(m_target);
    }

    // Instantiates and adds a edge between to vertices 
    private void CreateEdge(LedgeVertex from, LedgeVertex to, int edgeNum)
    {
        GameObject instance = Instantiate(Resources.Load("LedgeEdge", typeof(GameObject))) as GameObject;
        LedgeEdge instanceEdge = instance.GetComponent<LedgeEdge>();

        instance.name = "LedgeEdge " + edgeNum;
        instanceEdge.VertexClosestToOrigin = from;
        instanceEdge.VertexFurthestFromOrigin = to;
        instance.transform.position = from.transform.position + (to.transform.position - from.transform.position) * 0.5f;
        instance.transform.parent = from.transform;

        from.EdgeFurthestFromOrigin = instanceEdge;
        to.EdgeClosestToOrigin = instanceEdge;
        m_target.AddEdge(instanceEdge);
    }

    #endregion

    /* WELCOME TO THE RICEFIELDS MOTHERFUCKER */
    #region Fields

    private void DrawGlobalVariableFields()
    {
        GUILayout.BeginHorizontal();
        {
            m_ledgeSettings.DistanceBetweenPoints = EditorGUILayout.FloatField("Distance Between Points: ", m_ledgeSettings.DistanceBetweenPoints, GUILayout.Width(EditorGUIUtility.currentViewWidth - 19));
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        {
            m_ledgeSettings.TriggerHeight = EditorGUILayout.FloatField("Height of trigger boxes: ", m_ledgeSettings.TriggerHeight, GUILayout.Width(((EditorGUIUtility.currentViewWidth)/2)));
            m_ledgeSettings.TriggerDepth = EditorGUILayout.FloatField("Depth of trigger boxes: ", m_ledgeSettings.TriggerDepth, GUILayout.Width(((EditorGUIUtility.currentViewWidth)/2)-23));

        }
        GUILayout.EndHorizontal();
        EditorUtility.SetDirty(m_ledgeSettings);
    }

    #endregion

    /* BUTTONS THAT ARE DRAWN TO GUI */
    #region Buttons

    /* ================== Button Collections ================== */
    private void DrawVertexButtons()
    {
        GUILayout.BeginHorizontal();
        {
            DrawAddVertex();
        }
        GUILayout.EndHorizontal();
    }
    
    private void DrawSettingButtons()
    {
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        {
            DrawConnectFirstToLast();
            DrawDisonnectFirstToLast();
        }
        GUILayout.EndHorizontal();
    }

    /* ================== Single Buttons ================== */
    private void DrawAddVertex()
    {
        if (GUILayout.Button("Vertex", GUILayout.Height(30)))
        {
            Undo.RecordObject(m_target, "Vertex");  
            CreateVertex();
            EditorUtility.SetDirty(m_target);
        }
    }

    private void DrawConnectFirstToLast()
    {
        if (GUILayout.Button("Connect first to last", GUILayout.Height(30)))
        {
            Undo.RecordObject(m_target, "Connect first to last");
            if (m_target.ConnectFirstToLast())
                EditorUtility.SetDirty(m_target);
            else
                Debug.Log("Ledge does not have enough vertices, edges or targets to do this");
        }
    }

    private void DrawDisonnectFirstToLast()
    {
        if (GUILayout.Button("Disconnect first from last", GUILayout.Height(30)))
        {
            Undo.RecordObject(m_target, "Disconnect first from last");
            if (m_target.DisconnectFirstFromLast())
                EditorUtility.SetDirty(m_target);
            else
                Debug.Log("Ledge does not have enough vertices, edges or targets to do this");
        }
    }

    #endregion

}
