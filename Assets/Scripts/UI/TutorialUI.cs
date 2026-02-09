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
            public readonly VisualElement PaddleControls;
            public readonly Image MouseImage;
            public readonly VisualElement CameraControls;
            public readonly Image CameraImage;

            public Elements(UIDocument document)
            {
                Root = document.rootVisualElement;
                Position = Root.Q<VisualElement>("Position");
                
                PaddleControls = Root.Q<VisualElement>("PaddleControls");
                MouseImage = PaddleControls.Q<Image>("Image");
                
                CameraControls = Root.Q<VisualElement>("CameraControls");
                CameraImage = CameraControls.Q<Image>("Image");
            }
        }

        private const string PosTextStart = "You're on the ";
        private const string RightText = "RIGHT";
        private const string LeftText = "LEFT";
        
        [SerializeField] private UIDocument tutorialUI;
        
        [SerializeField] private float delayBetweenLetters = 0.1f;
        [SerializeField] private float delayBeforeEndText = 0.5f;
        [SerializeField] private float delayAfterTextRenderedSeconds = 3f;
        [SerializeField] private float displayPaddleControlsTime = 10f;
        [SerializeField] private float displayCameraControlsTime = 5f;
        
        private Elements e;

        public override void OnStartClient()
        {
            tutorialUI.enabled = true;
            e ??= new Elements(tutorialUI);
            ResetUI();
            PlayerManager.OnLocalPlayerAssigned(OnPlayerAssigned);
        }

        public override void OnStopClient()
        {
            PlayerManager.Unsubscribe(OnPlayerAssigned);
            StopAllCoroutines();
            ResetUI();
        }

        private void ResetUI()
        {
            e.Position.visible = false;
            e.PaddleControls.visible = false;
            e.CameraControls.visible = false;
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
            
            yield return new WaitForSeconds(0.5f);
            yield return DisplayControls();
        }

        private IEnumerator DisplayControls()
        {
            const float distance = 100f;
            const float speed = 100f;

            e.PaddleControls.visible = true;
            var initialPos = 0f;
            var currentPos = 0f;
            var dir = 1;
            var currentTime = 0f;
            
            // animate mouse
            while (e.PaddleControls.visible && currentTime < displayPaddleControlsTime)
            {
                currentTime += Time.deltaTime;
                e.MouseImage.style.top = new StyleLength(Length.Pixels(initialPos + currentPos));
                currentPos += speed * dir * Time.deltaTime;
                if (currentPos >= distance)
                {
                    currentPos = distance;
                    dir = -1;
                }
                else if (currentPos <= 0)
                {
                    currentPos = 0;
                    dir = 1;
                }
                yield return null;
            }
            
            e.PaddleControls.visible = false;
            yield return new WaitForSeconds(0.5f);
            yield return DisplayCameraControls();
        }

        private IEnumerator DisplayCameraControls()
        {
            e.CameraControls.visible = true;
            yield return new WaitForSeconds(displayCameraControlsTime);
            e.CameraControls.visible = false;
        }
    }
}
