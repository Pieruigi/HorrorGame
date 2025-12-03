using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TMM
{
    public class Branch : MonoBehaviour
    {
        [SerializeField]
        GameObject connector;

        float connectionDistance = 0.4f;

        int colorId = -1;
        public int ColorId
        {
            get { return colorId; }
            set { colorId = value; SetMaterial(colorId); }
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Connect()
        {
            float y = connector.transform.localPosition.y + connectionDistance;

            connector.transform.DOLocalMoveY(y, 1f).SetEase(Ease.InOutExpo);
        }

        void SetMaterial(int colorId)
        {
            // Get connection renderer
            connector.GetComponent<MeshRenderer>().material = GetComponentInParent<Pillar>().GetMaterial(colorId);
        }


    }

}
