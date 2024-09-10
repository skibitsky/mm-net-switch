using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using WalletConnectSharp.Common.Model.Errors;
using WalletConnectUnity.Core;
using WalletConnectUnity.Core.Evm;

namespace WalletConnectUnity.Modal.Sample
{
    public class DappButtons : MonoBehaviour
    {
        [SerializeField] private Button _disconnectButton;
        [SerializeField] private Button _personalSignButton;
        [SerializeField] private Button _transactionButton;
        [SerializeField] private Button _netSwitchButton;

        private void Awake()
        {
            WalletConnect.Instance.ActiveSessionChanged += (_, @struct) =>
            {
                _disconnectButton.interactable = true;
                _personalSignButton.interactable = true;
                _transactionButton.interactable = true;
                _netSwitchButton.interactable = true;
            };
        }

        public void OnDisconnectButton()
        {
            Debug.Log("[WalletConnectModalSample] OnDisconnectButton");

            _disconnectButton.interactable = false;
            _personalSignButton.interactable = false;
            _transactionButton.interactable = false;
            _netSwitchButton.interactable = false;

            WalletConnectModal.Disconnect();
        }

        public async void OnPersonalSignButton()
        {
            Debug.Log("[WalletConnectModalSample] OnPersonalSignButton");

            var session = WalletConnect.Instance.ActiveSession;
            var sessionNamespace = session.Namespaces;
            var address = WalletConnect.Instance.SignClient.AddressProvider.CurrentAddress().Address;

            var data = new PersonalSign("Hello world!", address);

            try
            {
                var result = await WalletConnect.Instance.RequestAsync<PersonalSign, string>(data);
                Notification.ShowMessage(
                    $"Received response.\nThis app cannot validate signatures yet.\n\nResponse: {result}");
            }
            catch (WalletConnectException e)
            {
                Notification.ShowMessage($"Personal Sign Request Error: {e.Message}");
                Debug.Log($"[WalletConnectModalSample] Personal Sign Error: {e.Message}");
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
        }

        public async void OnTransactionButton()
        {
            Debug.Log("[WalletConnectModalSample] OnTransactionButton");

            var session = WalletConnect.Instance.ActiveSession;
            var sessionNamespace = session.Namespaces;
            var address = WalletConnect.Instance.SignClient.AddressProvider.CurrentAddress().Address;

            var request = new EthSendTransaction(new Transaction
            {
                from = address,
                to = address,
                value = "0"
            });

            try
            {
                var result = await WalletConnect.Instance.RequestAsync<EthSendTransaction, string>(request);
                Notification.ShowMessage($"Done!\nResponse: {result}");
            }
            catch (WalletConnectException e)
            {
                Notification.ShowMessage($"Transaction Request Error: {e.Message}");
                Debug.Log($"[WalletConnectModalSample] Transaction Error: {e.Message}");
            }
        }

        public async void OnNetworkSwitchButton()
        {
            Debug.Log("[WalletConnectModalSample] OnNetworkSwitchButton");
            
            var chain = ChainConstants.Chains.Arbitrum;
            var ethereumChain = new EthereumChain(chain);
            
            try
            {
                await WalletConnect.Instance.SwitchEthereumChainAsync(ethereumChain);
                var address = WalletConnect.Instance.SignClient.AddressProvider.CurrentAddress().Address;
                Notification.ShowMessage($"Switched to {chain.Name} chain. Current address: {address}");
            }
            catch (WalletConnectException e)
            {
                Notification.ShowMessage($"Switch Chain Request Error: {e.Message}");
                Debug.Log($"[WalletConnectModalSample] Switch Chain Error: {e.Message}");
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
        }
    }
}