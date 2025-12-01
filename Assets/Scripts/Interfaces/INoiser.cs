using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM.Interfaces
{
	public interface INoiser
	{
		float GetNoiseRange();

		Vector3 GetTargetDistance(Vector3 target);
        
	}
}
