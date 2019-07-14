﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Quarrel.Messages.Navigation.SubFrame;
using Quarrel.Services;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.Messages.Navigation;
using Quarrel.SubPages;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.Services.Cache;
using Quarrel.Services.Rest;
using Quarrel.Services.Users;
using Quarrel.Services.Voice;
using Quarrel.Services.Voice.Audio.In;
using Quarrel.Services.Voice.Audio.Out;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls.Shell
{
    public sealed partial class Shell : UserControl
    {
        //private IDiscordService discordService = SimpleIoc.Default.GetInstance<IDiscordService>();
       // private ICacheService cacheService = SimpleIoc.Default.GetInstance<ICacheService>();
        public Shell()
        {
            this.InitializeComponent();

            // Setup SideDrawer
            ContentContainer.SetupInteraction();

            Messenger.Default.Register<GuildNavigateMessage>(this, m =>
            {
                ContentContainer.OpenLeft();
            });

            Messenger.Default.Register<ChannelNavigateMessage>(this, m =>
            {
                ContentContainer.CloseLeft();
            });

          //  Login();
        }

        private bool IsViewLarge
        {
            get => UISize.CurrentState == Large || UISize.CurrentState == ExtraLarge;
        }
/*
        public async void Login()
        {
            var token = (string)(await cacheService.Persistent.Roaming.TryGetValueAsync<object>(Quarrel.Helpers.Constants.Cache.Keys.AccessToken));
            if (string.IsNullOrEmpty(token))
            {
                await Task.Delay(100);
                Messenger.Default.Send(SubFrameNavigationRequestMessage.To(new LoginPage()));
            }
            else
            {
                discordService.Login(token);
            }
        }*/

        private void UISize_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            this.Bindings.Update();
        }

        private void HamburgerClicked(object sender, EventArgs e)
        {
            ContentContainer.ToggleLeft();
        }

        private void MemberListButtonClicked(object sender, EventArgs e)
        {
            ContentContainer.ToggleRight();
        }
    }
}