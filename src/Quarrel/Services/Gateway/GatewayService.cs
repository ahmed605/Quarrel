﻿// Special thanks to Sergio Pedri for the basis of this design

using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using DiscordAPI.Gateway;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.Gateway.DownstreamEvents;
using Quarrel.Messages.Gateway;
using Quarrel.Messages.Navigation;
using DiscordAPI.API;
using DiscordAPI.API.Gateway;
using DiscordAPI.Authentication;
using DiscordAPI.Models;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarSymbols;
using Quarrel.Models.Bindables;
using Quarrel.Messages.Posts.Requests;
using Quarrel.Services.Cache;
using Quarrel.Services.Guild;
using Quarrel.Services.Rest;
using Quarrel.Services.Users;
using UICompositionAnimations.Helpers;

namespace Quarrel.Services.Gateway
{
    public class GatewayService : IGatewayService
    {
        private ICacheService CacheService;
        private ICurrentUsersService CurrentUsersService;
        private IGuildsService GuildsService;
        public DiscordAPI.Gateway.Gateway Gateway { get; private set; }

        public GatewayService(ICacheService cacheService, ICurrentUsersService currentUsersService, IGuildsService guildsService)
        {
            CacheService = cacheService;
            CurrentUsersService = currentUsersService;
            GuildsService = guildsService;
        }

        public async void InitializeGateway([NotNull] string accessToken)
        {
            BasicRestFactory restFactory = new BasicRestFactory();
            IGatewayConfigService gatewayService = restFactory.GetGatewayConfigService();

            GatewayConfig gatewayConfig = await gatewayService.GetGatewayConfig();
            IAuthenticator authenticator = new DiscordAuthenticator(accessToken);

            Gateway = new DiscordAPI.Gateway.Gateway(gatewayConfig, authenticator);


            Gateway.Ready += Gateway_Ready;
            Gateway.GuildMemberChunk += Gateway_GuildMemberChunk;
            Gateway.GuildSynced += Gateway_GuildSynced;

            Gateway.MessageCreated += Gateway_MessageCreated;
            Gateway.MessageDeleted += Gateway_MessageDeleted;
            Gateway.MessageUpdated += Gateway_MessageUpdated;
            Gateway.MessageAck += Gateway_MessageAck;

            Gateway.GuildChannelCreated += Gateway_GuildChannelCreated;
            Gateway.GuildChannelDeleted += Gateway_GuildChannelDeleted;
            Gateway.GuildChannelUpdated += Gateway_GuildChannelUpdated;

            Gateway.PresenceUpdated += Gateway_PresenceUpdated;
            Gateway.UserNoteUpdated += Gateway_UserNoteUpdated;
            Gateway.UserSettingsUpdated += Gateway_UserSettingsUpdated;

            Gateway.VoiceServerUpdated += Gateway_VoiceServerUpdated;
            Gateway.VoiceStateUpdated += Gateway_VoiceStateUpdated;

            Gateway.SessionReplaced += Gateway_SessionReplaced;


            await Gateway.ConnectAsync();

            Messenger.Default.Register<GuildNavigateMessage>(this, async m =>
            {
                // TODO: Channel typing check
                //var channelList = ServicesManager.Cache.Runtime.TryGetValue<List<Channel>>(Quarrel.Helpers.Constants.Cache.Keys.ChannelList, m.GuildId);
                //var idList = channelList.ConvertAll(x => x.Id);

                List<string> idList = new List<string>();

                // Guild Sync
                if (m.Guild.Model.Id != "DM")
                {
                    idList.Add(m.Guild.Model.Id);
                }

                await Gateway.SubscribeToGuild(idList.ToArray());
            });
        }


        #region Events

        private void Gateway_Ready(object sender, GatewayEventArgs<Ready> e)
        {
            e.EventData.Cache();
            Messenger.Default.Send(new GatewayReadyMessage(e.EventData));
        }

        #region Messages

