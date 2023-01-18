using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;

namespace UnityEngine.UI
{
    public class HoldTimeButton : Button
    {
        public float timeToHold;
        public UnityEvent OnCompleteHold;

        private float _timer;
        private bool _isBeingPresed;

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            _isBeingPresed = true;
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (!_isBeingPresed)
                return;
            base.OnPointerClick(eventData);
            restore();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            restore();
        }

        private void Update()
        {
            if (_isBeingPresed)
            {
                _timer += Time.deltaTime;
                if (_timer >= timeToHold)
                {
                    OnCompleteHold.Invoke();
                    restore();
                }
            }
        }

        private void restore()
        {
            _timer = 0f;
            _isBeingPresed = false;
        }
    }
}

namespace UnityEditor.UI
{
#if UNITY_EDITOR
    [CustomEditor(typeof(HoldTimeButton))]
    public class HoldTimeButtonEditor : ButtonEditor
    {
        HoldTimeButton manager;

        GUIStyle style;
        protected override void OnEnable()
        {
            base.OnEnable();
            manager = (HoldTimeButton)target;

            style = new GUIStyle() { alignment = TextAnchor.MiddleCenter };
            style.normal.textColor = Color.white;
            style.fontStyle = FontStyle.Bold;
        }

        public override void OnInspectorGUI()
        {

            EditorGUILayout.LabelField("- Button Base Properties -", style, GUILayout.ExpandWidth(true));

            EditorGUILayout.Space();

            base.OnInspectorGUI();

            EditorGUILayout.LabelField("- Hold Time Button Properties -", style, GUILayout.ExpandWidth(true));

            EditorGUILayout.Space();


            manager.timeToHold = EditorGUILayout.FloatField("Time To Hold", manager.timeToHold);

            SerializedProperty prop = serializedObject.FindProperty("OnCompleteHold");

            EditorGUILayout.PropertyField(prop);

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}

