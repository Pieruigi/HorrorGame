using System.Collections;
using System.Collections.Generic;
using TMM;
using UnityEngine;

public class Trigger1 : MonoBehaviour
{
    [SerializeField]
    CarAlarm carAlarm;

    [SerializeField]
    Door door;

    [SerializeField]
    GameObject eyes;

    void Awake()
    {
        eyes.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        StartCoroutine(Play());
    }

    IEnumerator Play()
    {
        carAlarm.StartAlarm();

        yield return new WaitForSeconds(4f);

        door.Open();

        yield return new WaitForSeconds(3f);

        eyes.SetActive(true);

    }
}
