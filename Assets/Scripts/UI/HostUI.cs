using System;
using FishNet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HostUI : MonoBehaviour
    {
        [Header("Host")]
        [SerializeField] private Toggle hostToggle;
        [SerializeField] private Button hostConfirmButton;
        [SerializeField] private TMP_InputField hostPortInput;

        [Header("Join")]
        [SerializeField] private Toggle joinToggle;
        [SerializeField] private Button joinConfirmButton;
        [SerializeField] private TMP_InputField joinHostInput;
        [SerializeField] private TMP_InputField joinPortInput;

        private void Awake()
        {
            hostToggle.onValueChanged.AddListener(toggled => 
            {
                if (toggled) 
                    joinToggle.isOn = false;
            });
            joinToggle.onValueChanged.AddListener(toggled =>
            {
                if (toggled) 
                    hostToggle.isOn = false;
            });

            hostConfirmButton.onClick.AddListener(() =>
            {
                var port = ushort.Parse(hostPortInput.text);
                InstanceFinder.ServerManager.StartConnection(port);
                InstanceFinder.ClientManager.StartConnection("127.0.0.1", port);
            });
            joinConfirmButton.onClick.AddListener(() =>
            {
                var address = joinHostInput.text;
                var port = ushort.Parse(joinPortInput.text);
                InstanceFinder.ClientManager.StartConnection(address, port);
            });
        }
    }
}
