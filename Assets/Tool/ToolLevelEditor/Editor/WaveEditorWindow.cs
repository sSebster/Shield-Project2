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
    public class WaveEditorWindow : EditorWindow
    {
        [MenuItem("MoreWindow/Wave Editor Window")]
        private static void ShowWindow()
        {
            var window = GetWindow<WaveEditorWindow>();
            window.titleContent = new GUIContent("Wave Editor");
            window.Show();
        }

        public Vector2 scrollPosition = Vector2.zero;
        public Vector2 scrollPosition2 = Vector2.zero;
        private Wave focusOnWave;

        private void OnGUI()
        {
            LoadAllAssetsOfType<Wave>(out Wave[] allWaves);
            
            SerializedObject[] waveSerializedObjects = allWaves.Select(wave => new SerializedObject(wave)).ToArray();
            
            LoadAllAssetsOfType<AllLevels>(out AllLevels[] A);
            
            SerializedObject allLevelList = new SerializedObject(A[0]);
            AllLevels thisAllLevels = A[0]; //the AllLevels Scriptable Object
            SerializedObject serializedAllLevels = new SerializedObject(thisAllLevels); //the AllLevels Serialized Object

            thisAllLevels.UpdateAllLevelsList();
            
            #region waves list

            WaveList(allWaves, waveSerializedObjects);
            
            #endregion
            
            #region wave details

            if (focusOnWave != null)
            {
                SerializedObject serializedFocusWave = new SerializedObject(focusOnWave);

                GUILayout.BeginArea(new Rect(position.width / 4 + 50, 10, position.width / 4 * 3 - 75, position.height - 20));
                
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Close details", GUILayout.Height(25), GUILayout.Width(100)))
                {
                    focusOnWave = null;
                    GUILayout.EndHorizontal();
                    GUILayout.EndArea();
                    return;
                }
                
                GUIStyle boldLabelStyle = new GUIStyle(EditorStyles.label)
                {
                    fontStyle = FontStyle.Bold
                };
                
                EditorGUILayout.LabelField("Details of " + AssetDatabase.GetAssetPath(focusOnWave), boldLabelStyle);
                GUILayout.EndHorizontal();

                serializedFocusWave.Update();

                // Adding ScrollView
                //scrollPosition = GUI.BeginScrollView(new Rect(10, 40, position.width - 30, position.height - 160), scrollPosition2, new Rect(0, 0, position.width - 40, focusOnWave.allEnnemisInWave.Count), false, true);

                SerializedProperty allEnnemisInWave = serializedFocusWave.FindProperty("allEnnemisInWave");
                SerializedProperty waintingBetweenEachEnnemis = serializedFocusWave.FindProperty("waintingBetweenEachEnnemis");

                for (int i = 0; i < allEnnemisInWave.arraySize; i++)
                {
                    GUILayout.BeginHorizontal();

                    // Enemy popup
                    SerializedProperty enemy = allEnnemisInWave.GetArrayElementAtIndex(i);
                    Ennemi currentEnemy = enemy.objectReferenceValue as Ennemi;
                    int selectedIndex = -1;
                    LoadAllAssetsOfType<Ennemi>(out Ennemi[] allEnemies);
                    string[] options = new string[allEnemies.Length];
                    for (int j = 0; j < allEnemies.Length; j++)
                    {
                        options[j] = allEnemies[j].name;
                        if (allEnemies[j] == currentEnemy)
                        {
                            selectedIndex = j;
                        }
                    }
                    selectedIndex = EditorGUILayout.Popup(selectedIndex, options);
                    if (selectedIndex >= 0)
                    {
                        enemy.objectReferenceValue = allEnemies[selectedIndex];
                    }

                    // Waiting time field
                    if (i <= waintingBetweenEachEnnemis.arraySize && i < allEnnemisInWave.arraySize-1)
                    {
                        thisAllLevels.UpdateAllLevelsList();
                        SerializedProperty waitingTime = waintingBetweenEachEnnemis.GetArrayElementAtIndex(i);
                        waitingTime.floatValue = EditorGUILayout.FloatField(waitingTime.floatValue);
                    }
                    
                    GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
                    buttonStyle.alignment = TextAnchor.MiddleCenter;
                    buttonStyle.normal.textColor = Color.red;
                    if (GUILayout.Button("✖"))
                    {
                        bool validation = EditorUtility.DisplayDialog("Warning", "You're about to delete this ennemy", "Oui", "Non");
                        if (validation)
                        {
                            allEnnemisInWave.DeleteArrayElementAtIndex(i);
                            waintingBetweenEachEnnemis.DeleteArrayElementAtIndex(i);
                        }
                    }

                    GUILayout.EndHorizontal();
                }

                if (GUILayout.Button("Add Enemy"))
                {
                    allEnnemisInWave.InsertArrayElementAtIndex(allEnnemisInWave.arraySize);
                    waintingBetweenEachEnnemis.InsertArrayElementAtIndex(waintingBetweenEachEnnemis.arraySize);
                }

                //GUI.EndScrollView();

                serializedFocusWave.ApplyModifiedProperties();
                GUILayout.EndArea();
            }

            #endregion
        }

        private void LoadAllAssetsOfType<T>(out T[] assets) where T : Object
        {
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(T));
            assets = new T[guids.Length];

            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                assets[i] = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            }
        }

        private void WaveList(Wave[] allWaves, SerializedObject[] waveSerializedObjects)
        {
            scrollPosition2 = GUI.BeginScrollView(new Rect(10, 10, position.width / 4 + 30, position.height - 20), scrollPosition2, new Rect(0, 0, position.width / 4 + 30, (allWaves.Length + 1) * 65 + 100), true, true);
            for (int i = 0; i < allWaves.Length; i++)
            {
                GUILayout.BeginArea(new Rect(10, 10 + (i * 65), position.width / 4, position.height));
                EditorGUILayout.BeginVertical();
                SerializedObject serializedWave = waveSerializedObjects[i];

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Wave Path:", GUILayout.Width(EditorGUIUtility.labelWidth - 70));
                EditorGUILayout.LabelField(allWaves[i].ToString());
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginHorizontal();
                GUIStyle buttonStyle = new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter };

                if (GUILayout.Button("Show details", buttonStyle, GUILayout.Width(position.width / 4 * 0.7f)))
                {
                    focusOnWave = allWaves[i];
                }

                buttonStyle.normal.textColor = Color.red;
                if (GUILayout.Button("✖", buttonStyle, GUILayout.Width(position.width / 4 * 0.2f)))
                {
                    bool validation = EditorUtility.DisplayDialog("Warning", "You're about to delete " + AssetDatabase.GetAssetPath(allWaves[i]), "Oui", "Non");
                    if (validation)
                    {
                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(allWaves[i]));
                        serializedWave.Dispose();
                        GUIUtility.ExitGUI();
                        return;
                    }
                }
                
                serializedWave.ApplyModifiedProperties();
                EditorGUILayout.EndHorizontal();
                GUILayout.EndArea();
            }
            GUILayout.BeginArea(new Rect(10, 10 + (allWaves.Length * 65), position.width / 4, position.height));
            if (GUILayout.Button("Create a new Wave ?", GUILayout.Height(40)))
            {
                Wave newWave = CreateInstance<Wave>();
                AssetDatabase.CreateAsset(newWave, AssetDatabase.GenerateUniqueAssetPath("Assets/Data/Waves/NewWave.asset"));
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                LoadAllAssetsOfType<Wave>(out allWaves); // Recharger la liste des waves
            }
            GUILayout.EndArea();
            GUI.EndScrollView();
        }
    }
}
