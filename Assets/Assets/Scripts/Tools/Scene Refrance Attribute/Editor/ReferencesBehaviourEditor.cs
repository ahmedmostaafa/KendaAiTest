using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace KabreetGames.SceneReferences.Editor
{
    [CustomEditor(typeof(ValidatedMonoBehaviour), true)]
    public class ReferencesBehaviourEditor : OdinEditor
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            if (target is Component targetComponent)
            {
                targetComponent.ValidateRefs();
            }
        }

        private void OnValidate()
        {
            if (target is Component targetComponent)
            {
                targetComponent.ValidateRefs();
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();
            if (!EditorGUI.EndChangeCheck()) return;
            if (target is Component targetComponent)
                targetComponent.ValidateRefs();
        }
    }
}