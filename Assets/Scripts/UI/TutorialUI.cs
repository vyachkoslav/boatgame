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
            public readonly Label PaddleControlsDescription;
            
            public readonly VisualElement CameraControls;
            public readonly Image CameraImage;
            public readonly Label CameraControlsDescription;

            public Elements(UIDocument document)
            {
                Root = document.rootVisualElement;
                Position = Root.Q<VisualElement>("Position");
                
                PaddleControls = Root.Q<VisualElement>("PaddleControls");
                MouseImage = PaddleControls.Q<Image>("Image");
                PaddleControlsDescription = PaddleControls.Q<Label>("Description");
                
                CameraControls = Root.Q<VisualElement>("CameraControls");
                CameraImage = CameraControls.Q<Image>("Image");
                CameraControlsDescription = CameraControls.Q<Label>("Description");
            }
        }

        private const string PosTextStart = "You're on the ";
        private const string RightText = "<b>RIGHT</b>";
        private const string LeftText = "<b>LEFT</b>";
        
        [SerializeField] private UIDocument tutorialUI;
        
        [SerializeField] private float delayBetweenLetters = 0.1f;
        [SerializeField] private float delayBetweenDescriptionLetters = 0.03f;
        [SerializeField] private float delayBeforeEndText = 0.5f;
        [SerializeField] private float delayAfterTextRenderedSeconds = 3f;
        [SerializeField] private float displayPaddleControlsTime = 10f;
        [SerializeField] private float displayCameraControlsTime = 5f;
        
        private Elements e;
        private static TutorialUI instance;

        public bool IsTutorialComplete { get; private set; }

        private void Awake()
        {
            Assert.IsNull(instance);
            instance = this;
        }

        public override void OnStopClient()
        {
            StopAllCoroutines();
            ResetUI();
        }

        public static void StartTutorial()
        {
            // show only in first section and don't repeat without load to first section
            if (instance.e != null || Checkpoints.LoadedCheckpoint) return;
            instance.tutorialUI.enabled = true;
            instance.e = new Elements(instance.tutorialUI);
            instance.ResetUI();
            instance.StartCoroutine(instance.ShowPositionRoutine(PlayerManager.Instance.CurrentPlayer));
        }

        private void ResetUI()
        {
            if (e == null) return;
            e.Position.visible = false;
            e.PaddleControls.visible = false;
            e.CameraControls.visible = false;
        }

        private IEnumerator ShowPositionRoutine(PlayerManager.PlayerType type)
        {
            var endText = type == PlayerManager.PlayerType.Left ? LeftText : RightText;
            if (type == PlayerManager.PlayerType.Right)
                e.Position.style.right = new StyleLength(0f);
            else
                e.Position.style.right = new StyleLength(StyleKeyword.Auto);
            
            e.Position.visible = true;

            var text = e.Position.Q<Label>();
            yield return DrawText(text, PosTextStart, delayBetweenLetters);
            
            yield return new WaitForSeconds(delayBeforeEndText);
            text.text += endText;

            yield return new WaitForSeconds(delayAfterTextRenderedSeconds);
            e.Position.visible = false;
            
            yield return new WaitForSeconds(0.5f);
            yield return DisplayControls();
        }

        private IEnumerator DisplayControls()
        {
            e.PaddleControls.visible = true;
            StartCoroutine(DrawText(
                e.PaddleControlsDescription, 
                e.PaddleControlsDescription.text,
                delayBetweenDescriptionLetters));
            yield return AnimateMouse();
            e.PaddleControls.visible = false;
            yield return new WaitForSeconds(0.5f);
            yield return DisplayCameraControls();
            IsTutorialComplete = true;
        }

        private IEnumerator DrawText(Label label, string text, float letterDelay)
        {
            var waitBetween = new WaitForSeconds(letterDelay);
            var tag = false;
            label.text = "";
            foreach (var c in text)
            {
                if (!tag)
                    yield return waitBetween;
                
                label.text += c;
                if (c == '<')
                    tag = true;
                else if (c == '>')
                    tag = false;
            }
        }
        
        private IEnumerator AnimateMouse()
        {
            const float distance = 100f;
            const float speed = 100f;

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
        }

        private IEnumerator DisplayCameraControls()
        {
            e.CameraControls.visible = true;
            StartCoroutine(DrawText(
                e.CameraControlsDescription,
                e.CameraControlsDescription.text,
                delayBetweenDescriptionLetters));
            yield return new WaitForSeconds(displayCameraControlsTime);
            e.CameraControls.visible = false;
        }
    }
}
