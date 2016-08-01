using System.Collections.Generic;
using UnityEngine;

namespace Demonixis.Toolbox.Graphics
{
    public class SimpleOclusionCulling : MonoBehaviour
    {
        private Transform[] _transforms = null;
        private int _count = 0;
        private Renderer[] _renderers = null;

        [SerializeField]
        private Transform _player = null;

        [SerializeField]
        private GameObject[] _containers = null;

        [SerializeField]
        private GameObject[] _excluedContainers = null;

        [SerializeField]
        private float _fieldOfView = 120.0f;

        [SerializeField]
        private float _maxDistance = 45.0f;

        [SerializeField]
        private Vector3 _offsetPoint = new Vector3(0.0f, 1.0f, -4.0f);

        void Start()
        {
            if (_containers == null || _containers.Length == 0)
                _containers = new GameObject[1] { gameObject };

            if (_player == null)
            {
                var go = GameObject.FindWithTag("Player");
                if (go != null)
                {
                    Transform node = null;
                    var cam = go.GetComponentInChildren(typeof(Camera));
                    if (cam != null)
                        node = (Transform)cam.GetComponent(typeof(Transform));
                    else
                        node = (Transform)go.GetComponent(typeof(Transform));

                    var backPoint = new GameObject("BackPointView");
                    _player = (Transform)backPoint.GetComponent(typeof(Transform));
                    _player.parent = node.transform;
                    _player.localPosition = _offsetPoint;
                    _player.localRotation = Quaternion.identity;
                    _player.localScale = Vector3.one;
                }
                else
                    Destroy(this);
            }

            var list = new List<Renderer>();
            var i = 0;
            var size = _containers.Length;

            for (i = 0; i < size; i++)
                list.AddRange(_containers[i].GetComponentsInChildren<Renderer>());

            if (_excluedContainers != null)
            {
                Renderer[] exclued = null;
                int j = 0;
                int size2 = 0;

                for (i = 0; i < size; i++)
                {
                    exclued = _excluedContainers[i].GetComponentsInChildren<Renderer>();

                    for (j = 0, size2 = exclued.Length; j < size2; j++)
                        list.Remove(exclued[j]);
                }
            }

            _renderers = list.ToArray();
            _count = _renderers.Length;
            _transforms = new Transform[_count];

            for (i = 0; i < _count; i++)
                _transforms[i] = (Transform)_renderers[i].GetComponent(typeof(Transform));
        }

        void LateUpdate()
        {
            var angle = 0.0f;
            var distance = 0.0f;
            var direction = Vector3.zero;
            var disable = false;

            for (int i = 0; i < _count; i++)
            {
                if (_transforms[i] == null)
                    continue;

                direction = _transforms[i].position - _player.position;
                angle = Vector3.Angle(direction, _player.forward);
                distance = Vector3.Distance(_transforms[i].position, _player.position);
                disable = distance > _maxDistance;

                if (angle > _fieldOfView * 0.5f)
                    disable = true;

                if (_renderers[i].enabled && disable)
                    _renderers[i].enabled = false;
                else if (!_renderers[i].enabled && !disable)
                    _renderers[i].enabled = true;
            }
        }
    }
}
