﻿using PlenBotLogUploader.DiscordApi;
using PlenBotLogUploader.DpsReport;
using PlenBotLogUploader.Teams;
using PlenBotLogUploader.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PlenBotLogUploader
{
    public partial class FormEditDiscordWebhook : Form
    {
        #region definitions
        // fields
        private readonly FormDiscordWebhooks discordPingLink;
        private readonly DiscordWebhookData data;
        private readonly int reservedId;
        #endregion

        internal FormEditDiscordWebhook(FormDiscordWebhooks discordPingLink, DiscordWebhookData data, int reservedId)
        {
            this.discordPingLink = discordPingLink;
            this.data = data;
            this.reservedId = reservedId;
            InitializeComponent();
            Icon = Properties.Resources.AppIcon;
            Text = (data is null) ? "Add a new webhook" : "Edit an existing webhook";
            textBoxName.Text = data?.Name ?? "";
            textBoxUrl.Text = data?.Url ?? "";
            switch (data?.SuccessFailToggle ?? DiscordWebhookDataSuccessToggle.OnSuccessAndFailure)
            {
                case DiscordWebhookDataSuccessToggle.OnSuccessOnly:
                    radioButtonOnlySuccess.Checked = true;
                    break;
                case DiscordWebhookDataSuccessToggle.OnFailOnly:
                    radioButtonOnlyFail.Checked = true;
                    break;
                default:
                    radioButtonOnlySuccessAndFail.Checked = true;
                    break;
            }
            switch (data?.SummaryType ?? DiscordWebhookDataLogSummaryType.SquadAndPlayers)
            {
                case DiscordWebhookDataLogSummaryType.SquadAndPlayers:
                    radioButtonLogSummarySquadAndPlayers.Checked = true;
                    break;
                case DiscordWebhookDataLogSummaryType.SquadOnly:
                    radioButtonLogSummarySquad.Checked = true;
                    break;
                case DiscordWebhookDataLogSummaryType.PlayersOnly:
                    radioButtonLogSummaryPlayers.Checked = true;
                    break;
                default:
                    radioButtonLogSummaryNone.Checked = true;
                    break;
            }
            checkBoxAllowUnknownBossIds.Checked = data?.AllowUnknownBossIds ?? false;
            var bosses = Bosses.All
                .OrderBy(x => x.Type)
                .ThenBy(x => x.Name)
                .ToArray();
            var teams = Teams.Teams.All;
            foreach (var team in teams.Values)
            {
                comboBoxTeam.Items.Add(team);
            }
            comboBoxTeam.SelectedItem = data?.Team ?? teams[0];
            foreach (var boss in bosses.AsSpan())
            {
                checkedListBoxBossesEnable.Items.Add(new BossesDisableHelperClass()
                {
                    BossId = boss.BossId,
                    Text = $"{boss.Type}: {boss.Name} ({boss.BossId})" + (((boss.InternalDescription?.Length ?? 0) > 0) ? $" [{boss.InternalDescription}]" : null)
                }, data?.IsBossEnabled(boss.BossId) ?? true);
            }

            // BEAR
            checkBoxIncludeDamageSummary.Checked = data?.IncludeDamageSummary ?? true;
            checkBoxIncludeHealingSummary.Checked = data?.IncludeHealingSummary ?? false;
            checkBoxIncludeBarrierSummary.Checked = data?.IncludeBarrierSummary ?? false;
            //checkBoxCombineHealingBarrier.Checked = data?.CombineHealingBarrier ?? false;
            checkBoxAdjustBarrier.Checked = data?.AdjustBarrier ?? false;
            checkBoxIncludeCleansingSummary.Checked = data?.IncludeCleansingSummary ?? false;
            checkBoxIncludeStripSummary.Checked = data?.IncludeStripSummary ?? false;
            checkBoxIncludeCCSummary.Checked = data?.IncludeCCSummary ?? false;
            comboBoxMaxPlayers.SelectedItem = data?.MaxPlayers.ToString() ?? "10";
            //checkBoxShowDPSColumn.Checked = data?.ShowDpsColumn ?? true;
            checkBoxIncludeOpponentIcons.Checked = data?.ShowClassIcons ?? true;
            checkBoxShowFightAwards.Checked = data?.ShowFightAwards ?? true;
        }

        private void FormEditDiscordWebhook_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxName.Text.Trim()))
            {
                return;
            }
            var successFailToggle = DiscordWebhookDataSuccessToggle.OnSuccessAndFailure;
            if (radioButtonOnlySuccess.Checked)
            {
                successFailToggle = DiscordWebhookDataSuccessToggle.OnSuccessOnly;
            }
            else if (radioButtonOnlyFail.Checked)
            {
                successFailToggle = DiscordWebhookDataSuccessToggle.OnFailOnly;
            }
            var summaryType = DiscordWebhookDataLogSummaryType.SquadAndPlayers;
            if (radioButtonLogSummaryNone.Checked)
            {
                summaryType = DiscordWebhookDataLogSummaryType.None;
            }
            else if (radioButtonLogSummaryPlayers.Checked)
            {
                summaryType = DiscordWebhookDataLogSummaryType.PlayersOnly;
            }
            else if (radioButtonLogSummarySquad.Checked)
            {
                summaryType = DiscordWebhookDataLogSummaryType.SquadOnly;
            }
            if (data is null)
            {
                DiscordWebhooks.All[reservedId] = new DiscordWebhookData()
                {
                    Active = true,
                    Name = textBoxName.Text,
                    Url = textBoxUrl.Text,
                    SuccessFailToggle = successFailToggle,
                    SummaryType = summaryType,
                    BossesDisable = ConvertCheckboxListToArrayOfBossIds(),
                    AllowUnknownBossIds = checkBoxAllowUnknownBossIds.Checked,
                    Team = comboBoxTeam.SelectedItem as Team,

                    // BEAR
                    IncludeDamageSummary = checkBoxIncludeDamageSummary.Checked,
                    IncludeHealingSummary = checkBoxIncludeHealingSummary.Checked,
                    IncludeBarrierSummary = checkBoxIncludeBarrierSummary.Checked,
                    //CombineHealingBarrier = checkBoxCombineHealingBarrier.Checked,
                    AdjustBarrier = checkBoxAdjustBarrier.Checked,
                    IncludeCleansingSummary = checkBoxIncludeCleansingSummary.Checked,
                    IncludeStripSummary = checkBoxIncludeStripSummary.Checked,
                    IncludeCCSummary = checkBoxIncludeCCSummary.Checked,
                    MaxPlayers = int.Parse(comboBoxMaxPlayers.SelectedItem.ToString()),
                    ShowDpsColumn = false, // checkBoxShowDPSColumn.Checked,
                    ShowClassIcons = checkBoxIncludeOpponentIcons.Checked,
                    ShowFightAwards = checkBoxShowFightAwards.Checked
                };

                DiscordWebhooks.All[reservedId].ResetEmojis();

                discordPingLink.listViewDiscordWebhooks.Items.Add(new ListViewItem()
                {
                    Name = reservedId.ToString(),
                    Text = textBoxName.Text,
                    Checked = true,
                });
                return;
            }
            var webhook = DiscordWebhooks.All[reservedId];
            webhook.Active = data.Active;
            webhook.Name = textBoxName.Text;
            webhook.Url = textBoxUrl.Text;
            webhook.SuccessFailToggle = successFailToggle;
            webhook.SummaryType = summaryType;
            webhook.BossesDisable = ConvertCheckboxListToArrayOfBossIds();
            webhook.AllowUnknownBossIds = checkBoxAllowUnknownBossIds.Checked;
            webhook.Team = comboBoxTeam.SelectedItem as Team;

            // BEAR
            webhook.IncludeDamageSummary = checkBoxIncludeDamageSummary.Checked;
            webhook.IncludeHealingSummary = checkBoxIncludeHealingSummary.Checked;
            webhook.IncludeBarrierSummary = checkBoxIncludeBarrierSummary.Checked;
            //webhook.CombineHealingBarrier = checkBoxCombineHealingBarrier.Checked;
            webhook.AdjustBarrier = checkBoxAdjustBarrier.Checked;
            webhook.IncludeCleansingSummary = checkBoxIncludeCleansingSummary.Checked;
            webhook.IncludeStripSummary = checkBoxIncludeStripSummary.Checked;
            webhook.IncludeCCSummary = checkBoxIncludeCCSummary.Checked;
            webhook.MaxPlayers = int.Parse(comboBoxMaxPlayers.SelectedItem.ToString());
            webhook.ShowDpsColumn = false; // checkBoxShowDPSColumn.Checked;
            webhook.ShowClassIcons = checkBoxIncludeOpponentIcons.Checked;
            webhook.ShowFightAwards = checkBoxShowFightAwards.Checked;

            discordPingLink.listViewDiscordWebhooks.Items[discordPingLink.listViewDiscordWebhooks.Items.IndexOfKey(reservedId.ToString())] = new ListViewItem()
            {
                Name = reservedId.ToString(),
                Text = textBoxName.Text,
                Checked = data.Active,
            };
        }

        private int[] ConvertCheckboxListToArrayOfBossIds()
        {
            var list = new List<int>();
            for (var i = 0; i < checkedListBoxBossesEnable.Items.Count; i++)
            {
                var item = checkedListBoxBossesEnable.Items[i];
                if ((item is BossesDisableHelperClass bossEnableHelper) && (!checkedListBoxBossesEnable.GetItemChecked(i)))
                {
                    list.Add(bossEnableHelper.BossId);
                }
            }
            return list.ToArray();
        }

        private void ButtonUnSelectAll_Click(object sender, EventArgs e)
        {
            var allSelected = true;
            for (var i = 0; i < checkedListBoxBossesEnable.Items.Count; i++)
            {
                if (!checkedListBoxBossesEnable.GetItemChecked(i))
                {
                    allSelected = false;
                    break;
                }
            }
            if (!allSelected)
            {
                for (var i = 0; i < checkedListBoxBossesEnable.Items.Count; i++)
                {
                    checkedListBoxBossesEnable.SetItemChecked(i, true);
                }
                return;
            }
            for (var i = 0; i < checkedListBoxBossesEnable.Items.Count; i++)
            {
                checkedListBoxBossesEnable.SetItemChecked(i, false);
            }
        }

        private void ButtonUnSelectAllRaids_Click(object sender, EventArgs e)
        {
            var allSelected = true;
            for (var i = 0; i < checkedListBoxBossesEnable.Items.Count; i++)
            {
                var item = (BossesDisableHelperClass)checkedListBoxBossesEnable.Items[i];
                if ((Bosses.All.Find(x => x.BossId.Equals(item.BossId))?.Type.Equals(BossType.Raid) ?? false) && !checkedListBoxBossesEnable.GetItemChecked(i))
                {
                    allSelected = false;
                    break;
                }
            }
            if (!allSelected)
            {
                for (var i = 0; i < checkedListBoxBossesEnable.Items.Count; i++)
                {
                    var item = (BossesDisableHelperClass)checkedListBoxBossesEnable.Items[i];
                    if (Bosses.All.Find(x => x.BossId.Equals(item.BossId))?.Type.Equals(BossType.Raid) ?? false)
                    {
                        checkedListBoxBossesEnable.SetItemChecked(i, true);
                    }
                }
                return;
            }
            for (var i = 0; i < checkedListBoxBossesEnable.Items.Count; i++)
            {
                var item = (BossesDisableHelperClass)checkedListBoxBossesEnable.Items[i];
                if (Bosses.All.Find(x => x.BossId.Equals(item.BossId))?.Type.Equals(BossType.Raid) ?? false)
                {
                    checkedListBoxBossesEnable.SetItemChecked(i, false);
                }
            }
        }

        private void ButtonUnSelectAllFractals_Click(object sender, EventArgs e)
        {
            var allSelected = true;
            for (var i = 0; i < checkedListBoxBossesEnable.Items.Count; i++)
            {
                var item = (BossesDisableHelperClass)checkedListBoxBossesEnable.Items[i];
                if ((Bosses.All.Find(x => x.BossId.Equals(item.BossId))?.Type.Equals(BossType.Fractal) ?? false) && !checkedListBoxBossesEnable.GetItemChecked(i))
                {
                    allSelected = false;
                    break;
                }
            }
            if (!allSelected)
            {
                for (var i = 0; i < checkedListBoxBossesEnable.Items.Count; i++)
                {
                    var item = (BossesDisableHelperClass)checkedListBoxBossesEnable.Items[i];
                    if (Bosses.All.Find(x => x.BossId.Equals(item.BossId))?.Type.Equals(BossType.Fractal) ?? false)
                    {
                        checkedListBoxBossesEnable.SetItemChecked(i, true);
                    }
                }
                return;
            }
            for (var i = 0; i < checkedListBoxBossesEnable.Items.Count; i++)
            {
                var item = (BossesDisableHelperClass)checkedListBoxBossesEnable.Items[i];
                if (Bosses.All.Find(x => x.BossId.Equals(item.BossId))?.Type.Equals(BossType.Fractal) ?? false)
                {
                    checkedListBoxBossesEnable.SetItemChecked(i, false);
                }
            }
        }

        private void ButtonUnSelectAllStrikes_Click(object sender, EventArgs e)
        {
            var allSelected = true;
            for (var i = 0; i < checkedListBoxBossesEnable.Items.Count; i++)
            {
                var item = (BossesDisableHelperClass)checkedListBoxBossesEnable.Items[i];
                if ((Bosses.All.Find(x => x.BossId.Equals(item.BossId))?.Type.Equals(BossType.Strike) ?? false) && !checkedListBoxBossesEnable.GetItemChecked(i))
                {
                    allSelected = false;
                    break;
                }
            }
            if (!allSelected)
            {
                for (var i = 0; i < checkedListBoxBossesEnable.Items.Count; i++)
                {
                    var item = (BossesDisableHelperClass)checkedListBoxBossesEnable.Items[i];
                    if (Bosses.All.Find(x => x.BossId.Equals(item.BossId))?.Type.Equals(BossType.Strike) ?? false)
                    {
                        checkedListBoxBossesEnable.SetItemChecked(i, true);
                    }
                }
                return;
            }
            for (var i = 0; i < checkedListBoxBossesEnable.Items.Count; i++)
            {
                var item = (BossesDisableHelperClass)checkedListBoxBossesEnable.Items[i];
                if (Bosses.All.Find(x => x.BossId.Equals(item.BossId))?.Type.Equals(BossType.Strike) ?? false)
                {
                    checkedListBoxBossesEnable.SetItemChecked(i, false);
                }
            }
        }

        private void ButtonUnSelectAllGolems_Click(object sender, EventArgs e)
        {
            var allSelected = true;
            for (var i = 0; i < checkedListBoxBossesEnable.Items.Count; i++)
            {
                var item = (BossesDisableHelperClass)checkedListBoxBossesEnable.Items[i];
                if ((Bosses.All.Find(x => x.BossId.Equals(item.BossId))?.Type.Equals(BossType.Golem) ?? false) && !checkedListBoxBossesEnable.GetItemChecked(i))
                {
                    allSelected = false;
                    break;
                }
            }
            if (!allSelected)
            {
                for (var i = 0; i < checkedListBoxBossesEnable.Items.Count; i++)
                {
                    var item = (BossesDisableHelperClass)checkedListBoxBossesEnable.Items[i];
                    if (Bosses.All.Find(x => x.BossId.Equals(item.BossId))?.Type.Equals(BossType.Golem) ?? false)
                    {
                        checkedListBoxBossesEnable.SetItemChecked(i, true);
                    }
                }
                return;
            }
            for (var i = 0; i < checkedListBoxBossesEnable.Items.Count; i++)
            {
                var item = (BossesDisableHelperClass)checkedListBoxBossesEnable.Items[i];
                if (Bosses.All.Find(x => x.BossId.Equals(item.BossId))?.Type.Equals(BossType.Golem) ?? false)
                {
                    checkedListBoxBossesEnable.SetItemChecked(i, false);
                }
            }
        }

        private void ButtonUnSelectWvW_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < checkedListBoxBossesEnable.Items.Count; i++)
            {
                var item = (BossesDisableHelperClass)checkedListBoxBossesEnable.Items[i];
                if (item.BossId.Equals(1))
                {
                    checkedListBoxBossesEnable.SetItemChecked(i, !checkedListBoxBossesEnable.GetItemChecked(i));
                    break;
                }
            }
        }

        // BEAR
        private void checkBoxIncludeBarrierSummary_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxAdjustBarrier.Enabled = checkBoxIncludeBarrierSummary.Checked;
            checkBoxAdjustBarrier.Checked = checkBoxAdjustBarrier.Enabled && checkBoxAdjustBarrier.Checked;
            UpdateCombineHealingBarrier();
        }
        // BEAR
        private void checkBoxIncludeHealingSummary_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCombineHealingBarrier();
        }
        // BEAR
        private void UpdateCombineHealingBarrier()
        {
            //checkBoxCombineHealingBarrier.Enabled = checkBoxIncludeHealingSummary.Checked && checkBoxIncludeBarrierSummary.Checked;
            //checkBoxCombineHealingBarrier.Checked = checkBoxCombineHealingBarrier.Enabled && checkBoxCombineHealingBarrier.Checked;
        }

        private void checkBoxIncludeDamageSummary_CheckedChanged(object sender, EventArgs e)
        {
            //checkBoxShowDPSColumn.Enabled = checkBoxIncludeDamageSummary.Checked;
            //checkBoxShowDPSColumn.Checked = checkBoxShowDPSColumn.Enabled && checkBoxShowDPSColumn.Checked;
        }

        private void buttonResetClassIcons_Click(object sender, EventArgs e)
        {
            DiscordWebhooks.All[reservedId]?.ResetEmojis();
        }
    }
}
