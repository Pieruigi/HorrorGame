using UnityEngine;

namespace TMM.Scriptables
{
	public class ItemAsset : ScriptableObject
	{
		public const string ResourceFolder = "Items";

		[SerializeField]
		string displayName;

		[SerializeField]
		Sprite icon;

		[SerializeField]
		GameObject sceneObject;
		
	}
}
