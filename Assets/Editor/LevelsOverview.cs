using System;
using ScriptableObjects;
using Unity.VisualScripting;
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
        public Vector2 scrollPosition2 = Vector2.zero;
        public Level focusOnLevel;
        public int numberOfWavesInFocusLevel;
        private void OnGUI()
        {
            LoadAllAssetsOfType<AllLevels>(out AllLevels[] A);
            
            int count = 0;
            
            SerializedObject allLevelList = new SerializedObject(A[0]);
            AllLevels thisAllLevels = A[0]; //the AllLevels Scriptable Object
            SerializedObject serializedAllLevels = new SerializedObject(thisAllLevels); //the AllLevels Serialized Object

            thisAllLevels.UpdateAllLevelsList();
            
            #region levels list

            LevelList(thisAllLevels, allLevelList, count);
            
            #endregion
            
            #region waves details
            
            if (focusOnLevel != null)
            {
                GUILayout.BeginArea(new Rect(position.width/4+50, 10, position.width, focusOnLevel.allWavesInLevel.Count*50+150));
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Close details", GUILayout.Height(25),GUILayout.Width(100)))
                {
                    focusOnLevel = null;
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                    GUILayout.EndArea();
                    GUILayout.EndArea();
                    return;
                }
                GUILayout.Space(10);
                GUIStyle boldLabelStyle = new GUIStyle(EditorStyles.label);
                boldLabelStyle.fontStyle = FontStyle.Bold;
                EditorGUILayout.LabelField("Details waves of " + focusOnLevel.levelName, boldLabelStyle);
                GUILayout.EndHorizontal();
                GUILayout.EndArea();
                scrollPosition2 = GUI.BeginScrollView(new Rect(10, 60, position.width-30, position.height-50), scrollPosition2, new Rect(0, 0, position.width-40, focusOnLevel.allWavesInLevel.Count*50+200), true, true);
                
                GUILayout.BeginArea(new Rect(position.width/4+50, 10, position.width/4*3-100, focusOnLevel.allWavesInLevel.Count*50+200));
                
                SerializedObject serializedFocusLevel = new SerializedObject(focusOnLevel);
                var allWavesInLevelProperty = serializedFocusLevel.FindProperty("allWavesInLevel");
                LoadAllAssetsOfType<Wave>(out Wave[] allWaves);
                
                for (int i = 0; i < allWavesInLevelProperty.arraySize; i++)
                {
                    GUILayout.BeginHorizontal();
                    SerializedProperty element = allWavesInLevelProperty.GetArrayElementAtIndex(i);
                    numberOfWavesInFocusLevel = allWavesInLevelProperty.arraySize;
                    Wave currentWave = element.objectReferenceValue as Wave;

                    int selectedIndex = -1;
                    string[] options = new string[allWaves.Length];

                    for (int j = 0; j < allWaves.Length; j++)
                    {
                        options[j] = allWaves[j].name;

                        if (allWaves[j] == currentWave)
                        {
                            selectedIndex = j;
                        }
                    }
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"Wave N°{i+1}", GUILayout.Width(EditorGUIUtility.labelWidth - 70));
                    int newSelectedIndex = EditorGUILayout.Popup(selectedIndex, options, GUILayout.ExpandWidth(true));
                    EditorGUILayout.EndHorizontal();   
                    
                    if (newSelectedIndex != selectedIndex)
                    {
                        element.objectReferenceValue = allWaves[newSelectedIndex];
                    }
                    
                    GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
                    buttonStyle.alignment = TextAnchor.MiddleCenter;
                    buttonStyle.normal.textColor = Color.red;
                    if (GUILayout.Button("✖", buttonStyle, GUILayout.Width(position.width/5 * 0.5f)))
                    {
                        bool validation = EditorUtility.DisplayDialog("Warning", "You're about to delete this wave", "Oui", "Non");
                        if (validation)
                        {
                            focusOnLevel.allWavesInLevel.RemoveAt(i);
                            if (i < focusOnLevel.waintingConditionBetweenEachWaves.Count)
                            {
                                focusOnLevel.waintingConditionBetweenEachWaves.RemoveAt(i);
                                GUILayout.EndVertical();
                            }
                            break;
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(25);
                }
                if (GUILayout.Button("Add new wave ?", GUILayout.Height(40)))
                {
                    focusOnLevel.allWavesInLevel.Add(null);
                }
                serializedFocusLevel.ApplyModifiedProperties();
                GUILayout.EndArea();
                GUI.EndScrollView();
            }
            
            #endregion
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

        private void LevelList(AllLevels thisAllLevels, SerializedObject allLevelList, int count)
        {
            scrollPosition = GUI.BeginScrollView(new Rect(10, 10, position.width/4+30, position.height), scrollPosition, new Rect(0, 0, position.width/4+30, (thisAllLevels.allLevelsID.Count+1)*65+100), true, true);
            for (int i = 0; i < thisAllLevels.allLevelsID.Count; i++)
            {
                GUILayout.BeginArea(new Rect(10, 10 + (i * 65), position.width / 4, position.height));
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
                    focusOnLevel = thisAllLevels.allLevels[i];
                    count = 0;
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
            }
            GUILayout.BeginArea(new Rect(10, (thisAllLevels.allLevelsID.Count + 1) * 65 -40, position.width/4, position.height));

            if (GUILayout.Button("Create a new Level ?", GUILayout.Height(40)))
            {
                CreateNewLevel(thisAllLevels.allLevels[thisAllLevels.allLevels.Count-1]);
            }
            GUILayout.EndArea();
            GUI.EndScrollView();
            allLevelList.ApplyModifiedProperties();
        }
    }
}