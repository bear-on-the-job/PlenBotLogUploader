﻿namespace PlenBotLogUploader
{
    partial class FormEditDiscordWebhook
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            labelName = new System.Windows.Forms.Label();
            labelUrl = new System.Windows.Forms.Label();
            textBoxName = new System.Windows.Forms.TextBox();
            textBoxUrl = new System.Windows.Forms.TextBox();
            checkedListBoxBossesEnable = new System.Windows.Forms.CheckedListBox();
            groupBoxWebhookInfo = new System.Windows.Forms.GroupBox();
            groupBoxBossesEnable = new System.Windows.Forms.GroupBox();
            checkBoxAllowUnknownBossIds = new System.Windows.Forms.CheckBox();
            buttonUnSelectAllGolems = new System.Windows.Forms.Button();
            buttonUnSelectWvW = new System.Windows.Forms.Button();
            buttonUnSelectAllFractals = new System.Windows.Forms.Button();
            buttonUnSelectAllStrikes = new System.Windows.Forms.Button();
            buttonUnSelectAllRaids = new System.Windows.Forms.Button();
            buttonUnSelectAll = new System.Windows.Forms.Button();
            groupBoxConditionalPost = new System.Windows.Forms.GroupBox();
            radioButtonOnlySuccessAndFail = new System.Windows.Forms.RadioButton();
            radioButtonOnlyFail = new System.Windows.Forms.RadioButton();
            radioButtonOnlySuccess = new System.Windows.Forms.RadioButton();
            groupBoxTeam = new System.Windows.Forms.GroupBox();
            comboBoxTeam = new System.Windows.Forms.ComboBox();
            groupBoxLogSummaries = new System.Windows.Forms.GroupBox();
            radioButtonLogSummaryPlayers = new System.Windows.Forms.RadioButton();
            radioButtonLogSummarySquadAndPlayers = new System.Windows.Forms.RadioButton();
            radioButtonLogSummarySquad = new System.Windows.Forms.RadioButton();
            radioButtonLogSummaryNone = new System.Windows.Forms.RadioButton();
            groupBox1 = new System.Windows.Forms.GroupBox();
            checkBoxIncludeStabilitySummary = new System.Windows.Forms.CheckBox();
            checkBoxIncludeDownsContributionSummary = new System.Windows.Forms.CheckBox();
            checkBoxShowFightAwards = new System.Windows.Forms.CheckBox();
            button1 = new System.Windows.Forms.Button();
            checkBoxIncludeOpponentIcons = new System.Windows.Forms.CheckBox();
            checkBoxAdjustBarrier = new System.Windows.Forms.CheckBox();
            comboBoxMaxPlayers = new System.Windows.Forms.ComboBox();
            checkBoxIncludeCCSummary = new System.Windows.Forms.CheckBox();
            checkBoxIncludeStripSummary = new System.Windows.Forms.CheckBox();
            label1 = new System.Windows.Forms.Label();
            checkBoxIncludeCleansingSummary = new System.Windows.Forms.CheckBox();
            checkBoxIncludeHealingSummary = new System.Windows.Forms.CheckBox();
            checkBoxIncludeBarrierSummary = new System.Windows.Forms.CheckBox();
            checkBoxIncludeDamageSummary = new System.Windows.Forms.CheckBox();
            groupBox2 = new System.Windows.Forms.GroupBox();
            textBoxGoogleSheetsUrl = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            groupBoxIncludeCheckboxes = new System.Windows.Forms.GroupBox();
            checkBoxIncludeLegendaryChallengeModeLogs = new System.Windows.Forms.CheckBox();
            checkBoxIncludeChallengeModeLogs = new System.Windows.Forms.CheckBox();
            checkBoxIncludeNormalLogs = new System.Windows.Forms.CheckBox();
            groupBox3 = new System.Windows.Forms.GroupBox();
            label5 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            textboxBlueTeamIds = new System.Windows.Forms.TextBox();
            textboxRedTeamIds = new System.Windows.Forms.TextBox();
            textboxGreenTeamIds = new System.Windows.Forms.TextBox();
            checkBoxIncludeIncomingDefensiveStats = new System.Windows.Forms.CheckBox();
            groupBoxWebhookInfo.SuspendLayout();
            groupBoxBossesEnable.SuspendLayout();
            groupBoxConditionalPost.SuspendLayout();
            groupBoxTeam.SuspendLayout();
            groupBoxLogSummaries.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBoxIncludeCheckboxes.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // labelName
            // 
            labelName.AutoSize = true;
            labelName.Location = new System.Drawing.Point(7, 19);
            labelName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelName.Name = "labelName";
            labelName.Size = new System.Drawing.Size(94, 15);
            labelName.TabIndex = 0;
            labelName.Text = "Webhook name:";
            // 
            // labelUrl
            // 
            labelUrl.AutoSize = true;
            labelUrl.Location = new System.Drawing.Point(7, 64);
            labelUrl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelUrl.Name = "labelUrl";
            labelUrl.Size = new System.Drawing.Size(85, 15);
            labelUrl.TabIndex = 1;
            labelUrl.Text = "Webhook URL:";
            // 
            // textBoxName
            // 
            textBoxName.Location = new System.Drawing.Point(10, 37);
            textBoxName.Margin = new System.Windows.Forms.Padding(4);
            textBoxName.Name = "textBoxName";
            textBoxName.Size = new System.Drawing.Size(455, 23);
            textBoxName.TabIndex = 2;
            // 
            // textBoxUrl
            // 
            textBoxUrl.Location = new System.Drawing.Point(10, 82);
            textBoxUrl.Margin = new System.Windows.Forms.Padding(4);
            textBoxUrl.Name = "textBoxUrl";
            textBoxUrl.Size = new System.Drawing.Size(455, 23);
            textBoxUrl.TabIndex = 3;
            // 
            // checkedListBoxBossesEnable
            // 
            checkedListBoxBossesEnable.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            checkedListBoxBossesEnable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            checkedListBoxBossesEnable.FormattingEnabled = true;
            checkedListBoxBossesEnable.Location = new System.Drawing.Point(7, 22);
            checkedListBoxBossesEnable.Margin = new System.Windows.Forms.Padding(4);
            checkedListBoxBossesEnable.Name = "checkedListBoxBossesEnable";
            checkedListBoxBossesEnable.Size = new System.Drawing.Size(442, 810);
            checkedListBoxBossesEnable.TabIndex = 6;
            // 
            // groupBoxWebhookInfo
            // 
            groupBoxWebhookInfo.Controls.Add(textBoxName);
            groupBoxWebhookInfo.Controls.Add(labelName);
            groupBoxWebhookInfo.Controls.Add(labelUrl);
            groupBoxWebhookInfo.Controls.Add(textBoxUrl);
            groupBoxWebhookInfo.Location = new System.Drawing.Point(14, 14);
            groupBoxWebhookInfo.Margin = new System.Windows.Forms.Padding(4);
            groupBoxWebhookInfo.Name = "groupBoxWebhookInfo";
            groupBoxWebhookInfo.Padding = new System.Windows.Forms.Padding(4);
            groupBoxWebhookInfo.Size = new System.Drawing.Size(473, 120);
            groupBoxWebhookInfo.TabIndex = 7;
            groupBoxWebhookInfo.TabStop = false;
            groupBoxWebhookInfo.Text = "Webhook info";
            // 
            // groupBoxBossesEnable
            // 
            groupBoxBossesEnable.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBoxBossesEnable.Controls.Add(checkBoxAllowUnknownBossIds);
            groupBoxBossesEnable.Controls.Add(buttonUnSelectAllGolems);
            groupBoxBossesEnable.Controls.Add(buttonUnSelectWvW);
            groupBoxBossesEnable.Controls.Add(buttonUnSelectAllFractals);
            groupBoxBossesEnable.Controls.Add(buttonUnSelectAllStrikes);
            groupBoxBossesEnable.Controls.Add(buttonUnSelectAllRaids);
            groupBoxBossesEnable.Controls.Add(buttonUnSelectAll);
            groupBoxBossesEnable.Controls.Add(checkedListBoxBossesEnable);
            groupBoxBossesEnable.Location = new System.Drawing.Point(495, 14);
            groupBoxBossesEnable.Margin = new System.Windows.Forms.Padding(4);
            groupBoxBossesEnable.Name = "groupBoxBossesEnable";
            groupBoxBossesEnable.Padding = new System.Windows.Forms.Padding(4);
            groupBoxBossesEnable.Size = new System.Drawing.Size(458, 925);
            groupBoxBossesEnable.TabIndex = 8;
            groupBoxBossesEnable.TabStop = false;
            groupBoxBossesEnable.Text = "Only upload for selected bosses";
            // 
            // checkBoxAllowUnknownBossIds
            // 
            checkBoxAllowUnknownBossIds.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            checkBoxAllowUnknownBossIds.AutoSize = true;
            checkBoxAllowUnknownBossIds.Location = new System.Drawing.Point(8, 859);
            checkBoxAllowUnknownBossIds.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            checkBoxAllowUnknownBossIds.Name = "checkBoxAllowUnknownBossIds";
            checkBoxAllowUnknownBossIds.Size = new System.Drawing.Size(256, 19);
            checkBoxAllowUnknownBossIds.TabIndex = 13;
            checkBoxAllowUnknownBossIds.Text = "Allow unknown bosses to use this webhook";
            checkBoxAllowUnknownBossIds.UseVisualStyleBackColor = true;
            // 
            // buttonUnSelectAllGolems
            // 
            buttonUnSelectAllGolems.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            buttonUnSelectAllGolems.Location = new System.Drawing.Point(317, 886);
            buttonUnSelectAllGolems.Margin = new System.Windows.Forms.Padding(4);
            buttonUnSelectAllGolems.Name = "buttonUnSelectAllGolems";
            buttonUnSelectAllGolems.Size = new System.Drawing.Size(63, 26);
            buttonUnSelectAllGolems.TabIndex = 12;
            buttonUnSelectAllGolems.Text = "Golems";
            buttonUnSelectAllGolems.UseVisualStyleBackColor = true;
            buttonUnSelectAllGolems.Click += ButtonUnSelectAllGolems_Click;
            // 
            // buttonUnSelectWvW
            // 
            buttonUnSelectWvW.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            buttonUnSelectWvW.Location = new System.Drawing.Point(387, 886);
            buttonUnSelectWvW.Margin = new System.Windows.Forms.Padding(4);
            buttonUnSelectWvW.Name = "buttonUnSelectWvW";
            buttonUnSelectWvW.Size = new System.Drawing.Size(63, 26);
            buttonUnSelectWvW.TabIndex = 11;
            buttonUnSelectWvW.Text = "WvW";
            buttonUnSelectWvW.UseVisualStyleBackColor = true;
            buttonUnSelectWvW.Click += ButtonUnSelectWvW_Click;
            // 
            // buttonUnSelectAllFractals
            // 
            buttonUnSelectAllFractals.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            buttonUnSelectAllFractals.Location = new System.Drawing.Point(177, 886);
            buttonUnSelectAllFractals.Margin = new System.Windows.Forms.Padding(4);
            buttonUnSelectAllFractals.Name = "buttonUnSelectAllFractals";
            buttonUnSelectAllFractals.Size = new System.Drawing.Size(63, 26);
            buttonUnSelectAllFractals.TabIndex = 10;
            buttonUnSelectAllFractals.Text = "Fractals";
            buttonUnSelectAllFractals.UseVisualStyleBackColor = true;
            buttonUnSelectAllFractals.Click += ButtonUnSelectAllFractals_Click;
            // 
            // buttonUnSelectAllStrikes
            // 
            buttonUnSelectAllStrikes.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            buttonUnSelectAllStrikes.Location = new System.Drawing.Point(247, 886);
            buttonUnSelectAllStrikes.Margin = new System.Windows.Forms.Padding(4);
            buttonUnSelectAllStrikes.Name = "buttonUnSelectAllStrikes";
            buttonUnSelectAllStrikes.Size = new System.Drawing.Size(63, 26);
            buttonUnSelectAllStrikes.TabIndex = 9;
            buttonUnSelectAllStrikes.Text = "Strikes";
            buttonUnSelectAllStrikes.UseVisualStyleBackColor = true;
            buttonUnSelectAllStrikes.Click += ButtonUnSelectAllStrikes_Click;
            // 
            // buttonUnSelectAllRaids
            // 
            buttonUnSelectAllRaids.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            buttonUnSelectAllRaids.Location = new System.Drawing.Point(107, 886);
            buttonUnSelectAllRaids.Margin = new System.Windows.Forms.Padding(4);
            buttonUnSelectAllRaids.Name = "buttonUnSelectAllRaids";
            buttonUnSelectAllRaids.Size = new System.Drawing.Size(63, 26);
            buttonUnSelectAllRaids.TabIndex = 8;
            buttonUnSelectAllRaids.Text = "Raids";
            buttonUnSelectAllRaids.UseVisualStyleBackColor = true;
            buttonUnSelectAllRaids.Click += ButtonUnSelectAllRaids_Click;
            // 
            // buttonUnSelectAll
            // 
            buttonUnSelectAll.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            buttonUnSelectAll.Location = new System.Drawing.Point(6, 886);
            buttonUnSelectAll.Margin = new System.Windows.Forms.Padding(4);
            buttonUnSelectAll.Name = "buttonUnSelectAll";
            buttonUnSelectAll.Size = new System.Drawing.Size(94, 26);
            buttonUnSelectAll.TabIndex = 7;
            buttonUnSelectAll.Text = "(Un)Select all";
            buttonUnSelectAll.UseVisualStyleBackColor = true;
            buttonUnSelectAll.Click += ButtonUnSelectAll_Click;
            // 
            // groupBoxConditionalPost
            // 
            groupBoxConditionalPost.Controls.Add(radioButtonOnlySuccessAndFail);
            groupBoxConditionalPost.Controls.Add(radioButtonOnlyFail);
            groupBoxConditionalPost.Controls.Add(radioButtonOnlySuccess);
            groupBoxConditionalPost.Location = new System.Drawing.Point(14, 142);
            groupBoxConditionalPost.Margin = new System.Windows.Forms.Padding(4);
            groupBoxConditionalPost.Name = "groupBoxConditionalPost";
            groupBoxConditionalPost.Padding = new System.Windows.Forms.Padding(4);
            groupBoxConditionalPost.Size = new System.Drawing.Size(473, 76);
            groupBoxConditionalPost.TabIndex = 9;
            groupBoxConditionalPost.TabStop = false;
            groupBoxConditionalPost.Text = "Use this Webhook if...";
            // 
            // radioButtonOnlySuccessAndFail
            // 
            radioButtonOnlySuccessAndFail.AutoSize = true;
            radioButtonOnlySuccessAndFail.Location = new System.Drawing.Point(10, 50);
            radioButtonOnlySuccessAndFail.Margin = new System.Windows.Forms.Padding(4);
            radioButtonOnlySuccessAndFail.Name = "radioButtonOnlySuccessAndFail";
            radioButtonOnlySuccessAndFail.Size = new System.Drawing.Size(236, 19);
            radioButtonOnlySuccessAndFail.TabIndex = 2;
            radioButtonOnlySuccessAndFail.TabStop = true;
            radioButtonOnlySuccessAndFail.Text = "the encounter is either success or failure";
            radioButtonOnlySuccessAndFail.UseVisualStyleBackColor = true;
            // 
            // radioButtonOnlyFail
            // 
            radioButtonOnlyFail.AutoSize = true;
            radioButtonOnlyFail.Location = new System.Drawing.Point(280, 22);
            radioButtonOnlyFail.Margin = new System.Windows.Forms.Padding(4);
            radioButtonOnlyFail.Name = "radioButtonOnlyFail";
            radioButtonOnlyFail.Size = new System.Drawing.Size(155, 19);
            radioButtonOnlyFail.TabIndex = 1;
            radioButtonOnlyFail.TabStop = true;
            radioButtonOnlyFail.Text = "the encounter is a failure";
            radioButtonOnlyFail.UseVisualStyleBackColor = true;
            // 
            // radioButtonOnlySuccess
            // 
            radioButtonOnlySuccess.AutoSize = true;
            radioButtonOnlySuccess.Location = new System.Drawing.Point(10, 23);
            radioButtonOnlySuccess.Margin = new System.Windows.Forms.Padding(4);
            radioButtonOnlySuccess.Name = "radioButtonOnlySuccess";
            radioButtonOnlySuccess.Size = new System.Drawing.Size(162, 19);
            radioButtonOnlySuccess.TabIndex = 0;
            radioButtonOnlySuccess.TabStop = true;
            radioButtonOnlySuccess.Text = "the encounter is a success";
            radioButtonOnlySuccess.UseVisualStyleBackColor = true;
            // 
            // groupBoxTeam
            // 
            groupBoxTeam.Controls.Add(comboBoxTeam);
            groupBoxTeam.Location = new System.Drawing.Point(14, 226);
            groupBoxTeam.Margin = new System.Windows.Forms.Padding(4);
            groupBoxTeam.Name = "groupBoxTeam";
            groupBoxTeam.Padding = new System.Windows.Forms.Padding(4);
            groupBoxTeam.Size = new System.Drawing.Size(473, 54);
            groupBoxTeam.TabIndex = 7;
            groupBoxTeam.TabStop = false;
            groupBoxTeam.Text = "Associate with a player team";
            // 
            // comboBoxTeam
            // 
            comboBoxTeam.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxTeam.FormattingEnabled = true;
            comboBoxTeam.Location = new System.Drawing.Point(10, 22);
            comboBoxTeam.Margin = new System.Windows.Forms.Padding(4);
            comboBoxTeam.MaxDropDownItems = 100;
            comboBoxTeam.Name = "comboBoxTeam";
            comboBoxTeam.Size = new System.Drawing.Size(455, 23);
            comboBoxTeam.TabIndex = 0;
            // 
            // groupBoxLogSummaries
            // 
            groupBoxLogSummaries.Controls.Add(radioButtonLogSummaryPlayers);
            groupBoxLogSummaries.Controls.Add(radioButtonLogSummarySquadAndPlayers);
            groupBoxLogSummaries.Controls.Add(radioButtonLogSummarySquad);
            groupBoxLogSummaries.Controls.Add(radioButtonLogSummaryNone);
            groupBoxLogSummaries.Location = new System.Drawing.Point(14, 287);
            groupBoxLogSummaries.Name = "groupBoxLogSummaries";
            groupBoxLogSummaries.Size = new System.Drawing.Size(473, 86);
            groupBoxLogSummaries.TabIndex = 10;
            groupBoxLogSummaries.TabStop = false;
            groupBoxLogSummaries.Text = "Log summaries";
            // 
            // radioButtonLogSummaryPlayers
            // 
            radioButtonLogSummaryPlayers.AutoSize = true;
            radioButtonLogSummaryPlayers.Location = new System.Drawing.Point(230, 26);
            radioButtonLogSummaryPlayers.Name = "radioButtonLogSummaryPlayers";
            radioButtonLogSummaryPlayers.Size = new System.Drawing.Size(174, 19);
            radioButtonLogSummaryPlayers.TabIndex = 9;
            radioButtonLogSummaryPlayers.TabStop = true;
            radioButtonLogSummaryPlayers.Text = "Detailed player performance";
            radioButtonLogSummaryPlayers.UseVisualStyleBackColor = true;
            // 
            // radioButtonLogSummarySquadAndPlayers
            // 
            radioButtonLogSummarySquadAndPlayers.AutoSize = true;
            radioButtonLogSummarySquadAndPlayers.Location = new System.Drawing.Point(230, 56);
            radioButtonLogSummarySquadAndPlayers.Name = "radioButtonLogSummarySquadAndPlayers";
            radioButtonLogSummarySquadAndPlayers.Size = new System.Drawing.Size(225, 19);
            radioButtonLogSummarySquadAndPlayers.TabIndex = 8;
            radioButtonLogSummarySquadAndPlayers.TabStop = true;
            radioButtonLogSummarySquadAndPlayers.Text = "Detailed squad and player performace";
            radioButtonLogSummarySquadAndPlayers.UseVisualStyleBackColor = true;
            // 
            // radioButtonLogSummarySquad
            // 
            radioButtonLogSummarySquad.AutoSize = true;
            radioButtonLogSummarySquad.Location = new System.Drawing.Point(11, 56);
            radioButtonLogSummarySquad.Name = "radioButtonLogSummarySquad";
            radioButtonLogSummarySquad.Size = new System.Drawing.Size(169, 19);
            radioButtonLogSummarySquad.TabIndex = 7;
            radioButtonLogSummarySquad.TabStop = true;
            radioButtonLogSummarySquad.Text = "Detailed squad information";
            radioButtonLogSummarySquad.UseVisualStyleBackColor = true;
            // 
            // radioButtonLogSummaryNone
            // 
            radioButtonLogSummaryNone.AutoSize = true;
            radioButtonLogSummaryNone.Location = new System.Drawing.Point(11, 26);
            radioButtonLogSummaryNone.Name = "radioButtonLogSummaryNone";
            radioButtonLogSummaryNone.Size = new System.Drawing.Size(54, 19);
            radioButtonLogSummaryNone.TabIndex = 6;
            radioButtonLogSummaryNone.TabStop = true;
            radioButtonLogSummaryNone.Text = "None";
            radioButtonLogSummaryNone.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(checkBoxIncludeIncomingDefensiveStats);
            groupBox1.Controls.Add(checkBoxIncludeStabilitySummary);
            groupBox1.Controls.Add(checkBoxIncludeDownsContributionSummary);
            groupBox1.Controls.Add(checkBoxShowFightAwards);
            groupBox1.Controls.Add(button1);
            groupBox1.Controls.Add(checkBoxIncludeOpponentIcons);
            groupBox1.Controls.Add(checkBoxAdjustBarrier);
            groupBox1.Controls.Add(comboBoxMaxPlayers);
            groupBox1.Controls.Add(checkBoxIncludeCCSummary);
            groupBox1.Controls.Add(checkBoxIncludeStripSummary);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(checkBoxIncludeCleansingSummary);
            groupBox1.Controls.Add(checkBoxIncludeHealingSummary);
            groupBox1.Controls.Add(checkBoxIncludeBarrierSummary);
            groupBox1.Controls.Add(checkBoxIncludeDamageSummary);
            groupBox1.Location = new System.Drawing.Point(14, 451);
            groupBox1.Margin = new System.Windows.Forms.Padding(4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(4);
            groupBox1.Size = new System.Drawing.Size(473, 327);
            groupBox1.TabIndex = 10;
            groupBox1.TabStop = false;
            groupBox1.Text = "Player report information";
            // 
            // checkBoxIncludeStabilitySummary
            // 
            checkBoxIncludeStabilitySummary.AutoSize = true;
            checkBoxIncludeStabilitySummary.Location = new System.Drawing.Point(11, 211);
            checkBoxIncludeStabilitySummary.Margin = new System.Windows.Forms.Padding(4);
            checkBoxIncludeStabilitySummary.Name = "checkBoxIncludeStabilitySummary";
            checkBoxIncludeStabilitySummary.Size = new System.Drawing.Size(162, 19);
            checkBoxIncludeStabilitySummary.TabIndex = 20;
            checkBoxIncludeStabilitySummary.Text = "Include stability summary";
            checkBoxIncludeStabilitySummary.UseVisualStyleBackColor = true;
            checkBoxIncludeStabilitySummary.CheckedChanged += checkBoxIncludeStabilitySummary_CheckedChanged;
            // 
            // checkBoxIncludeDownsContributionSummary
            // 
            checkBoxIncludeDownsContributionSummary.AutoSize = true;
            checkBoxIncludeDownsContributionSummary.Location = new System.Drawing.Point(226, 49);
            checkBoxIncludeDownsContributionSummary.Name = "checkBoxIncludeDownsContributionSummary";
            checkBoxIncludeDownsContributionSummary.Size = new System.Drawing.Size(229, 19);
            checkBoxIncludeDownsContributionSummary.TabIndex = 11;
            checkBoxIncludeDownsContributionSummary.Text = "Include Downs Contribution Summary";
            checkBoxIncludeDownsContributionSummary.UseVisualStyleBackColor = true;
            // 
            // checkBoxShowFightAwards
            // 
            checkBoxShowFightAwards.AutoSize = true;
            checkBoxShowFightAwards.Location = new System.Drawing.Point(11, 265);
            checkBoxShowFightAwards.Margin = new System.Windows.Forms.Padding(4);
            checkBoxShowFightAwards.Name = "checkBoxShowFightAwards";
            checkBoxShowFightAwards.Size = new System.Drawing.Size(123, 19);
            checkBoxShowFightAwards.TabIndex = 19;
            checkBoxShowFightAwards.Text = "Show fight awards";
            checkBoxShowFightAwards.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(226, 233);
            button1.Margin = new System.Windows.Forms.Padding(4);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(234, 26);
            button1.TabIndex = 18;
            button1.Text = "Reset Class Icons";
            button1.UseVisualStyleBackColor = true;
            button1.Click += buttonResetClassIcons_Click;
            // 
            // checkBoxIncludeOpponentIcons
            // 
            checkBoxIncludeOpponentIcons.AutoSize = true;
            checkBoxIncludeOpponentIcons.Location = new System.Drawing.Point(11, 238);
            checkBoxIncludeOpponentIcons.Margin = new System.Windows.Forms.Padding(4);
            checkBoxIncludeOpponentIcons.Name = "checkBoxIncludeOpponentIcons";
            checkBoxIncludeOpponentIcons.Size = new System.Drawing.Size(104, 19);
            checkBoxIncludeOpponentIcons.TabIndex = 17;
            checkBoxIncludeOpponentIcons.Text = "Use class icons";
            checkBoxIncludeOpponentIcons.UseVisualStyleBackColor = true;
            // 
            // checkBoxAdjustBarrier
            // 
            checkBoxAdjustBarrier.AutoSize = true;
            checkBoxAdjustBarrier.Enabled = false;
            checkBoxAdjustBarrier.Location = new System.Drawing.Point(226, 103);
            checkBoxAdjustBarrier.Margin = new System.Windows.Forms.Padding(4);
            checkBoxAdjustBarrier.Name = "checkBoxAdjustBarrier";
            checkBoxAdjustBarrier.Size = new System.Drawing.Size(236, 19);
            checkBoxAdjustBarrier.TabIndex = 15;
            checkBoxAdjustBarrier.Text = "Adjust barrier based on absorbed values";
            checkBoxAdjustBarrier.UseVisualStyleBackColor = true;
            // 
            // comboBoxMaxPlayers
            // 
            comboBoxMaxPlayers.FormattingEnabled = true;
            comboBoxMaxPlayers.Items.AddRange(new object[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" });
            comboBoxMaxPlayers.Location = new System.Drawing.Point(273, 290);
            comboBoxMaxPlayers.Name = "comboBoxMaxPlayers";
            comboBoxMaxPlayers.Size = new System.Drawing.Size(49, 23);
            comboBoxMaxPlayers.TabIndex = 14;
            // 
            // checkBoxIncludeCCSummary
            // 
            checkBoxIncludeCCSummary.AutoSize = true;
            checkBoxIncludeCCSummary.Location = new System.Drawing.Point(11, 184);
            checkBoxIncludeCCSummary.Margin = new System.Windows.Forms.Padding(4);
            checkBoxIncludeCCSummary.Name = "checkBoxIncludeCCSummary";
            checkBoxIncludeCCSummary.Size = new System.Drawing.Size(137, 19);
            checkBoxIncludeCCSummary.TabIndex = 11;
            checkBoxIncludeCCSummary.Text = "Include CC summary";
            checkBoxIncludeCCSummary.UseVisualStyleBackColor = true;
            // 
            // checkBoxIncludeStripSummary
            // 
            checkBoxIncludeStripSummary.AutoSize = true;
            checkBoxIncludeStripSummary.Location = new System.Drawing.Point(11, 157);
            checkBoxIncludeStripSummary.Margin = new System.Windows.Forms.Padding(4);
            checkBoxIncludeStripSummary.Name = "checkBoxIncludeStripSummary";
            checkBoxIncludeStripSummary.Size = new System.Drawing.Size(144, 19);
            checkBoxIncludeStripSummary.TabIndex = 10;
            checkBoxIncludeStripSummary.Text = "Include strip summary";
            checkBoxIncludeStripSummary.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(13, 293);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(253, 15);
            label1.TabIndex = 13;
            label1.Text = "Number of players to include in summary lists:";
            // 
            // checkBoxIncludeCleansingSummary
            // 
            checkBoxIncludeCleansingSummary.AutoSize = true;
            checkBoxIncludeCleansingSummary.Location = new System.Drawing.Point(11, 130);
            checkBoxIncludeCleansingSummary.Margin = new System.Windows.Forms.Padding(4);
            checkBoxIncludeCleansingSummary.Name = "checkBoxIncludeCleansingSummary";
            checkBoxIncludeCleansingSummary.Size = new System.Drawing.Size(171, 19);
            checkBoxIncludeCleansingSummary.TabIndex = 9;
            checkBoxIncludeCleansingSummary.Text = "Include cleansing summary";
            checkBoxIncludeCleansingSummary.UseVisualStyleBackColor = true;
            // 
            // checkBoxIncludeHealingSummary
            // 
            checkBoxIncludeHealingSummary.AutoSize = true;
            checkBoxIncludeHealingSummary.Location = new System.Drawing.Point(11, 76);
            checkBoxIncludeHealingSummary.Margin = new System.Windows.Forms.Padding(4);
            checkBoxIncludeHealingSummary.Name = "checkBoxIncludeHealingSummary";
            checkBoxIncludeHealingSummary.Size = new System.Drawing.Size(160, 19);
            checkBoxIncludeHealingSummary.TabIndex = 8;
            checkBoxIncludeHealingSummary.Text = "Include healing summary";
            checkBoxIncludeHealingSummary.UseVisualStyleBackColor = true;
            checkBoxIncludeHealingSummary.CheckedChanged += checkBoxIncludeHealingSummary_CheckedChanged;
            // 
            // checkBoxIncludeBarrierSummary
            // 
            checkBoxIncludeBarrierSummary.AutoSize = true;
            checkBoxIncludeBarrierSummary.Location = new System.Drawing.Point(11, 103);
            checkBoxIncludeBarrierSummary.Margin = new System.Windows.Forms.Padding(4);
            checkBoxIncludeBarrierSummary.Name = "checkBoxIncludeBarrierSummary";
            checkBoxIncludeBarrierSummary.Size = new System.Drawing.Size(155, 19);
            checkBoxIncludeBarrierSummary.TabIndex = 7;
            checkBoxIncludeBarrierSummary.Text = "Include barrier summary";
            checkBoxIncludeBarrierSummary.UseVisualStyleBackColor = true;
            checkBoxIncludeBarrierSummary.CheckedChanged += checkBoxIncludeBarrierSummary_CheckedChanged;
            // 
            // checkBoxIncludeDamageSummary
            // 
            checkBoxIncludeDamageSummary.AutoSize = true;
            checkBoxIncludeDamageSummary.Location = new System.Drawing.Point(11, 49);
            checkBoxIncludeDamageSummary.Margin = new System.Windows.Forms.Padding(4);
            checkBoxIncludeDamageSummary.Name = "checkBoxIncludeDamageSummary";
            checkBoxIncludeDamageSummary.Size = new System.Drawing.Size(164, 19);
            checkBoxIncludeDamageSummary.TabIndex = 5;
            checkBoxIncludeDamageSummary.Text = "Include damage summary";
            checkBoxIncludeDamageSummary.UseVisualStyleBackColor = true;
            checkBoxIncludeDamageSummary.CheckedChanged += checkBoxIncludeDamageSummary_CheckedChanged;
            // 
            // groupBox2
            // 
            groupBox2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            groupBox2.Controls.Add(textBoxGoogleSheetsUrl);
            groupBox2.Controls.Add(label2);
            groupBox2.Location = new System.Drawing.Point(14, 866);
            groupBox2.Margin = new System.Windows.Forms.Padding(4);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new System.Windows.Forms.Padding(4);
            groupBox2.Size = new System.Drawing.Size(473, 73);
            groupBox2.TabIndex = 8;
            groupBox2.TabStop = false;
            groupBox2.Text = "Custom Awards";
            // 
            // textBoxGoogleSheetsUrl
            // 
            textBoxGoogleSheetsUrl.Location = new System.Drawing.Point(10, 37);
            textBoxGoogleSheetsUrl.Margin = new System.Windows.Forms.Padding(4);
            textBoxGoogleSheetsUrl.Name = "textBoxGoogleSheetsUrl";
            textBoxGoogleSheetsUrl.Size = new System.Drawing.Size(455, 23);
            textBoxGoogleSheetsUrl.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(8, 20);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(109, 15);
            label2.TabIndex = 0;
            label2.Text = "Google Sheets URL:";
            // 
            // groupBoxIncludeCheckboxes
            // 
            groupBoxIncludeCheckboxes.Controls.Add(checkBoxIncludeLegendaryChallengeModeLogs);
            groupBoxIncludeCheckboxes.Controls.Add(checkBoxIncludeChallengeModeLogs);
            groupBoxIncludeCheckboxes.Controls.Add(checkBoxIncludeNormalLogs);
            groupBoxIncludeCheckboxes.Location = new System.Drawing.Point(14, 381);
            groupBoxIncludeCheckboxes.Margin = new System.Windows.Forms.Padding(5);
            groupBoxIncludeCheckboxes.Name = "groupBoxIncludeCheckboxes";
            groupBoxIncludeCheckboxes.Padding = new System.Windows.Forms.Padding(5);
            groupBoxIncludeCheckboxes.Size = new System.Drawing.Size(473, 63);
            groupBoxIncludeCheckboxes.TabIndex = 10;
            groupBoxIncludeCheckboxes.TabStop = false;
            groupBoxIncludeCheckboxes.Text = "Include all ...";
            // 
            // checkBoxIncludeLegendaryChallengeModeLogs
            // 
            checkBoxIncludeLegendaryChallengeModeLogs.AutoSize = true;
            checkBoxIncludeLegendaryChallengeModeLogs.Location = new System.Drawing.Point(330, 28);
            checkBoxIncludeLegendaryChallengeModeLogs.Name = "checkBoxIncludeLegendaryChallengeModeLogs";
            checkBoxIncludeLegendaryChallengeModeLogs.Size = new System.Drawing.Size(125, 19);
            checkBoxIncludeLegendaryChallengeModeLogs.TabIndex = 2;
            checkBoxIncludeLegendaryChallengeModeLogs.Text = "legendary CM logs";
            checkBoxIncludeLegendaryChallengeModeLogs.UseVisualStyleBackColor = true;
            // 
            // checkBoxIncludeChallengeModeLogs
            // 
            checkBoxIncludeChallengeModeLogs.AutoSize = true;
            checkBoxIncludeChallengeModeLogs.Location = new System.Drawing.Point(194, 28);
            checkBoxIncludeChallengeModeLogs.Name = "checkBoxIncludeChallengeModeLogs";
            checkBoxIncludeChallengeModeLogs.Size = new System.Drawing.Size(70, 19);
            checkBoxIncludeChallengeModeLogs.TabIndex = 1;
            checkBoxIncludeChallengeModeLogs.Text = "CM logs";
            checkBoxIncludeChallengeModeLogs.UseVisualStyleBackColor = true;
            // 
            // checkBoxIncludeNormalLogs
            // 
            checkBoxIncludeNormalLogs.AutoSize = true;
            checkBoxIncludeNormalLogs.Location = new System.Drawing.Point(11, 28);
            checkBoxIncludeNormalLogs.Name = "checkBoxIncludeNormalLogs";
            checkBoxIncludeNormalLogs.Size = new System.Drawing.Size(123, 19);
            checkBoxIncludeNormalLogs.TabIndex = 0;
            checkBoxIncludeNormalLogs.Text = "normal mode logs";
            checkBoxIncludeNormalLogs.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            groupBox3.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            groupBox3.Controls.Add(label5);
            groupBox3.Controls.Add(label4);
            groupBox3.Controls.Add(label3);
            groupBox3.Controls.Add(textboxBlueTeamIds);
            groupBox3.Controls.Add(textboxRedTeamIds);
            groupBox3.Controls.Add(textboxGreenTeamIds);
            groupBox3.Location = new System.Drawing.Point(14, 785);
            groupBox3.Margin = new System.Windows.Forms.Padding(4);
            groupBox3.Name = "groupBox3";
            groupBox3.Padding = new System.Windows.Forms.Padding(4);
            groupBox3.Size = new System.Drawing.Size(473, 73);
            groupBox3.TabIndex = 11;
            groupBox3.TabStop = false;
            groupBox3.Text = "WvW Team IDs";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(320, 18);
            label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(41, 15);
            label5.TabIndex = 7;
            label5.Text = "Green:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(162, 18);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(33, 15);
            label4.TabIndex = 6;
            label4.Text = "Blue:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(8, 18);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(30, 15);
            label3.TabIndex = 5;
            label3.Text = "Red:";
            // 
            // textboxBlueTeamIds
            // 
            textboxBlueTeamIds.Location = new System.Drawing.Point(162, 37);
            textboxBlueTeamIds.Margin = new System.Windows.Forms.Padding(4);
            textboxBlueTeamIds.Name = "textboxBlueTeamIds";
            textboxBlueTeamIds.Size = new System.Drawing.Size(150, 23);
            textboxBlueTeamIds.TabIndex = 4;
            // 
            // textboxRedTeamIds
            // 
            textboxRedTeamIds.Location = new System.Drawing.Point(10, 37);
            textboxRedTeamIds.Margin = new System.Windows.Forms.Padding(4);
            textboxRedTeamIds.Name = "textboxRedTeamIds";
            textboxRedTeamIds.Size = new System.Drawing.Size(144, 23);
            textboxRedTeamIds.TabIndex = 3;
            // 
            // textboxGreenTeamIds
            // 
            textboxGreenTeamIds.Location = new System.Drawing.Point(320, 37);
            textboxGreenTeamIds.Margin = new System.Windows.Forms.Padding(4);
            textboxGreenTeamIds.Name = "textboxGreenTeamIds";
            textboxGreenTeamIds.Size = new System.Drawing.Size(145, 23);
            textboxGreenTeamIds.TabIndex = 2;
            // 
            // checkBoxIncludeIncomingDefensiveStats
            // 
            checkBoxIncludeIncomingDefensiveStats.AutoSize = true;
            checkBoxIncludeIncomingDefensiveStats.Location = new System.Drawing.Point(11, 22);
            checkBoxIncludeIncomingDefensiveStats.Margin = new System.Windows.Forms.Padding(4);
            checkBoxIncludeIncomingDefensiveStats.Name = "checkBoxIncludeIncomingDefensiveStats";
            checkBoxIncludeIncomingDefensiveStats.Size = new System.Drawing.Size(199, 19);
            checkBoxIncludeIncomingDefensiveStats.TabIndex = 21;
            checkBoxIncludeIncomingDefensiveStats.Text = "Include incoming defensive stats";
            checkBoxIncludeIncomingDefensiveStats.UseVisualStyleBackColor = true;
            // 
            // FormEditDiscordWebhook
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.White;
            ClientSize = new System.Drawing.Size(968, 952);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBoxIncludeCheckboxes);
            Controls.Add(groupBoxLogSummaries);
            Controls.Add(groupBox1);
            Controls.Add(groupBoxTeam);
            Controls.Add(groupBoxConditionalPost);
            Controls.Add(groupBoxBossesEnable);
            Controls.Add(groupBoxWebhookInfo);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Margin = new System.Windows.Forms.Padding(4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormEditDiscordWebhook";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "FormEditDiscordWebhook";
            FormClosing += FormEditDiscordWebhook_FormClosing;
            groupBoxWebhookInfo.ResumeLayout(false);
            groupBoxWebhookInfo.PerformLayout();
            groupBoxBossesEnable.ResumeLayout(false);
            groupBoxBossesEnable.PerformLayout();
            groupBoxConditionalPost.ResumeLayout(false);
            groupBoxConditionalPost.PerformLayout();
            groupBoxTeam.ResumeLayout(false);
            groupBoxLogSummaries.ResumeLayout(false);
            groupBoxLogSummaries.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBoxIncludeCheckboxes.ResumeLayout(false);
            groupBoxIncludeCheckboxes.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Label labelUrl;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.TextBox textBoxUrl;
        private System.Windows.Forms.CheckedListBox checkedListBoxBossesEnable;
        private System.Windows.Forms.GroupBox groupBoxWebhookInfo;
        private System.Windows.Forms.GroupBox groupBoxBossesEnable;
        private System.Windows.Forms.GroupBox groupBoxConditionalPost;
        private System.Windows.Forms.RadioButton radioButtonOnlySuccessAndFail;
        private System.Windows.Forms.RadioButton radioButtonOnlyFail;
        private System.Windows.Forms.RadioButton radioButtonOnlySuccess;
        private System.Windows.Forms.GroupBox groupBoxTeam;
        private System.Windows.Forms.ComboBox comboBoxTeam;
        private System.Windows.Forms.Button buttonUnSelectAll;
        private System.Windows.Forms.Button buttonUnSelectWvW;
        private System.Windows.Forms.Button buttonUnSelectAllFractals;
        private System.Windows.Forms.Button buttonUnSelectAllStrikes;
        private System.Windows.Forms.Button buttonUnSelectAllRaids;
        private System.Windows.Forms.Button buttonUnSelectAllGolems;
        private System.Windows.Forms.CheckBox checkBoxAllowUnknownBossIds;
        private System.Windows.Forms.GroupBox groupBoxLogSummaries;
        private System.Windows.Forms.RadioButton radioButtonLogSummaryNone;
        private System.Windows.Forms.RadioButton radioButtonLogSummarySquadAndPlayers;
        private System.Windows.Forms.RadioButton radioButtonLogSummarySquad;
        private System.Windows.Forms.RadioButton radioButtonLogSummaryPlayers;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxIncludeBarrierSummary;
        private System.Windows.Forms.CheckBox checkBoxIncludeHealingSummary;
        private System.Windows.Forms.CheckBox checkBoxIncludeCleansingSummary;
        private System.Windows.Forms.CheckBox checkBoxIncludeStripSummary;
        private System.Windows.Forms.CheckBox checkBoxIncludeCCSummary;
        private System.Windows.Forms.CheckBox checkBoxIncludeDamageSummary;
        private System.Windows.Forms.ComboBox comboBoxMaxPlayers;
        private System.Windows.Forms.CheckBox checkBoxAdjustBarrier;
        private System.Windows.Forms.CheckBox checkBoxIncludeOpponentIcons;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox checkBoxShowFightAwards;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBoxGoogleSheetsUrl;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBoxIncludeDownsContributionSummary;
        private System.Windows.Forms.GroupBox groupBoxIncludeCheckboxes;
        private System.Windows.Forms.CheckBox checkBoxIncludeLegendaryChallengeModeLogs;
        private System.Windows.Forms.CheckBox checkBoxIncludeChallengeModeLogs;
        private System.Windows.Forms.CheckBox checkBoxIncludeNormalLogs;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textboxBlueTeamIds;
        private System.Windows.Forms.TextBox textboxRedTeamIds;
        private System.Windows.Forms.TextBox textboxGreenTeamIds;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBoxIncludeStabilitySummary;
        private System.Windows.Forms.CheckBox checkBoxIncludeIncomingDefensiveStats;
    }
}