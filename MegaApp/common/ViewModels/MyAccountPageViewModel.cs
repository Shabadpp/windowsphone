﻿using System.Windows;
using mega;
using MegaApp.Classes;
using MegaApp.Enums;
using MegaApp.MegaApi;
using MegaApp.Resources;
using MegaApp.Services;

namespace MegaApp.ViewModels
{
    public class MyAccountPageViewModel : BaseAppInfoAwareViewModel
    {
        public MyAccountPageViewModel(MegaSDK megaSdk, AppInformation appInformation)
            : base(megaSdk, appInformation)
        {
            InitializeMenu(HamburgerMenuItemType.MyAccount);

            UpdateUserData();

            IsAccountUpdate = false;
        }

        #region Methods

        public void SetOfflineContentTemplate()
        {
            OnUiThread(() =>
            {
                this.EmptyContentTemplate = (DataTemplate)Application.Current.Resources["OfflineEmptyContent"];
                this.EmptyInformationText = UiResources.NoInternetConnection.ToLower();
            });
        }

        public void Initialize(GlobalListener globalListener)
        {
            
        }

        public void Deinitialize(GlobalListener globalListener)
        {
            
        }

        public void GetAccountDetails()
        {
            if(!AccountDetails.IsDataLoaded)
            {
                AccountService.GetAccountDetails();
                MegaSdk.creditCardQuerySubscriptions(new GetAccountDetailsRequestListener());
                AccountDetails.IsDataLoaded = true;
            }            
        }

        public async void GetPricing()
        {
            this.UpgradeAccount.InAppPaymentMethodAvailable = await LicenseService.IsAvailable();
            MegaSdk.getPaymentMethods(new GetPaymentMethodsRequestListener(UpgradeAccount));
            MegaSdk.getPricing(new GetPricingRequestListener());            
        }

        public async void Logout()
        {
            if (await AccountService.ShouldShowPasswordReminderDialogAsync(true))
            {
                DialogService.ShowPasswordReminderDialog(true);
                return;
            }

            MegaSdk.logout(new LogOutRequestListener());
        }

        public void ChangePassword()
        {
            DialogService.ShowChangePasswordDialog();
        }

        public void CancelSubscription()
        {
            DialogService.ShowCancelSubscriptionFeedbackDialog();
        }

        #endregion

        #region Properties

        private DataTemplate _emptyContentTemplate;
        public DataTemplate EmptyContentTemplate
        {
            get { return _emptyContentTemplate; }
            private set { SetField(ref _emptyContentTemplate, value); }
        }

        private string _emptyInformationText;
        public string EmptyInformationText
        {
            get { return _emptyInformationText; }
            private set { SetField(ref _emptyInformationText, value); }
        }

        public UpgradeAccountViewModel UpgradeAccount
        {
            get { return AccountService.UpgradeAccount; }
        }

        private bool _isAccountUpdate;
        public bool IsAccountUpdate
        {
            get { return _isAccountUpdate; }
            set
            {
                _isAccountUpdate = value;
                OnPropertyChanged("IsAccountUpdate");
            }
        }

        #endregion
    }
}
