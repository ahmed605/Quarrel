﻿using DiscordAPI.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quarrel.Models.Bindables.Abstract;

namespace Quarrel.Models.Bindables
{
    public class BindableChannel : BindableModelBase<Channel>
    {
        public BindableChannel([NotNull] Channel model) : base(model) { }
    }
}
