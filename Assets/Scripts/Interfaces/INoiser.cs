using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM.Interfaces
{
	public interface INoiser
	{
		float GetNoiseRange();

		float GetTargetDistance(Vector3 target);
        
	}
}
