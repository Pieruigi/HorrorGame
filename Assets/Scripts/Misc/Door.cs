using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace TMM
{
    public class Door : MonoBehaviour
    {
        [SerializeField]
        float openAngle = 90;

        [SerializeField]
        float closeAngle = 0;

        [SerializeField]
        float speed = 1;

        [SerializeField]
        bool isOpen = false;

        [SerializeField]
        AudioSource _audio;



        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Z))
                Open();
#endif
        }

        public void Open()
        {
            DOTween.KillAll();

            transform.DORotate(Vector3.up * openAngle, 1f / speed).SetEase(Ease.OutQuad);
 
            _audio.Play();
        }
    }
}