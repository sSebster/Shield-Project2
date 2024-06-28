using System;
using System.Linq;
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
        private void OnGUI()
        {
            LoadAllAssetsOfType<AllLevels>(out AllLevels[] A);
            
            SerializedObject allLevelList = new SerializedObject(A[0]);
            AllLevels thisAllLevels = A[0]; //the AllLevels Scriptable Object
            SerializedObject serializedAllLevels = new SerializedObject(thisAllLevels); //the AllLevels Serialized Object

            thisAllLevels.UpdateAllLevelsList();
            
            #region levels list

            LevelList(thisAllLevels, allLevelList);
            
            #endregion
            
            #region waves details
            
            if (focusOnLevel != null)
            {
                int calculatedHeight = (focusOnLevel.allWavesInLevel.Count * 110) + 80;
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
                
                GUILayout.Space(10);
                EditorGUILayout.LabelField("Summary :", boldLabelStyle);
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                EditorGUILayout.LabelField("-Total number of waves : " + focusOnLevel.allWavesInLevel.Count, boldLabelStyle);
                GUILayout.EndHorizontal();
                
                float waitTimeforAllWaves = 0;
                float timeOfWaves = 0;
                int numberOfWaitingForThePlayer = 0;
                for (int i = 0; i < focusOnLevel.waintingConditionBetweenEachWaves.Count-1; i++)
                {
                    var conditionList = focusOnLevel.waintingConditionBetweenEachWaves;
                    if (conditionList[i] != null && conditionList[i].conditionToEnd != ConditionWaveEnd.ConditionToEnd.AllEnnemisDied)
                    {
                        waitTimeforAllWaves += focusOnLevel.waintingConditionBetweenEachWaves[i].timeToWait;
                    }
                    if (conditionList[i] != null && conditionList[i].conditionToEnd == ConditionWaveEnd.ConditionToEnd.AllEnnemisDied)
                    {
                        numberOfWaitingForThePlayer++;
                    }
                }

                for (int i = 0; i < focusOnLevel.allWavesInLevel.Count(); i++)
                {
                    if (focusOnLevel.allWavesInLevel[i] != null)
                    {
                        for (int j = 0; j < focusOnLevel.allWavesInLevel[i].allEnnemisInWave.Count-1; j++)
                        {
                            if (j < focusOnLevel.allWavesInLevel[i].waintingBetweenEachEnnemis.Count())
                            {
                                timeOfWaves += focusOnLevel.allWavesInLevel[i].waintingBetweenEachEnnemis[j];
                            }
                        }
                    }
                }

                waitTimeforAllWaves += timeOfWaves;
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                EditorGUILayout.LabelField("-Time total of waves : " + waitTimeforAllWaves + " seconds", boldLabelStyle);
                GUILayout.EndHorizontal();
                
                
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                EditorGUILayout.LabelField("-We wait " + numberOfWaitingForThePlayer + " times for the player to kill every ennemis", boldLabelStyle);
                GUILayout.EndHorizontal();
                
                int totalNumberOfEnnemis = 0;

                for (int i = 0; i < focusOnLevel.allWavesInLevel.Count; i++)
                {
                    if (focusOnLevel.allWavesInLevel[i] != null)
                    {
                        totalNumberOfEnnemis += focusOnLevel.allWavesInLevel[i].allEnnemisInWave.Count;
                    }
                }
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                EditorGUILayout.LabelField("-Total numbers of ennemis : " + totalNumberOfEnnemis, boldLabelStyle);
                GUILayout.EndHorizontal();
                GUILayout.EndArea();
                
                GUILayout.Space(30);
                scrollPosition2 = GUI.BeginScrollView(new Rect(10, 170, position.width - 30, position.height - 160), scrollPosition2, new Rect(0, 0, position.width - 40, calculatedHeight), false, true);

                
                GUILayout.BeginArea(new Rect(position.width / 4 + 50, 10, position.width / 4 * 3 - 100, calculatedHeight));
                
                SerializedObject serializedFocusLevel = new SerializedObject(focusOnLevel);
                var allWavesInLevelProperty = serializedFocusLevel.FindProperty("allWavesInLevel");
                var waintingConditionBetweenEachWavesProperty = serializedFocusLevel.FindProperty("waintingConditionBetweenEachWaves");
                LoadAllAssetsOfType<Wave>(out Wave[] allWaves);
                LoadAllAssetsOfType<ConditionWaveEnd>(out ConditionWaveEnd[] allConditions);

                
                for (int i = 0; i < allWavesInLevelProperty.arraySize; i++)
                {
                    GUILayout.BeginHorizontal();
                    SerializedProperty element = allWavesInLevelProperty.GetArrayElementAtIndex(i);
                    SerializedProperty element2 = waintingConditionBetweenEachWavesProperty.GetArrayElementAtIndex(i);
                    Wave currentWave = element.objectReferenceValue as Wave;
                    //Start of First Popup//
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
                    selectedIndex = EditorGUILayout.Popup($"Wave {i+1}", selectedIndex, options);
                    
                    if (selectedIndex >= 0)
                    {
                        element.objectReferenceValue = allWaves[selectedIndex];
                    }
                    //End of First Popup//
                    
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
                    
                    if (i != allWavesInLevelProperty.arraySize - 1)
                    {
                        //Start of Second Popup//
                        if (selectedIndex >= 0)
                        {
                            element.objectReferenceValue = allWaves[selectedIndex];
                        }
                    
                        ConditionWaveEnd currentCondition = element2.objectReferenceValue as ConditionWaveEnd;
                        int selectedConditionIndex = -1;
                        string[] conditionOptions = new string[allConditions.Length];

                        for (int j = 0; j < allConditions.Length; j++)
                        {
                            conditionOptions[j] = allConditions[j].name;

                            if (allConditions[j] == currentCondition)
                            {
                                selectedConditionIndex = j;
                            }
                        }
                        GUILayout.Space(3);
                        selectedConditionIndex = EditorGUILayout.Popup($"-Condition to end", selectedConditionIndex, conditionOptions, GUILayout.Width(position.width/2));
                        if (selectedConditionIndex >= 0)
                        {
                            element2.objectReferenceValue = allConditions[selectedConditionIndex];
                        }
                        //End of Second Popup//
                    }
                    if (focusOnLevel.allWavesInLevel[i] != null)
                    { 
                        EditorGUILayout.LabelField("-Number of ennemies : " + focusOnLevel.allWavesInLevel[i].allEnnemisInWave.Count()); 
                    }
                    else
                    {
                        EditorGUILayout.LabelField("-Number of ennemies : N/A" );
                    }
                    
                    float timeOfTheWave = 0;
                    if (focusOnLevel.allWavesInLevel[i] != null)
                    {
                        for (int j = 0; j < focusOnLevel.allWavesInLevel[i].allEnnemisInWave.Count()-1; j++)
                        { 
                            timeOfTheWave += focusOnLevel.allWavesInLevel[i].waintingBetweenEachEnnemis[j];
                        }
                    }
                    
                    EditorGUILayout.LabelField("-Time of the wave : " + timeOfTheWave + " seconds");
                    //Todo Button Edit wave
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

        private void LevelList(AllLevels thisAllLevels, SerializedObject allLevelList)
        {
            GUILayout.Space(10);
            GUIStyle boldLabelStyle = new GUIStyle(EditorStyles.label);
            boldLabelStyle.fontStyle = FontStyle.Bold;
            EditorGUILayout.LabelField("All Levels :", boldLabelStyle);
            
            scrollPosition = GUI.BeginScrollView(new Rect(10, 30, position.width/4+30, position.height-20), scrollPosition, new Rect(0, 0, position.width/4+30, (thisAllLevels.allLevelsID.Count+1)*65+100), true, true);
            
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