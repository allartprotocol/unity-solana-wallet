using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using AllArt.Solana;
using Solnet.Rpc.Models;
using Solnet.Rpc.Core.Http;

namespace AllArt.Solana.Example
{
    public class TransferScreen : Screen
    {
        public TextMeshProUGUI ownedAmmount_txt;
        public TextMeshProUGUI error_txt;
        public TMP_InputField toPublic_txt;
        public TMP_InputField ammount_txt;
        public Button transfer_btn;

        public Button close_btn;

        private TokenAccount transferTokenAccount;
        private double ownedSolAmmount;

        private void Start()
        {
            //toPublic_txt.text = "FEbviHLML4a98Y4iDCAS9aEbBqgdNS6DTP4soRipDGQc";
            transfer_btn.onClick.AddListener(() =>
            {
                TryTransfer();
            });

            close_btn.onClick.AddListener(() =>
            {
               manager.ShowScreen(this, "wallet_screen");
            });
        }

        private void TryTransfer()
        {
            if (transferTokenAccount == null)
            {
                if (CheckInput())
                    TransferSol();
            }
            else
            {
                if (CheckInput())
                    TransferToken();
            }
        }

        private async void TransferSol()
        {
            RequestResult<string> result = await SimpleWallet.instance.TransferSol(toPublic_txt.text, long.Parse(ammount_txt.text));
            HandleResponse(result);
        }

        bool CheckInput() {
            if (string.IsNullOrEmpty(ammount_txt.text))
            {
                error_txt.text = "Please input transfer ammount";
                return false;
            }

            if (string.IsNullOrEmpty(toPublic_txt.text))
            {
                error_txt.text = "Please enter receiver public key";
                return false;
            }

            if (transferTokenAccount == null)
            {
                if (long.Parse(ammount_txt.text) > (long)(ownedSolAmmount * 1000000000))
                {
                    error_txt.text = "Not enough funds for transaction.";
                    return false;
                }
            }
            else {
                if (long.Parse(ammount_txt.text) > long.Parse(ownedAmmount_txt.text))
                {
                    error_txt.text = "Not enough funds for transaction.";
                    return false;
                }
            }
            error_txt.text = "";
            return true;
        }

        private async void TransferToken()
        {
            RequestResult<string> result = await SimpleWallet.instance.TransferToken(
                                transferTokenAccount.pubkey,
                                toPublic_txt.text,
                                SimpleWallet.instance.wallet.GetAccount(0),
                                transferTokenAccount.Account.Data.Parsed.Info.Mint,
                                long.Parse(ammount_txt.text));

            HandleResponse(result);
        }

        private void HandleResponse(RequestResult<string> result)
        {
            if (result.Result == null)
            {
                error_txt.text = result.Reason;
            }
            else
                error_txt.text = "";
        }

        public async override void ShowScreen(object data = null)
        {
            base.ShowScreen();

            ResetInputFileds();
            await PopulateInfoFileds(data);
            gameObject.SetActive(true);
        }

        private async System.Threading.Tasks.Task PopulateInfoFileds(object data)
        {
            if (data != null && data.GetType().Equals(typeof(TokenAccount)))
            {
                this.transferTokenAccount = (TokenAccount)data;
                ownedAmmount_txt.text = $"{transferTokenAccount.Account.Data.Parsed.Info.TokenAmount.Amount}";
            }
            else
            {
                ownedSolAmmount = await SimpleWallet.instance.GetSolAmmount(SimpleWallet.instance.wallet.GetAccount(0));
                ownedAmmount_txt.text = $"{ownedSolAmmount}";
            }
        }

        private void ResetInputFileds()
        {
            error_txt.text = "";
            ammount_txt.text = "";
            toPublic_txt.text = "";
        }

        public override void HideScreen()
        {
            base.HideScreen();
            this.transferTokenAccount = null;
            gameObject.SetActive(false);
        }
    }

}