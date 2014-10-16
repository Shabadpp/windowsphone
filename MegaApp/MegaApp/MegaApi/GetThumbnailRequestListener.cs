﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Windows.Storage;
using mega;
using MegaApp.Classes;
using MegaApp.Enums;
using MegaApp.Extensions;
using MegaApp.Models;
using MegaApp.Resources;
using MegaApp.Services;

namespace MegaApp.MegaApi
{
    class GetThumbnailRequestListener: BaseRequestListener
    {
        private readonly NodeViewModel _node;
        public GetThumbnailRequestListener(NodeViewModel node)
        {
            this._node = node;
        }

        #region Base Properties

        protected override string ProgressMessage
        {
            get { throw new NotImplementedException(); }
        }

        protected override bool ShowProgressMessage
        {
            get { return false; }
        }

        protected override string ErrorMessage
        {
            get { return AppMessages.GetThumbnailFailed; }
        }

        protected override string ErrorMessageTitle
        {
            get { return AppMessages.GetThumbnailFailed_Title; }
        }

        protected override string SuccessMessage
        {
            get { throw new NotImplementedException(); }
        }

        protected override string SuccessMessageTitle
        {
            get { throw new NotImplementedException(); }
        }

        protected override bool ShowSuccesMessage
        {
            get { return false; }
        }

        protected override bool NavigateOnSucces
        {
            get { return false; }
        }

        protected override bool ActionOnSucces
        {
            get { return true; }
        }

        protected override Type NavigateToPage
        {
            get { throw new NotImplementedException(); }
        }

        protected override NavigationParameter NavigationParameter
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region Override Methods

        protected override void OnSuccesAction(MRequest request)
        {
            _node.LoadThumbnailImage(request.getFile());
        }

        #endregion

    }
}
