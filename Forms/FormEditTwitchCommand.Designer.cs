﻿namespace PlenBotLogUploader.Forms
{
    partial class FormEditTwitchCommand
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
            groupBoxTwichCommandName = new System.Windows.Forms.GroupBox();
            textBoxTwitchCommandName = new System.Windows.Forms.TextBox();
            groupBoxCommandTrigger = new System.Windows.Forms.GroupBox();
            checkBoxIsRegEx = new System.Windows.Forms.CheckBox();
            textBoxTwitchCommandCommand = new System.Windows.Forms.TextBox();
            groupBoxResponse = new System.Windows.Forms.GroupBox();
            textBoxResponse = new System.Windows.Forms.TextBox();
            groupBoxResponseType = new System.Windows.Forms.GroupBox();
            radioButtonResponseTypeReplyAt = new System.Windows.Forms.RadioButton();
            radioButtonResponseTypeReplyPlain = new System.Windows.Forms.RadioButton();
            labelInfoNonLink = new System.Windows.Forms.Label();
            linkLabelInfoLink = new System.Windows.Forms.LinkLabel();
            groupBoxTwichCommandName.SuspendLayout();
            groupBoxCommandTrigger.SuspendLayout();
            groupBoxResponse.SuspendLayout();
            groupBoxResponseType.SuspendLayout();
            SuspendLayout();
            // 
            // groupBoxTwichCommandName
            // 
            groupBoxTwichCommandName.Controls.Add(textBoxTwitchCommandName);
            groupBoxTwichCommandName.Location = new System.Drawing.Point(13, 14);
            groupBoxTwichCommandName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            groupBoxTwichCommandName.Name = "groupBoxTwichCommandName";
            groupBoxTwichCommandName.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            groupBoxTwichCommandName.Size = new System.Drawing.Size(533, 66);
            groupBoxTwichCommandName.TabIndex = 11;
            groupBoxTwichCommandName.TabStop = false;
            groupBoxTwichCommandName.Text = "Twitch command name (will not save unless it is set)";
            // 
            // textBoxTwitchCommandName
            // 
            textBoxTwitchCommandName.Location = new System.Drawing.Point(8, 30);
            textBoxTwitchCommandName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            textBoxTwitchCommandName.Name = "textBoxTwitchCommandName";
            textBoxTwitchCommandName.Size = new System.Drawing.Size(517, 27);
            textBoxTwitchCommandName.TabIndex = 7;
            // 
            // groupBoxCommandTrigger
            // 
            groupBoxCommandTrigger.Controls.Add(checkBoxIsRegEx);
            groupBoxCommandTrigger.Controls.Add(textBoxTwitchCommandCommand);
            groupBoxCommandTrigger.Location = new System.Drawing.Point(13, 90);
            groupBoxCommandTrigger.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            groupBoxCommandTrigger.Name = "groupBoxCommandTrigger";
            groupBoxCommandTrigger.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            groupBoxCommandTrigger.Size = new System.Drawing.Size(533, 143);
            groupBoxCommandTrigger.TabIndex = 12;
            groupBoxCommandTrigger.TabStop = false;
            groupBoxCommandTrigger.Text = "Twitch command trigger";
            // 
            // checkBoxIsRegEx
            // 
            checkBoxIsRegEx.AutoSize = true;
            checkBoxIsRegEx.Location = new System.Drawing.Point(7, 110);
            checkBoxIsRegEx.Name = "checkBoxIsRegEx";
            checkBoxIsRegEx.Size = new System.Drawing.Size(447, 24);
            checkBoxIsRegEx.TabIndex = 8;
            checkBoxIsRegEx.Text = "Is a regular expression (RegEx) trigger (will always ignore case)";
            checkBoxIsRegEx.UseVisualStyleBackColor = true;
            // 
            // textBoxTwitchCommandCommand
            // 
            textBoxTwitchCommandCommand.Location = new System.Drawing.Point(8, 30);
            textBoxTwitchCommandCommand.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            textBoxTwitchCommandCommand.Multiline = true;
            textBoxTwitchCommandCommand.Name = "textBoxTwitchCommandCommand";
            textBoxTwitchCommandCommand.PlaceholderText = "Command trigger, such as \"!hello\" or \"!lurk\"";
            textBoxTwitchCommandCommand.Size = new System.Drawing.Size(517, 72);
            textBoxTwitchCommandCommand.TabIndex = 7;
            // 
            // groupBoxResponse
            // 
            groupBoxResponse.Controls.Add(textBoxResponse);
            groupBoxResponse.Location = new System.Drawing.Point(12, 243);
            groupBoxResponse.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            groupBoxResponse.Name = "groupBoxResponse";
            groupBoxResponse.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            groupBoxResponse.Size = new System.Drawing.Size(533, 112);
            groupBoxResponse.TabIndex = 13;
            groupBoxResponse.TabStop = false;
            groupBoxResponse.Text = "Twitch command response";
            // 
            // textBoxResponse
            // 
            textBoxResponse.Location = new System.Drawing.Point(8, 30);
            textBoxResponse.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            textBoxResponse.Multiline = true;
            textBoxResponse.Name = "textBoxResponse";
            textBoxResponse.PlaceholderText = "Command response, such as \"Hello!\" or \"Lurking mode engaged MrDestructoid\"";
            textBoxResponse.Size = new System.Drawing.Size(517, 72);
            textBoxResponse.TabIndex = 7;
            // 
            // groupBoxResponseType
            // 
            groupBoxResponseType.Controls.Add(radioButtonResponseTypeReplyAt);
            groupBoxResponseType.Controls.Add(radioButtonResponseTypeReplyPlain);
            groupBoxResponseType.Location = new System.Drawing.Point(13, 365);
            groupBoxResponseType.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            groupBoxResponseType.Name = "groupBoxResponseType";
            groupBoxResponseType.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            groupBoxResponseType.Size = new System.Drawing.Size(533, 62);
            groupBoxResponseType.TabIndex = 14;
            groupBoxResponseType.TabStop = false;
            groupBoxResponseType.Text = "Type of the response";
            // 
            // radioButtonResponseTypeReplyAt
            // 
            radioButtonResponseTypeReplyAt.AutoSize = true;
            radioButtonResponseTypeReplyAt.Location = new System.Drawing.Point(91, 28);
            radioButtonResponseTypeReplyAt.Name = "radioButtonResponseTypeReplyAt";
            radioButtonResponseTypeReplyAt.Size = new System.Drawing.Size(85, 24);
            radioButtonResponseTypeReplyAt.TabIndex = 1;
            radioButtonResponseTypeReplyAt.TabStop = true;
            radioButtonResponseTypeReplyAt.Text = "Reply @";
            radioButtonResponseTypeReplyAt.UseVisualStyleBackColor = true;
            // 
            // radioButtonResponseTypeReplyPlain
            // 
            radioButtonResponseTypeReplyPlain.AutoSize = true;
            radioButtonResponseTypeReplyPlain.Location = new System.Drawing.Point(8, 28);
            radioButtonResponseTypeReplyPlain.Name = "radioButtonResponseTypeReplyPlain";
            radioButtonResponseTypeReplyPlain.Size = new System.Drawing.Size(62, 24);
            radioButtonResponseTypeReplyPlain.TabIndex = 0;
            radioButtonResponseTypeReplyPlain.Text = "Plain";
            radioButtonResponseTypeReplyPlain.UseVisualStyleBackColor = true;
            // 
            // labelInfoNonLink
            // 
            labelInfoNonLink.AutoSize = true;
            labelInfoNonLink.Location = new System.Drawing.Point(59, 432);
            labelInfoNonLink.Name = "labelInfoNonLink";
            labelInfoNonLink.Size = new System.Drawing.Size(418, 20);
            labelInfoNonLink.TabIndex = 15;
            labelInfoNonLink.Text = "For all available variables for responses please look up the list";
            // 
            // linkLabelInfoLink
            // 
            linkLabelInfoLink.AutoSize = true;
            linkLabelInfoLink.Location = new System.Drawing.Point(472, 432);
            linkLabelInfoLink.Name = "linkLabelInfoLink";
            linkLabelInfoLink.Size = new System.Drawing.Size(38, 20);
            linkLabelInfoLink.TabIndex = 16;
            linkLabelInfoLink.TabStop = true;
            linkLabelInfoLink.Text = "here";
            linkLabelInfoLink.VisitedLinkColor = System.Drawing.Color.Blue;
            linkLabelInfoLink.LinkClicked += LinkLabelInfoLink_LinkClicked;
            // 
            // FormEditTwitchCommand
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.White;
            ClientSize = new System.Drawing.Size(558, 461);
            Controls.Add(linkLabelInfoLink);
            Controls.Add(labelInfoNonLink);
            Controls.Add(groupBoxResponseType);
            Controls.Add(groupBoxResponse);
            Controls.Add(groupBoxCommandTrigger);
            Controls.Add(groupBoxTwichCommandName);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "FormEditTwitchCommand";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "FormEditTwitchCommand";
            FormClosing += FormEditTwitchCommand_FormClosing;
            groupBoxTwichCommandName.ResumeLayout(false);
            groupBoxTwichCommandName.PerformLayout();
            groupBoxCommandTrigger.ResumeLayout(false);
            groupBoxCommandTrigger.PerformLayout();
            groupBoxResponse.ResumeLayout(false);
            groupBoxResponse.PerformLayout();
            groupBoxResponseType.ResumeLayout(false);
            groupBoxResponseType.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxTwichCommandName;
        private System.Windows.Forms.TextBox textBoxTwitchCommandName;
        private System.Windows.Forms.GroupBox groupBoxCommandTrigger;
        private System.Windows.Forms.TextBox textBoxTwitchCommandCommand;
        private System.Windows.Forms.CheckBox checkBoxIsRegEx;
        private System.Windows.Forms.GroupBox groupBoxResponse;
        private System.Windows.Forms.TextBox textBoxResponse;
        private System.Windows.Forms.GroupBox groupBoxResponseType;
        private System.Windows.Forms.RadioButton radioButtonResponseTypeReplyAt;
        private System.Windows.Forms.RadioButton radioButtonResponseTypeReplyPlain;
        private System.Windows.Forms.Label labelInfoNonLink;
        private System.Windows.Forms.LinkLabel linkLabelInfoLink;
    }
}