        private void Gateway_MessageCreated(object sender, GatewayEventArgs<Message> e)
        {
            var currentUser = CurrentUsersService.CurrentUser.Model;
            var channel = GuildsService.GetChannel(e.EventData.ChannelId);
            if (channel != null)
            {
                if (channel.IsDirectChannel || channel.IsGroupChannel || e.EventData.Mentions.Contains(currentUser) ||
                    e.EventData.MentionEveryone)
                    channel.ReadState.MentionCount++;
                channel.UpdateLMID(e.EventData.Id);
            }

            if (e.EventData.User == null)
                e.EventData.User = currentUser;
            Messenger.Default.Send(new GatewayMessageRecievedMessage(e.EventData));
        }

        private void Gateway_MessageDeleted(object sender, GatewayEventArgs<MessageDelete> e)
        {
            Messenger.Default.Send(new GatewayMessageDeletedMessage(e.EventData.ChannelId, e.EventData.MessageId));
        }

        private void Gateway_MessageUpdated(object sender, GatewayEventArgs<Message> e)
        {
            Messenger.Default.Send(new GatewayMessageUpdatedMessage(e.EventData));
        }

        private void Gateway_MessageAck(object sender, GatewayEventArgs<MessageAck> e)
        {
            GuildsService.GetChannel(e.EventData.ChannelId)?.UpdateLRMID(e.EventData.Id);
            Messenger.Default.Send(new GatewayMessageAckMessage(e.EventData.ChannelId, e.EventData.Id));
        }

        #endregion

        #region Channels
        private void Gateway_GuildChannelCreated(object sender, GatewayEventArgs<GuildChannel> e)
        {
            Messenger.Default.Send(new GatewayGuildChannelCreatedMessage(e.EventData));
        }

        private void Gateway_GuildChannelDeleted(object sender, GatewayEventArgs<GuildChannel> e)
        {
            Messenger.Default.Send(new GatewayGuildChannelDeletedMessage(e.EventData));
        }

        private void Gateway_GuildChannelUpdated(object sender, GatewayEventArgs<GuildChannel> e)
        {
            Messenger.Default.Send(new GatewayGuildChannelUpdatedMessage(e.EventData));
        }

        #endregion

        private void Gateway_GuildMemberChunk(object sender, GatewayEventArgs<GuildMemberChunk> e)
        {
            e.EventData.Cache();
        }

        private void Gateway_GuildSynced(object sender, GatewayEventArgs<GuildSync> e)
        {
            e.EventData.Cache();
            Messenger.Default.Send(new GatewayGuildSyncMessage(e.EventData.GuildId, e.EventData.Members.ToList()));
        }

        private void Gateway_PresenceUpdated(object sender, GatewayEventArgs<Presence> e)
        {
            Messenger.Default.Send(new GatewayPresenceUpdatedMessage(e.EventData.User.Id, e.EventData));
        }

        private void Gateway_UserNoteUpdated(object sender, GatewayEventArgs<UserNote> e)
        {
            CacheService.Runtime.SetValue(Quarrel.Helpers.Constants.Cache.Keys.Note, e.EventData.Note, e.EventData.UserId);
            Messenger.Default.Send(new GatewayNoteUpdatedMessage(e.EventData.UserId));
        }

        private void Gateway_UserSettingsUpdated(object sender, GatewayEventArgs<UserSettings> e)
        {
            Messenger.Default.Send(new GatewayUserSettingsUpdatedMessage(e.EventData));
            Messenger.Default.Send(new GatewayPresenceUpdatedMessage(CurrentUsersService.CurrentUser.Model.Id, new Presence() { Status = e.EventData.Status}));
        }

        #region Voice 

        private void Gateway_VoiceServerUpdated(object sender, GatewayEventArgs<VoiceServerUpdate> e)
        {
            Messenger.Default.Send(new GatewayVoiceServerUpdateMessage(e.EventData));
        }

        private void Gateway_VoiceStateUpdated(object sender, GatewayEventArgs<VoiceState> e)
        {
            Messenger.Default.Send(new GatewayVoiceStateUpdateMessage(e.EventData));
        }

        private void Gateway_SessionReplaced(object sender, GatewayEventArgs<SessionReplace[]> e)
        {
            Messenger.Default.Send(new GatewaySessionReplacedMessage(e.EventData));
        }

        #endregion

        #endregion
    }
}