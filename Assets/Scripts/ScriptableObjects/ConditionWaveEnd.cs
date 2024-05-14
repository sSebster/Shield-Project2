using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Scriptable", menuName = "ProjectScriptable/ConditionWaveEnd", order = 3)]
    public class ConditionWaveEnd : ScriptableObject
    {
        #region Variables

        public enum ConditionToEnd
        {
            Time,
            AllEnnemisDied,
        }
        
        public ConditionToEnd conditionToEnd;
        public float timeToWait;

        #endregion
    }
}