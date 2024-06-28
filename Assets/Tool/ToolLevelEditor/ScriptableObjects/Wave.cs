using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Scriptable", menuName = "ProjectScriptable/Wave", order = 2)]
    public class Wave : ScriptableObject
    {
        #region Variables

        public List<Ennemi> allEnnemisInWave;
        public List<float> waintingBetweenEachEnnemis;

        #endregion
    }
}