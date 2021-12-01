﻿
using MusicPlayer.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TagLib;
using TagLib.Id3v2;

namespace MusicPlayer.View
{
    public partial class MediaSearch : UserControl
    {
        public Song song;
        Playlist parent;
        int idx;
        string localpath = "";
        public MediaSearch()
        {
            InitializeComponent();
        }

        public MediaSearch(Song s, int idx, Playlist parent)
        {
            InitializeComponent();
            song = s;
            this.parent = parent;
            this.idx = idx;
            lbName.Text = song.GetTitle();
            lbArtist.Text = song.GetArtistNameJoined();
            if (song.GetThumbImg() is string)
            {
                pbImg.SizeMode = PictureBoxSizeMode.StretchImage;
                pbImg.LoadAsync(song.GetThumbImg().ToString());
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                localpath = dialog.SelectedPath + "\\" + song.GetTitle() + "-" + song.GetArtistNameJoined() + ".mp3";
                using (var client = new WebClient())
                {
                    client.DownloadFileCompleted += Client_DownloadFileCompleted;
                    client.DownloadFileAsync(new Uri(song.GetSrc()), localpath);
                }
            }
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            var tfile = File.Create(localpath);
            TagLib.Id3v2.Tag t = (TagLib.Id3v2.Tag)tfile.GetTag(TagTypes.Id3v2); // You can add a true parameter to the GetTag function if the file doesn't already have a tag.
            PrivateFrame p = PrivateFrame.Get(t, "EncodedId", true);
            p.PrivateData = Encoding.Unicode.GetBytes(song.GetEncodedId());
            tfile.Save(); // This is optional.
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            MainForm.Instance.isPlaying = parent;
            MainForm.Instance.NormalPlaying();
            MainForm.Instance.counter = idx;
            MainForm.Instance.PlayMedia();
            MainForm.Instance.LoadViewPlaying();
        }
    }
}
