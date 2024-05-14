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
            foreach (var levelID in levels)
            {
                if (!allLevelsID.Contains(levelID))
                {
                    allLevelsID.Add(levelID);
                }
                
                var levelPath = UnityEditor.AssetDatabase.GUIDToAssetPath(levelID);
                if (!allLevelsPath.Contains(levelPath))
                {
                    allLevelsPath.Add(levelPath);
                    allLevels.Add(AssetDatabase.LoadAssetAtPath<Level>(levelPath));
                    
                }
            }

            for (int i = 0; i < allLevelsID.Count; i++)
            {
                if (!levels.Contains(allLevelsID[i]))
                {
                    int indexTemp = i;
                    allLevelsID.Remove(allLevelsID[i]);
                    allLevelsPath.RemoveAt(i);
                    allLevels.RemoveAt(i);
                }
            }
        }
    }
}