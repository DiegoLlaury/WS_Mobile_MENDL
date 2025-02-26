using UnityEngine;
using System.Collections;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(DeckManager))]
public class DeckMannagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DeckManager deckManager = (DeckManager)target;
        if (GUILayout.Button("Draw Next Card"))
        {
            HandManager handManager = FindAnyObjectByType<HandManager>();
            if (handManager != null)
            {
                deckManager.DrawCard(handManager);
            }
        }
    }
}
#endif