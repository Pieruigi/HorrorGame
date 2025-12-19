using System.Collections;
using System.Collections.Generic;
using Coffee.UIEffects;
using UnityEngine;
using UnityEngine.UI;

namespace TMM.UI
{
	public class UIEffectMaskRandomizer : MonoBehaviour
	{
		[SerializeField]
		List<Texture> masks;

		

		// Start is called before the first frame update
		void Start()
	    {
			var image = GetComponent<UIEffect>();
			image.detailTexture = masks[Random.Range(0, masks.Count)];

	    }

	    // Update is called once per frame
	    void Update()
	    {
	        
	    }
	}
}
