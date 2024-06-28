using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Scriptable", menuName = "ProjectScriptable/AllLevels", order = 0)]
    public class AllLevels : ScriptableObject
    {
        public List<Level> allLevels;
        public List<String> allLevelsPath;
        public List<String> allLevelsID;

        public void UpdateAllLevelsList()
        {
            string[] levels = UnityEditor.AssetDatabase.FindAssets("t:Level");
            for (int i = 0; i < levels.Length; i++)
            {
                if (!allLevelsID.Contains(levels[i]))
                {
                    allLevelsID.Add(levels[i]);
                }
                
                var levelPath = UnityEditor.AssetDatabase.GUIDToAssetPath(levels[i]);
                if (!allLevelsPath.Contains(levelPath))
                {
                    allLevelsPath.Add(levelPath);
                    allLevels.Add(AssetDatabase.LoadAssetAtPath<Level>(levelPath));
                }

                if (allLevels[i].waintingConditionBetweenEachWaves.Count < allLevels[i].allWavesInLevel.Count)
                {
                    for (int j = 0; j < allLevels[i].allWavesInLevel.Count - allLevels[i].waintingConditionBetweenEachWaves.Count; j++)
                    {
                        allLevels[i].waintingConditionBetweenEachWaves.Add(null);
                    }
                }
            }

            for (int i = 0; i < allLevelsID.Count; i++)
            {
                if (!levels.Contains(allLevelsID[i]))
                {
                    allLevelsID.Remove(allLevelsID[i]);
                    allLevelsPath.RemoveAt(i);
                    allLevels.RemoveAt(i);
                }
            }
            
            Wave[] allWaves = AssetDatabase.FindAssets("t:Wave").Select(guid => AssetDatabase.LoadAssetAtPath<Wave>(AssetDatabase.GUIDToAssetPath(guid))).ToArray();
            foreach (var wave in allWaves)
            {
                if (wave != null)
                {
                    if (wave.allEnnemisInWave != null && wave.waintingBetweenEachEnnemis != null)
                    {
                        while (wave.allEnnemisInWave.Count > wave.waintingBetweenEachEnnemis.Count)
                        {
                            wave.waintingBetweenEachEnnemis.Add(0);
                        }
                    }
                }
            }

        }
    }
}