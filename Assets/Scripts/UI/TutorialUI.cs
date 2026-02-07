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

        private const string PosTextStart = "You're on the ";
        private const string RightText = "RIGHT";
        private const string LeftText = "LEFT";
        
        [SerializeField] private UIDocument tutorialUI;
        
        [SerializeField] private float delayBetweenLetters = 0.1f;
        [SerializeField] private float delayBeforeEndText = 0.5f;
        [SerializeField] private float delayAfterTextRenderedSeconds = 3f;
        
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
            var endText = type == PlayerManager.PlayerType.Left ? LeftText : RightText;
            
            var text = pos.Q<Label>();
            text.text = "";
            pos.visible = true;
            var waitBetween = new WaitForSeconds(delayBetweenLetters);
            foreach (var c in PosTextStart)
            {
                yield return waitBetween;
                text.text += c;
            }

            yield return new WaitForSeconds(delayBeforeEndText);
            text.text += endText;
            
            yield return new WaitForSeconds(delayAfterTextRenderedSeconds);
            pos.visible = false;
        }
    }
}
