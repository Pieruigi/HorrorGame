using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace TMM
{
	public class MemoryTile : MonoBehaviour
	{
		[SerializeField]
		GameObject model;

	

		bool shaking = false;

		bool selected = false;
		public bool IsSelected
		{
			get { return selected; }
		}

		Memory memory;

		

		// Start is called before the first frame update
		void Start()
	    {
			memory = transform.root.GetComponentInChildren<Memory>();
	    }

		// Update is called once per frame
		void Update()
		{

		}

		public void Shake(bool value)
		{
			
			//Debug.Log("Shake " + gameObject.name);
			if (value == shaking) return;
			if (value && selected) return;
		
			shaking = value;

			model.transform.DOKill();

			if (value)
				model.transform.DOShakeRotation(.25f).OnComplete(() => { model.transform.rotation = Quaternion.identity; }).SetLoops(-1);
			else
				model.transform.localRotation = Quaternion.identity;


		}

		public void Select(bool value)
		{
			if (selected == value) return;

			model.transform.DOKill();

			selected = value;
			if (selected)
			{
				Shake(false);
				// Rotate
				shaking = false;
				model.transform.DOLocalRotate(Vector3.up * 180, .25f);
			}
			else
			{
				// Rotate back
				model.transform.DOLocalRotate(Vector3.zero, .25f);
			}

			PlaySwoosh();

		}
		
		void PlaySwoosh()
        {
			memory.PlaySwoosh();
        }
		

		
		
	}
}
