using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TMM
{
	public class RandomDestroyer : MonoBehaviour
	{
		[SerializeField]
		List<GameObject> objects;

		[SerializeField]
		[Range(0f,1f)]
		float rate;

        void Awake()
        {
			int count = Mathf.RoundToInt(((float)objects.Count) * rate);

			for (int i = 0; i < count; i++)
			{
				int index = Random.Range(0, objects.Count);
				var obj = objects[index];
				objects.Remove(obj);
				Destroy(obj);
			}
        }

        // Start is called before the first frame update
        void Start()
	    {
	        
	    }

	    // Update is called once per frame
	    void Update()
	    {
	        
	    }
	}
}
