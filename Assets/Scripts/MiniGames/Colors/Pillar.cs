using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMM
{
    public class Pillar : MonoBehaviour
    {
        [SerializeField]
        Branch[] branches = new Branch[4];

        [SerializeField]
        Material[] materials = new Material[4];

        [SerializeField]
        Collider boundary;
        public Collider Boundary
        {
            get { return boundary; }
        }

        AudioSource audioSource;

        private void Awake()
        {
            // Get the audio source
            audioSource = GetComponent<AudioSource>();

            foreach (Branch branch in branches)
                branch.gameObject.SetActive(false);
        }

        // Start is called before the first frame update
        void Start()
        {
            // Fill branches
            //branches = GetComponentsInChildren<Branch>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        
        public bool HasBranch(int index)
        {
            return branches[index].gameObject.activeSelf;
        }

        public void RemoveBranch(int index)
        {
            branches[index].gameObject.SetActive(false);
            foreach(Branch branch in branches)
            {
                if (branch.gameObject.activeSelf)
                    return;
            }

            gameObject.SetActive(false);
        }

        public void AddBranch(int index)
        {
            branches[index].gameObject.SetActive(true);
        }

        public Branch GetBranch(int index)
        {
            return branches[index];
        }

        public Material GetMaterial(int index)
        {
            return materials[index];
        }

        public void PlayClip()
        {
            audioSource.Play();
        }

        public void StartShaking()
        {

        }
        
        public void StopShaking()
        {
            
        }

    }

}
