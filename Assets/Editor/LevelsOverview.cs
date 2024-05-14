using System;
using ScriptableObjects;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using SerializedObject = UnityEditor.SerializedObject;

namespace Editor
{
    public class LevelsOverview : UnityEditor.EditorWindow
    {
        [UnityEditor.MenuItem("MoreWindow/Levels Overview Window")]
        private static void ShowWindow()
        {
            var window = GetWindow<LevelsOverview>();
            window.titleContent = new UnityEngine.GUIContent("Levels Overview");
            window.Show();
        }
        public Vector2 scrollPosition = Vector2.zero;
        public Level focusOnLevel;
        private void OnGUI()
        {
            LoadAllAssetsOfType<AllLevels>(out AllLevels[] A);
            
            SerializedObject allLevelList = new SerializedObject(A[0]);
            AllLevels thisAllLevels = A[0]; //the AllLevels Scriptable Object
            SerializedObject serializedAllLevels = new SerializedObject(thisAllLevels); //the AllLevels Serialized Object

            thisAllLevels.UpdateAllLevelsList();
            scrollPosition = GUI.BeginScrollView(new Rect(10, 10, position.width/4, position.height), scrollPosition, new Rect(0, 0, position.width/4, thisAllLevels.allLevelsID.Count*50+150), true, true);
            for (int i = 0; i < thisAllLevels.allLevelsID.Count; i++)
            {
                GUILayout.BeginArea(new Rect(10, 10+(i*50), position.width/4, 200));
                EditorGUILayout.BeginVertical();
                SerializedObject serializedLevel = new SerializedObject(thisAllLevels.allLevels[i]);

                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.LabelField("Level Name :", GUILayout.Width(EditorGUIUtility.labelWidth - 70));
                EditorGUILayout.PropertyField(serializedLevel.FindProperty("levelName"), GUIContent.none);
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.EndVertical();
                
                EditorGUILayout.BeginHorizontal();
                
                GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
                buttonStyle.alignment = TextAnchor.MiddleCenter;
                if (GUILayout.Button("Show details", buttonStyle, GUILayout.Width(position.width/4 * 0.7f)))
                {
                    //Show details todo
                }
                
                buttonStyle.normal.textColor = Color.red;

                if (GUILayout.Button("✖", buttonStyle, GUILayout.Width(position.width/4 * 0.2f)))
                {
                    bool validation = EditorUtility.DisplayDialog("Warning", "You're about to delete " + serializedLevel.FindProperty("levelName").stringValue, "Oui", "Non");
                    if (validation)
                    {
                        AssetDatabase.DeleteAsset(thisAllLevels.allLevelsPath[i]);
                        thisAllLevels.allLevels.Remove(thisAllLevels.allLevels[i]);
                        thisAllLevels.allLevelsID.Remove(thisAllLevels.allLevelsID[i]);
                        thisAllLevels.allLevelsPath.Remove(thisAllLevels.allLevelsPath[i]);
                        thisAllLevels.UpdateAllLevelsList();
                        GUILayout.EndArea();
                        EditorGUILayout.EndHorizontal();
                        break;
                    }
                }
                serializedLevel.ApplyModifiedProperties();
                EditorGUILayout.EndHorizontal();
                GUILayout.EndArea();
                serializedLevel.ApplyModifiedProperties();
            }
            GUILayout.BeginArea(new Rect(10, 10+(thisAllLevels.allLevels.Count*50), position.width/4, 150));
            if (GUILayout.Button("Create a new Level ?"))
            {
                CreateNewLevel(thisAllLevels.allLevels[thisAllLevels.allLevels.Count-1]);
            }
            GUILayout.EndArea();
            GUI.EndScrollView();
            allLevelList.ApplyModifiedProperties();
        }

            
        private void LoadAllAssetsOfType<T>(out T[] assets) where T : Object
        {
            string[] guids = AssetDatabase.FindAssets("t:"+typeof(T));
            assets = new T[guids.Length];

            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                assets[i] = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            }
        }
        
        private void CreateNewLevel(Level level)
        {
            var newLevel = Instantiate(level);
            AssetDatabase.CreateAsset(newLevel, AssetDatabase.GenerateUniqueAssetPath("Assets/Data/Levels/NewLevel.asset"));
            SerializedObject serialized = new SerializedObject(newLevel);
            serialized.FindProperty("levelName").stringValue = "Level without name";
            
            serialized.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}