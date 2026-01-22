using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TMM
{
	public class ClownParty : MonoBehaviour
	{
		[SerializeField]
		List<WinnerLoserClown> clowns;

        private void Awake()
        {
            List<int> types = new List<int>();
            for(int i = 0; i<=17; i++)
            {
                types.Add(i);
            }
            foreach(var clown in clowns)
            {
                int index = Random.Range(0, types.Count);
                clown.SetType(types[index]);
                types.RemoveAt(index);
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
