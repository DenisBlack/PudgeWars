using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VIRA.Animations
{
    public class DotweenAppendSequence : MonoBehaviour
    {
        public enum AnimTypes
        {
            scale,
            movement,
            color,
            rotation,
            restartAnim
        }

        [Serializable]
        public class Animations
        {

            [Header("SetUpData")] public AnimTypes animType;
            public Ease Ease;
            public Vector3 changeVector;
            public Vector3 increment { get; private set; }

            public void SetIncrement(Vector3 inc)
            {
                increment = inc;
            }
            public float duration;
            [Header("delay")] public float delay;
            [Header("Loop")] public LoopType loopType;
            public int loopsAmoungt;
            [Header("Color")] public Color startColor;
            public Color endColor;
            [Header("Properties")]
            public bool isIncrimental;
        }

        [Serializable]
        public class AnimationsSimultanious
        {
            public List<Animations> animationsSimultanious;


            [Header("StartScale (onlyControlledByScript)")]
            public Vector3 startScale;
        }



        [Header("SetUP")]
        [SerializeField] List<AnimationsSimultanious> _animationsSequence;
        [SerializeField] AnimationSettingsObject _animationViaObject = null;
        [SerializeField] bool _useViaObjectAnim = false;
        [Header("OnClickAnimation")]
        [SerializeField] AnimationsSimultanious _clickAnim;
        [SerializeField] AnimationSettingsObject _defaultClickAnim;
        [SerializeField] bool _useDefaultClickAnim = true;



        private Sequence _Seq;
        private RectTransform _rectTransform;
        private Transform _transform;
        private Image _image;
        public Vector3 _stdScale;
        public Vector3 _stdPos;
        public Quaternion _stdRotation;


        [Header("ControlableByScript")] public bool controlableByScript;
        public bool hasScale = false;
        public bool hasMovement = false;
        public bool hasColor;
        public bool hasRotation;

        bool _isTransform;

        // Start is called before the first frame update
        bool first = true;

        public void Awake()
        {
            if (_useDefaultClickAnim)
            {
                _clickAnim = _defaultClickAnim.AnimationsSequence[0];
            }
            if (_useViaObjectAnim)
            {
                _animationsSequence = _animationViaObject.AnimationsSequence;
            }
            foreach (AnimationsSimultanious a in _animationsSequence)
            {
                foreach (Animations anim in a.animationsSimultanious)
                {
                    if (anim.isIncrimental)
                    {
                        anim.SetIncrement(anim.changeVector);
                    }
                }
            }
            _rectTransform = GetComponent<RectTransform>();
            _transform = GetComponent<Transform>();
            _Seq = DOTween.Sequence();
            if (first)
            {
                if (_rectTransform)
                {
                    _isTransform = false;
                }
                else if (_transform)
                {
                    _isTransform = true;
                }

                first = false;
                if (_stdPos == Vector3.zero)
                {
                    if (_isTransform)
                    {
                        _stdPos = _transform.localPosition;
                    }
                    else
                    {
                        _stdPos = _rectTransform.anchoredPosition;
                    }
                }

                if (_stdScale == Vector3.zero)
                {
                    if (_isTransform)
                    {
                        _stdScale = _transform.localScale;
                    }
                    else
                    {
                        _stdScale = _rectTransform.localScale;
                    }
                }

                if (_stdRotation == new Quaternion())
                {
                    if (_isTransform)
                    {
                        _stdRotation = _transform.localRotation;
                    }
                    else
                    {
                        _stdRotation = _rectTransform.localRotation;
                    }
                }
            }
        }

        public void ClickAnim()
        {
            /*if (_isTransform)
            {
                _transform = GetComponent<Transform>();
                _transform.DOKill();

                _transform.localScale = _stdScale;
                _transform.localPosition = _stdPos;
                _transform.localRotation = _stdRotation;
            }
            else
            {
                _rectTransform = GetComponent<RectTransform>();
                _rectTransform.DOKill();

                _rectTransform.anchoredPosition = _stdPos;
                _rectTransform.localScale = _stdScale;
                _rectTransform.localRotation = _stdRotation;
            }
            */

            bool first = true;
            foreach (Animations anim in _clickAnim.animationsSimultanious)
            {
                if (anim.animType == AnimTypes.scale)
                {
                    _Seq.Join(ScaleTween(anim));
                }
                else if (anim.animType == AnimTypes.movement)
                {
                    _Seq.Join(TransformTween(anim));
                }
                else if (anim.animType == AnimTypes.rotation)
                {
                    _Seq.Join(RotateTween(anim));
                }
                else if (_isTransform == false && anim.animType == AnimTypes.color)
                {
                    if (_image == null)
                    {
                        _image = GetComponent<Image>();
                    }

                    _Seq.Join(ColorTween(anim));
                }
                else if (anim.animType == AnimTypes.restartAnim)
                {
                    StartCoroutine(RestartAnim(anim.delay + anim.duration));
                }
            }

        }

        private void OnEnable()
        {
            StopAllCoroutines();
            if (!controlableByScript)
            {
                if (_isTransform)
                {
                    _transform = GetComponent<Transform>();
                    _transform.DOKill();

                    _transform.localScale = _stdScale;
                    _transform.localPosition = _stdPos;
                    _transform.localRotation = _stdRotation;
                }
                else
                {
                    _rectTransform = GetComponent<RectTransform>();
                    _rectTransform.DOKill();

                    _rectTransform.anchoredPosition = _stdPos;
                    _rectTransform.localScale = _stdScale;
                    _rectTransform.localRotation = _stdRotation;
                }

                StartCoroutine(WaitFrame());
            }
        }

        private bool _appearAnimFinished = true;

        public void PlayAnim()
        {
            if (this != null)
                FillFields();
        }

        private void FillFields()
        {
            if (controlableByScript) //&& gameObject.activeInHierarchy)
            {
                if (_isTransform)
                {
                    _transform = GetComponent<Transform>();
                    _transform.DOKill();

                    if (hasScale)
                    {
                        _transform.localScale = _stdScale;
                    }

                    if (hasMovement)
                    {
                        _transform.localPosition = _stdPos;
                    }

                    if (hasRotation)
                    {
                        _transform.localRotation = _stdRotation;
                    }
                }
                else
                {
                    _rectTransform = GetComponent<RectTransform>();
                    _rectTransform.DOKill();

                    if (hasMovement)
                    {
                        _rectTransform.anchoredPosition = _stdPos;
                    }

                    if (hasScale)
                    {
                        _rectTransform.localScale = _stdScale;
                    }

                    if (hasRotation)
                    {
                        _rectTransform.localRotation = _stdRotation;
                    }
                }

                SetUpAnimationSequence();
            }
        }
        private void FillFields(int n)
        {
            if (controlableByScript) //&& gameObject.activeInHierarchy)
            {
                if (_isTransform)
                {
                    _transform = GetComponent<Transform>();
                    _transform.DOKill();

                    if (hasScale)
                    {
                        _transform.localScale = _stdScale;
                    }

                    if (hasMovement)
                    {
                        _transform.localPosition = _stdPos;
                    }

                    if (hasRotation)
                    {
                        _transform.localRotation = _stdRotation;
                    }
                }
                else
                {
                    _rectTransform = GetComponent<RectTransform>();
                    _rectTransform.DOKill();

                    if (hasMovement)
                    {
                        _rectTransform.anchoredPosition = _stdPos;
                    }

                    if (hasScale)
                    {
                        _rectTransform.localScale = _stdScale;
                    }

                    if (hasRotation)
                    {
                        _rectTransform.localRotation = _stdRotation;
                    }
                }

                SetUpAnimationSequence(n);
            }
        }

        public void PlayAnim(int n)
        {
            if (gameObject != null)
                FillFields(n);
        }

        public void StopAnim()
        {
            FillFields();
        }


        private void OnDisable()
        {
            _Seq.Complete();
        }



        IEnumerator WaitFrame()
        {
            yield return new WaitUntil(() => _appearAnimFinished);
            SetUpAnimationSequence();
        }

        float duration = 0;

        private void SetUpAnimationSequence()
        {

            foreach (AnimationsSimultanious animS in _animationsSequence)
            {
                bool first = true;
                foreach (Animations anim in animS.animationsSimultanious)
                {
                    if (anim.animType == AnimTypes.scale)
                    {
                        _Seq.Join(ScaleTween(anim));
                    }
                    else if (anim.animType == AnimTypes.movement)
                    {
                        _Seq.Join(TransformTween(anim));
                    }
                    else if (anim.animType == AnimTypes.rotation)
                    {
                        _Seq.Join(RotateTween(anim));
                    }
                    else if (_isTransform == false && anim.animType == AnimTypes.color)
                    {
                        if (_image == null)
                        {
                            _image = GetComponent<Image>();
                        }

                        _Seq.Join(ColorTween(anim));
                    }
                    else if (anim.animType == AnimTypes.restartAnim)
                    {
                        StartCoroutine(RestartAnim(anim.delay + anim.duration));
                    }

                }
                
            }
        }

        private void SetUpAnimationSequence(int n)
        {
            if (_animationsSequence[n].startScale != Vector3.zero && hasScale)
            {

                if (_isTransform)
                {
                    _transform.localScale = _animationsSequence[n].startScale;
                }
                else
                {
                    _rectTransform.localScale = _animationsSequence[n].startScale;
                }

            }

            foreach (Animations anim in (_animationsSequence[n].animationsSimultanious))
            {
                if (anim.animType == AnimTypes.scale)
                {
                    _Seq.Join(ScaleTween(anim));
                }
                else if (anim.animType == AnimTypes.movement)
                {
                    _Seq.Join(TransformTween(anim));
                }
                else if (anim.animType == AnimTypes.rotation)
                {
                    _Seq.Join(RotateTween(anim));
                }
                else if (_isTransform == false && anim.animType == AnimTypes.color)
                {
                    if (_image == null)
                    {
                        _image = GetComponent<Image>();
                    }

                    _Seq.Join(ColorTween(anim));
                }
            }
        }

        private IEnumerator RestartAnim(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            OnEnable();
        }
        private Tween ScaleTween(Animations anim)
        {
            if (_isTransform)
            {
                Tween t = _transform.DOScale(anim.changeVector, anim.duration).SetEase(anim.Ease).SetDelay(anim.delay)
                    .SetLoops(anim.loopsAmoungt, anim.loopType);
                if (anim.isIncrimental)
                    anim.changeVector += anim.increment;
                ;
                return t;
            }
            else
            {
                Tween t = _rectTransform.DOScale(anim.changeVector, anim.duration).SetEase(anim.Ease).SetDelay(anim.delay)
                    .SetLoops(anim.loopsAmoungt, anim.loopType);
                if (anim.isIncrimental)
                    anim.changeVector += anim.increment;
                return t;
            }
        }

        private Tween TransformTween(Animations anim)
        {
            if (_isTransform)
            {
                Tween t = _transform.DOLocalMove(anim.changeVector, anim.duration).SetEase(anim.Ease).SetDelay(anim.delay)
                    .SetLoops(anim.loopsAmoungt, anim.loopType);
                if (anim.isIncrimental)
                    anim.changeVector += anim.increment;
                return t;
            }
            else
            {
                Tween t = _rectTransform.DOAnchorPos(anim.changeVector, anim.duration).SetEase(anim.Ease)
                    .SetDelay(anim.delay).SetLoops(anim.loopsAmoungt, anim.loopType);
                if (anim.isIncrimental)
                    anim.changeVector += anim.increment;
                return t;
            }
        }

        private Tween RotateTween(Animations anim)
        {
            if (_isTransform)
            {
                Tween t = _transform.DOLocalRotate(anim.changeVector, anim.duration, RotateMode.FastBeyond360).SetEase(anim.Ease)
                    .SetDelay(anim.delay)
                    .SetLoops(anim.loopsAmoungt, anim.loopType);
                if (anim.isIncrimental)
                    anim.changeVector += anim.increment;
                return t;
            }
            else
            {
                Tween t = _rectTransform.DOLocalRotate(anim.changeVector, anim.duration, RotateMode.FastBeyond360).SetEase(anim.Ease)
                    .SetDelay(anim.delay).SetLoops(anim.loopsAmoungt, anim.loopType);
                if (anim.isIncrimental)
                    anim.changeVector += anim.increment;
                return t;
            }
        }

        private Tween ColorTween(Animations anim)
        {
            _image.color = anim.startColor;
            return _image.DOColor(anim.endColor, anim.duration).SetEase(anim.Ease).SetDelay(anim.delay)
                .SetLoops(anim.loopsAmoungt, anim.loopType);
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}