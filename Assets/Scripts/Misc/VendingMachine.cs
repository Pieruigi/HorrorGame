using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMM.Scriptables;
using UnityEngine;

namespace TMM
{
	public class VendingMachine : MonoBehaviour
	{
		ItemAsset item;

        void Awake()
        {
			ChooseItem();
        }

        // Start is called before the first frame update
        void Start()
	    {
	        
	    }

		// Update is called once per frame
		void Update()
		{

		}
		
		public void ChooseItem()
        {
			var res = Resources.LoadAll<ItemAsset>(ItemAsset.ResourceFolder).ToList();
			if (item)
				res.Remove(item);
			
			item = res[Random.Range(0, res.Count)];
        }
	}
}
