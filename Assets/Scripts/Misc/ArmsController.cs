using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace TMM
{
    public class ArmsController : MonoBehaviour
    {
        [SerializeField]
        Transform target;

        [SerializeField]
        float speed;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void LateUpdate()
        {
            var rot = target.rotation;

            transform.rotation = Quaternion.Slerp(transform.rotation, rot, speed * Time.deltaTime);

            //transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, speed * Time.deltaTime);
        }
    }
    
}
