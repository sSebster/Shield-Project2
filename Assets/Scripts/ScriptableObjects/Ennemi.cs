using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Scriptable", menuName = "ProjectScriptable/Ennemis", order = 4)]
    public class Ennemi : ScriptableObject
    {
        #region Variables
        
        public enum ennemisType : int
        {
            Melee,
            Range,
        }

        public enum movementType : int
        {
            ZigZag,
            StraightLign,
            Static,
        }

        public ennemisType EnnemisType;
        public movementType MovementType;
        public float EnnemisSpeed;
        public float EnnemisMaxSpeed;
        
        public GameObject bullet;

        public float cooldown;
        private float actualTime;

        #endregion

    }
}