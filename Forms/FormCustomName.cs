﻿using PlenBotLogUploader.AppSettings;
using PlenBotLogUploader.Properties;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace PlenBotLogUploader;

public partial class FormCustomName : Form
{
    // constants
    private const string generateOAuthUrl = "https://twitchapps.com/tmi/";
    // fields
    private readonly FormMain mainLink;

    internal FormCustomName(FormMain mainLink)
    {
        this.mainLink = mainLink;
        InitializeComponent();
        Icon = Resources.AppIcon;
    }

    private async void FormCustomName_FormClosing(object sender, FormClosingEventArgs e)
    {
        e.Cancel = true;
        Hide();
        ApplicationSettings.Current.Twitch.Custom.Name = textBoxCustomName.Text.ToLower();
        if (!textBoxCustomOAuth.Text.StartsWith("oauth:"))
        {
            textBoxCustomOAuth.Text = $"oauth:{textBoxCustomOAuth.Text}";
        }
        ApplicationSettings.Current.Twitch.Custom.OauthPassword = textBoxCustomOAuth.Text;
        ApplicationSettings.Current.Save();
        if (!mainLink.IsTwitchConnectionNull())
        {
            await mainLink.ReconnectTwitchBot();
        }
    }

    private void CheckBoxCustomNameEnable_CheckedChanged(object sender, EventArgs e)
    {
        ApplicationSettings.Current.Twitch.Custom.Enabled = checkBoxCustomNameEnable.Checked;
        ApplicationSettings.Current.Save();
        groupBoxCustomNameSettings.Enabled = checkBoxCustomNameEnable.Checked;
    }

    private void LinkLabelGetOAuth_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => Process.Start(new ProcessStartInfo { UseShellExecute = true, FileName = generateOAuthUrl });
}
