﻿using System.Text.RegularExpressions;
using AsyncTwitch;

namespace TwitchIntegrationPlugin.Commands
{
    public class CommandAddToQueue : IrcCommand
    {
        public override string CommandName => "bsr";
        public override string[] CommandAlias => new string[] {"add"};

        private readonly Regex _songIDRX = new Regex(@"\d+-\d+", RegexOptions.Compiled);
        

        public override void Run(TwitchMessage msg)
        {
            if (StaticData.TwitchMode && !msg.Author.IsMod && !msg.Author.IsBroadcaster)
            {
                TwitchConnection.Instance.SendChatMessage("The Queue is currently closed.");
                return;
            }

            string QueryString = msg.Content.Remove(0, 5);
            bool isTextSearch = _songIDRX.IsMatch(QueryString);
            

            QueuedSong request = BeatSaver.GetSongFromBeatSaver(QueryString, isTextSearch, msg.Author.DisplayName);
            if (request.SongHash == "")
            {
                TwitchConnection.Instance.SendChatMessage("Invalid Request.");
            }
            if (StaticData.BanList.Contains(request.Id))
            {
                TwitchConnection.Instance.SendChatMessage("Song is currently banned.");
                return;
            }

            if (msg.Author.IsMod || msg.Author.IsBroadcaster)
            {
                AddToQueue(request);
                return;
            }

            if (StaticData.userRequests.ContainsKey(msg.Author.DisplayName))
            {
                int requestLimit = msg.Author.IsSubscriber
                    ? StaticData.TiConfig.SubLimit
                    : StaticData.TiConfig.ViewerLimit;
                if (StaticData.userRequests[msg.Author.DisplayName] <= requestLimit)
                {
                    TwitchConnection.Instance.SendChatMessage(msg.Author.DisplayName + " you're making too many requests. Slow down.");
                    return;
                }

                if(AddToQueue(request))
                    StaticData.userRequests[msg.Author.DisplayName]++;
                
            }
            else
            {
                if(AddToQueue(request))
                    StaticData.userRequests.Add(msg.Author.DisplayName, 1);
            }

        }

        private bool AddToQueue(QueuedSong song)
        {
            if (StaticData.QueueList.Contains(song))
            {
                TwitchConnection.Instance.SendChatMessage("Song already in queue.");
                return false;
            }

            StaticData.QueueList.Add(song);
            TwitchConnection.Instance.SendChatMessage($"{song.RequestedBy} added \"{song.SongName}\", uploaded by: {song.AuthName} to queue!");
            return true;
        }
    }
}
