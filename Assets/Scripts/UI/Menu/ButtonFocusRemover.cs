using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TMM.UI
{
    public class ButtonFocusRemover : MonoBehaviour
    {
        Button button;

        void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(() => { RemoveFocus().Forget(); });
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        async UniTaskVoid RemoveFocus()
        {
            await UniTask.Delay(100);
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
