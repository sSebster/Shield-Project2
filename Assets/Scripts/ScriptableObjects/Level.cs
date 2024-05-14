using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Scriptable", menuName = "ProjectScriptable/Level", order = 1)]
    public class Level : ScriptableObject
    {
        #region Variables

        public string levelName = "Level without name";
        
        public List<Wave> allWavesInLevel;
        public List<ConditionWaveEnd> waintingConditionBetweenEachWaves;

        #endregion
    }
}