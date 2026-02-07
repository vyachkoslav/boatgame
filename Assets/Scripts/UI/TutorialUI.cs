using System.Collections;
using FishNet.Object;
using Network;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace UI
{
    public class TutorialUI : NetworkBehaviour
    {
        private class Elements
        {
            public readonly VisualElement Root;
            public readonly VisualElement RightPos;
            public readonly VisualElement LeftPos;

            public Elements(UIDocument document)
            {
                Root = document.rootVisualElement;
                RightPos = Root.Q<VisualElement>("Right");
                LeftPos = Root.Q<VisualElement>("Left");
            }
        }
        
        [SerializeField] private UIDocument tutorialUI;
        private Elements e;

        public override void OnStartClient()
        {
            tutorialUI.enabled = true;
            e = new Elements(tutorialUI)
            {
                LeftPos = { visible = false },
                RightPos = { visible = false }
            };
            PlayerManager.OnLocalPlayerAssigned(OnPlayerAssigned);
        }

        private void OnDestroy()
        {
            PlayerManager.Unsubscribe(OnPlayerAssigned);
        }

        private void OnPlayerAssigned(PlayerManager.PlayerType type)
        {
            Assert.IsFalse(type == PlayerManager.PlayerType.None);
            if (type == PlayerManager.PlayerType.Spectator) return;
            
            StartCoroutine(ShowPositionRoutine(type));
        }

        private IEnumerator ShowPositionRoutine(PlayerManager.PlayerType type)
        {
            var pos = type == PlayerManager.PlayerType.Left ? e.LeftPos : e.RightPos;
            pos.visible = true;
            yield return new WaitForSeconds(5f);
            pos.visible = false;
        }
    }
}
