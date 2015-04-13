﻿using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using Windows.Storage;
using mega;
using MegaApp.Classes;
using MegaApp.Enums;
using MegaApp.Interfaces;
using MegaApp.MegaApi;
using MegaApp.Models;
using MegaApp.Resources;
using MegaApp.Services;

namespace MegaApp.ViewModels
{
    class ImageNodeViewModel: FileNodeViewModel
    {
        public ImageNodeViewModel(MegaSDK megaSdk, AppInformation appInformation, MNode megaNode,
            ObservableCollection<IMegaNode> parentCollection = null, ObservableCollection<IMegaNode> childCollection = null)
            : base(megaSdk, appInformation, megaNode, parentCollection, childCollection)
        {
            // Image node downloads to the image path of the full original image
            this.Transfer = new TransferObjectModel(MegaSdk, this, TransferType.Download, LocalImagePath);

            this.IsDownloadAvailable = File.Exists(LocalImagePath);

            // Default false for preview slide
            InViewingRange = false;
        }

        #region Override Methods

        public override async void Open()
        {
            await FileService.OpenFile(LocalImagePath);
        }

        #endregion

        #region Public Methods

        public bool HasPreviewInCache()
        {
            return FileService.FileExists(PreviewPath);
        }

        public void CancelPreviewRequest()
        {
            MegaSdk.cancelGetPreview(this.OriginalMNode);
            IsBusy = false;
        }

        public void SetPreviewImage()
        {
            if (this.IsBusy) return;
            if (!this.OriginalMNode.hasPreview()) return;

            if (this.OriginalMNode.hasPreview())
            {
                GetPreview();
            }
            else
            {
                GetImage(true);
            }
        }

        public void SetImage()
        {
            if (this.ImageUri != null) return;
            if (this.IsBusy) return;

            GetImage(false);
        }

        public void SaveImageToCameraRoll(bool showMessages = true)
        {
            if (this.ImageUri == null) return;

            if(showMessages)
                if (MessageBox.Show(AppMessages.SaveImageQuestion, AppMessages.SaveImageQuestion_Title,
                        MessageBoxButton.OKCancel) == MessageBoxResult.Cancel) return;

            if (ImageService.SaveToCameraRoll(this.Name, this.ImageUri))
            {
                if (showMessages)
                    MessageBox.Show(AppMessages.ImageSaved, AppMessages.ImageSaved_Title, MessageBoxButton.OK);
            }
            else
                MessageBox.Show(AppMessages.ImageSaveError, AppMessages.ImageSaveError_Title, MessageBoxButton.OK);
        }

        #endregion

        #region Private Methods

        private void GetPreview()
        {
            if (FileService.FileExists(PreviewPath))
            {
                PreviewImageUri = new Uri(PreviewPath);
            }
            else
            {
                this.MegaSdk.getPreview(this.OriginalMNode, PreviewPath, new GetPreviewRequestListener(this));
            }
        }

        private void GetImage(bool isForPreview)
        {
            if (FileService.FileExists(LocalImagePath))
            {
                ImageUri = new Uri(LocalImagePath);

                if (!isForPreview) return;

                PreviewImageUri = new Uri(LocalImagePath);

            }
            else
            {
                if (isForPreview)
                    IsBusy = true;
                Transfer.AutoLoadImageOnFinish = true;
                Transfer.StartTransfer();
            }
        }

        #endregion

        #region Properties

        public bool InViewingRange { get; set; }

        private Uri _previewImageUri;
        public Uri PreviewImageUri
        {
            get
            {
                if (_previewImageUri == null && InViewingRange)
                    SetPreviewImage();
                return _previewImageUri;
            }
            set
            {
                _previewImageUri = value;
                OnPropertyChanged("PreviewImageUri");
            }
        }

        private Uri _imageUri;
        public Uri ImageUri
        {
            get { return _imageUri; }
            set
            {
                _imageUri = value;
                OnPropertyChanged("ImageUri");
            }
        }

        public string PreviewPath
        {
            get
            {
                return Path.Combine(ApplicationData.Current.LocalFolder.Path,
                                    AppResources.PreviewsDirectory,
                                    this.OriginalMNode.getBase64Handle());
            }
        }

        public string LocalImagePath
        {
            get
            {
                return Path.Combine(ApplicationData.Current.LocalFolder.Path,
                                    AppResources.DownloadsDirectory,
                                    String.Format("{0}{1}",
                                        this.OriginalMNode.getBase64Handle(),
                                        Path.GetExtension(base.Name)));
            }
        }

        public string PublicImagePath
        {
            get
            {
                return Path.Combine(AppService.GetSelectedDownloadDirectoryPath(),
                                    String.Format("{0}{1}",
                                        this.OriginalMNode.getBase64Handle(),
                                        Path.GetExtension(base.Name)));
            }
        }

        #endregion
    }
}
