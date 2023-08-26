namespace PlenBotLogUploader
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
            checkBoxPlayers = new System.Windows.Forms.CheckBox();
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
            groupBox1 = new System.Windows.Forms.GroupBox();
            checkBoxAdjustBarrier = new System.Windows.Forms.CheckBox();
            comboBoxMaxPlayers = new System.Windows.Forms.ComboBox();
            label1 = new System.Windows.Forms.Label();
            checkBoxIncludeCCSummary = new System.Windows.Forms.CheckBox();
            checkBoxIncludeStripSummary = new System.Windows.Forms.CheckBox();
            checkBoxIncludeCleansingSummary = new System.Windows.Forms.CheckBox();
            checkBoxIncludeHealingSummary = new System.Windows.Forms.CheckBox();
            checkBoxIncludeBarrierSummary = new System.Windows.Forms.CheckBox();
            checkBoxIncludeDamageSummary = new System.Windows.Forms.CheckBox();
            checkBoxShowDPSColumn = new System.Windows.Forms.CheckBox();
            checkBoxIncludeOpponentIcons = new System.Windows.Forms.CheckBox();
            groupBoxWebhookInfo.SuspendLayout();
            groupBoxBossesEnable.SuspendLayout();
            groupBoxConditionalPost.SuspendLayout();
            groupBoxTeam.SuspendLayout();
            groupBox1.SuspendLayout();
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
            textBoxName.Size = new System.Drawing.Size(439, 23);
            textBoxName.TabIndex = 2;
            // 
            // textBoxUrl
            // 
            textBoxUrl.Location = new System.Drawing.Point(10, 82);
            textBoxUrl.Margin = new System.Windows.Forms.Padding(4);
            textBoxUrl.Name = "textBoxUrl";
            textBoxUrl.Size = new System.Drawing.Size(439, 23);
            textBoxUrl.TabIndex = 3;
            // 
            // checkBoxPlayers
            // 
            checkBoxPlayers.AutoSize = true;
            checkBoxPlayers.Location = new System.Drawing.Point(10, 112);
            checkBoxPlayers.Margin = new System.Windows.Forms.Padding(4);
            checkBoxPlayers.Name = "checkBoxPlayers";
            checkBoxPlayers.Size = new System.Drawing.Size(369, 19);
            checkBoxPlayers.TabIndex = 5;
            checkBoxPlayers.Text = "Show detailed information about players and squad performance";
            checkBoxPlayers.UseVisualStyleBackColor = true;
            // 
            // checkedListBoxBossesEnable
            // 
            checkedListBoxBossesEnable.BorderStyle = System.Windows.Forms.BorderStyle.None;
            checkedListBoxBossesEnable.FormattingEnabled = true;
            checkedListBoxBossesEnable.Location = new System.Drawing.Point(7, 22);
            checkedListBoxBossesEnable.Margin = new System.Windows.Forms.Padding(4);
            checkedListBoxBossesEnable.Name = "checkedListBoxBossesEnable";
            checkedListBoxBossesEnable.Size = new System.Drawing.Size(442, 450);
            checkedListBoxBossesEnable.TabIndex = 6;
            // 
            // groupBoxWebhookInfo
            // 
            groupBoxWebhookInfo.Controls.Add(textBoxName);
            groupBoxWebhookInfo.Controls.Add(labelName);
            groupBoxWebhookInfo.Controls.Add(labelUrl);
            groupBoxWebhookInfo.Controls.Add(checkBoxPlayers);
            groupBoxWebhookInfo.Controls.Add(textBoxUrl);
            groupBoxWebhookInfo.Location = new System.Drawing.Point(14, 14);
            groupBoxWebhookInfo.Margin = new System.Windows.Forms.Padding(4);
            groupBoxWebhookInfo.Name = "groupBoxWebhookInfo";
            groupBoxWebhookInfo.Padding = new System.Windows.Forms.Padding(4);
            groupBoxWebhookInfo.Size = new System.Drawing.Size(458, 140);
            groupBoxWebhookInfo.TabIndex = 7;
            groupBoxWebhookInfo.TabStop = false;
            groupBoxWebhookInfo.Text = "Webhook info";
            // 
            // groupBoxBossesEnable
            // 
            groupBoxBossesEnable.Controls.Add(checkBoxAllowUnknownBossIds);
            groupBoxBossesEnable.Controls.Add(buttonUnSelectAllGolems);
            groupBoxBossesEnable.Controls.Add(buttonUnSelectWvW);
            groupBoxBossesEnable.Controls.Add(buttonUnSelectAllFractals);
            groupBoxBossesEnable.Controls.Add(buttonUnSelectAllStrikes);
            groupBoxBossesEnable.Controls.Add(buttonUnSelectAllRaids);
            groupBoxBossesEnable.Controls.Add(buttonUnSelectAll);
            groupBoxBossesEnable.Controls.Add(checkedListBoxBossesEnable);
            groupBoxBossesEnable.Location = new System.Drawing.Point(479, 14);
            groupBoxBossesEnable.Margin = new System.Windows.Forms.Padding(4);
            groupBoxBossesEnable.Name = "groupBoxBossesEnable";
            groupBoxBossesEnable.Padding = new System.Windows.Forms.Padding(4);
            groupBoxBossesEnable.Size = new System.Drawing.Size(458, 539);
            groupBoxBossesEnable.TabIndex = 8;
            groupBoxBossesEnable.TabStop = false;
            groupBoxBossesEnable.Text = "Only upload for selected bosses";
            // 
            // checkBoxAllowUnknownBossIds
            // 
            checkBoxAllowUnknownBossIds.AutoSize = true;
            checkBoxAllowUnknownBossIds.Location = new System.Drawing.Point(7, 480);
            checkBoxAllowUnknownBossIds.Name = "checkBoxAllowUnknownBossIds";
            checkBoxAllowUnknownBossIds.Size = new System.Drawing.Size(256, 19);
            checkBoxAllowUnknownBossIds.TabIndex = 13;
            checkBoxAllowUnknownBossIds.Text = "Allow unknown bosses to use this webhook";
            checkBoxAllowUnknownBossIds.UseVisualStyleBackColor = true;
            // 
            // buttonUnSelectAllGolems
            // 
            buttonUnSelectAllGolems.Location = new System.Drawing.Point(316, 507);
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
            buttonUnSelectWvW.Location = new System.Drawing.Point(386, 507);
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
            buttonUnSelectAllFractals.Location = new System.Drawing.Point(176, 507);
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
            buttonUnSelectAllStrikes.Location = new System.Drawing.Point(246, 507);
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
            buttonUnSelectAllRaids.Location = new System.Drawing.Point(106, 507);
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
            buttonUnSelectAll.Location = new System.Drawing.Point(5, 507);
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
            groupBoxConditionalPost.Location = new System.Drawing.Point(14, 162);
            groupBoxConditionalPost.Margin = new System.Windows.Forms.Padding(4);
            groupBoxConditionalPost.Name = "groupBoxConditionalPost";
            groupBoxConditionalPost.Padding = new System.Windows.Forms.Padding(4);
            groupBoxConditionalPost.Size = new System.Drawing.Size(458, 76);
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
            groupBoxTeam.Location = new System.Drawing.Point(14, 246);
            groupBoxTeam.Margin = new System.Windows.Forms.Padding(4);
            groupBoxTeam.Name = "groupBoxTeam";
            groupBoxTeam.Padding = new System.Windows.Forms.Padding(4);
            groupBoxTeam.Size = new System.Drawing.Size(458, 54);
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
            comboBoxTeam.Size = new System.Drawing.Size(439, 23);
            comboBoxTeam.TabIndex = 0;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(checkBoxIncludeOpponentIcons);
            groupBox1.Controls.Add(checkBoxShowDPSColumn);
            groupBox1.Controls.Add(checkBoxAdjustBarrier);
            groupBox1.Controls.Add(comboBoxMaxPlayers);
            groupBox1.Controls.Add(checkBoxIncludeCCSummary);
            groupBox1.Controls.Add(checkBoxIncludeStripSummary);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(checkBoxIncludeCleansingSummary);
            groupBox1.Controls.Add(checkBoxIncludeHealingSummary);
            groupBox1.Controls.Add(checkBoxIncludeBarrierSummary);
            groupBox1.Controls.Add(checkBoxIncludeDamageSummary);
            groupBox1.Location = new System.Drawing.Point(14, 308);
            groupBox1.Margin = new System.Windows.Forms.Padding(4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(4);
            groupBox1.Size = new System.Drawing.Size(458, 245);
            groupBox1.TabIndex = 10;
            groupBox1.TabStop = false;
            groupBox1.Text = "Player report information";
            // 
            // checkBoxAdjustBarrier
            // 
            checkBoxAdjustBarrier.AutoSize = true;
            checkBoxAdjustBarrier.Enabled = false;
            checkBoxAdjustBarrier.Location = new System.Drawing.Point(201, 78);
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
            comboBoxMaxPlayers.Location = new System.Drawing.Point(270, 211);
            comboBoxMaxPlayers.Name = "comboBoxMaxPlayers";
            comboBoxMaxPlayers.Size = new System.Drawing.Size(49, 23);
            comboBoxMaxPlayers.TabIndex = 14;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(10, 214);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(253, 15);
            label1.TabIndex = 13;
            label1.Text = "Number of players to include in summary lists:";
            // 
            // checkBoxIncludeCCSummary
            // 
            checkBoxIncludeCCSummary.AutoSize = true;
            checkBoxIncludeCCSummary.Location = new System.Drawing.Point(10, 159);
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
            checkBoxIncludeStripSummary.Location = new System.Drawing.Point(10, 132);
            checkBoxIncludeStripSummary.Margin = new System.Windows.Forms.Padding(4);
            checkBoxIncludeStripSummary.Name = "checkBoxIncludeStripSummary";
            checkBoxIncludeStripSummary.Size = new System.Drawing.Size(144, 19);
            checkBoxIncludeStripSummary.TabIndex = 10;
            checkBoxIncludeStripSummary.Text = "Include strip summary";
            checkBoxIncludeStripSummary.UseVisualStyleBackColor = true;
            // 
            // checkBoxIncludeCleansingSummary
            // 
            checkBoxIncludeCleansingSummary.AutoSize = true;
            checkBoxIncludeCleansingSummary.Location = new System.Drawing.Point(10, 105);
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
            checkBoxIncludeHealingSummary.Location = new System.Drawing.Point(10, 51);
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
            checkBoxIncludeBarrierSummary.Location = new System.Drawing.Point(10, 78);
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
            checkBoxIncludeDamageSummary.Location = new System.Drawing.Point(10, 24);
            checkBoxIncludeDamageSummary.Margin = new System.Windows.Forms.Padding(4);
            checkBoxIncludeDamageSummary.Name = "checkBoxIncludeDamageSummary";
            checkBoxIncludeDamageSummary.Size = new System.Drawing.Size(164, 19);
            checkBoxIncludeDamageSummary.TabIndex = 5;
            checkBoxIncludeDamageSummary.Text = "Include damage summary";
            checkBoxIncludeDamageSummary.UseVisualStyleBackColor = true;
            checkBoxIncludeDamageSummary.CheckedChanged += checkBoxIncludeDamageSummary_CheckedChanged;
            // 
            // checkBoxShowDPSColumn
            // 
            checkBoxShowDPSColumn.AutoSize = true;
            checkBoxShowDPSColumn.Enabled = false;
            checkBoxShowDPSColumn.Location = new System.Drawing.Point(201, 24);
            checkBoxShowDPSColumn.Margin = new System.Windows.Forms.Padding(4);
            checkBoxShowDPSColumn.Name = "checkBoxShowDPSColumn";
            checkBoxShowDPSColumn.Size = new System.Drawing.Size(123, 19);
            checkBoxShowDPSColumn.TabIndex = 16;
            checkBoxShowDPSColumn.Text = "Show DPS column";
            checkBoxShowDPSColumn.UseVisualStyleBackColor = true;
            // 
            // checkBoxIncludeOpponentIcons
            // 
            checkBoxIncludeOpponentIcons.AutoSize = true;
            checkBoxIncludeOpponentIcons.Location = new System.Drawing.Point(10, 186);
            checkBoxIncludeOpponentIcons.Margin = new System.Windows.Forms.Padding(4);
            checkBoxIncludeOpponentIcons.Name = "checkBoxIncludeOpponentIcons";
            checkBoxIncludeOpponentIcons.Size = new System.Drawing.Size(151, 19);
            checkBoxIncludeOpponentIcons.TabIndex = 17;
            checkBoxIncludeOpponentIcons.Text = "Include opponent icons";
            checkBoxIncludeOpponentIcons.UseVisualStyleBackColor = true;
            // 
            // FormEditDiscordWebhook
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.White;
            ClientSize = new System.Drawing.Size(948, 566);
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
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Label labelUrl;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.TextBox textBoxUrl;
        private System.Windows.Forms.CheckBox checkBoxPlayers;
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
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox checkBoxIncludeBarrierSummary;
        private System.Windows.Forms.CheckBox checkBoxIncludeHealingSummary;
        private System.Windows.Forms.CheckBox checkBoxIncludeCleansingSummary;
        private System.Windows.Forms.CheckBox checkBoxIncludeStripSummary;
        private System.Windows.Forms.CheckBox checkBoxIncludeCCSummary;
        private System.Windows.Forms.CheckBox checkBoxIncludeDamageSummary;
        private System.Windows.Forms.ComboBox comboBoxMaxPlayers;
        private System.Windows.Forms.CheckBox checkBoxAdjustBarrier;
        private System.Windows.Forms.CheckBox checkBoxIncludeOpponentIcons;
        private System.Windows.Forms.CheckBox checkBoxShowDPSColumn;
    }
}