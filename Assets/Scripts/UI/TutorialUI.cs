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
            public readonly VisualElement Position;

            public Elements(UIDocument document)
            {
                Root = document.rootVisualElement;
                Position = Root.Q<VisualElement>("Position");
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
                Position = { visible = false },
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
            var endText = type == PlayerManager.PlayerType.Left ? LeftText : RightText;
            if (type == PlayerManager.PlayerType.Right)
                e.Position.style.right = new StyleLength(0f);
            else
                e.Position.style.right = new StyleLength(StyleKeyword.Auto);
            
            var text = e.Position.Q<Label>();
            text.text = "";
            e.Position.visible = true;
            var waitBetween = new WaitForSeconds(delayBetweenLetters);
            foreach (var c in PosTextStart)
            {
                yield return waitBetween;
                text.text += c;
            }

            yield return new WaitForSeconds(delayBeforeEndText);
            text.text += endText;
            
            yield return new WaitForSeconds(delayAfterTextRenderedSeconds);
            e.Position.visible = false;
        }
    }
}
