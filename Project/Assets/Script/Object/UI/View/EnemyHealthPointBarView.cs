using System.Collections;
using System.Collections.Generic;
using Backend.Util.Debug;
using Unity.VisualScripting;
using UnityEngine;

namespace Backend.Object.UI.View
{
    public class EnemyHealthPointBarView : DelayedPointBarView
    {
        [SerializeField] private bool lookCamera = false;

        private Camera _camera;
        private Canvas _canvas;

        private void Awake()
        {
            base.Awake();

            if (lookCamera)
            {
                _canvas = GetComponentInParent<Canvas>();
                if (_canvas != null)
                {
                    _camera = Camera.main;
                }
            }
        }

        private void Update()
        {
            if (!lookCamera || _camera == null)
            {
                return;
            }

            var camTransform = Camera.main.transform;
            var canvasTransform = _canvas.transform;
            var camDir = canvasTransform.position - camTransform.position;
            camDir.y = 0f;

            canvasTransform.rotation = Quaternion.LookRotation(camDir.normalized, Vector3.up);
        }

        public override void Change(float value) => base.Change(value);
    }
}
