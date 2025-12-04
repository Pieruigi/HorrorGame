using UnityEngine;

namespace TMM
{
	public class LightObject : MonoBehaviour
	{
		[SerializeField]
		Light _light;

		[SerializeField]
		float rangeMin = 8;

		[SerializeField]
		float rangeMax = 12;

		[SerializeField]
		float powerMin = 1.6f;

		[SerializeField]
		float powerMax = 2.4f;

		FlickeringLight flickeringLight;

        void Awake()
        {
			flickeringLight = _light.GetComponent<FlickeringLight>();

			float power = Random.Range(powerMin, powerMax);
			float range = Random.Range(rangeMin, rangeMax);

			_light.range = range;

			

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
