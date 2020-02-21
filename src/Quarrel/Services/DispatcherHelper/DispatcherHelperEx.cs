﻿// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight.Threading;
using Quarrel.ViewModels.Services.DispatcherHelper;
using System;

namespace Quarrel.Services.DispatcherHelperEx
{
    /// <summary>
    /// A wrapper on the <see cref="DispatcherHelper"/> for running tasks on the UI thread.
    /// </summary>
    public class DispatcherHelperEx : IDispatcherHelper
    {
        /// <summary>
        /// Runs <paramref name="action"/> on the UI thread.
        /// </summary>
        /// <param name="action">Action to run.</param>
        public void CheckBeginInvokeOnUi(Action action)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(action);
        }
    }
}
