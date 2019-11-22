﻿using System.Windows.Forms;
using System.Collections.Generic;
using PlenBotLogUploader.DPSReport;

namespace PlenBotLogUploader
{
    public partial class FormEditBossData : Form
    {
        #region definitions
        // fields
        private FormBossData editLink;
        private BossData data;
        private int reservedId;
        private Dictionary<int, BossData> allBosses = Bosses.GetAllBosses();
        #endregion

        public FormEditBossData(FormBossData editLink, BossData data, int reservedId)
        {
            this.editLink = editLink;
            InitializeComponent();
            this.data = data;
            this.reservedId = reservedId;
            Icon = Properties.Resources.AppIcon;
            if (data == null)
            {
                Text = "Add a new boss";
            }
            else
            {
                Text = $"{data.Name} ({data.BossId})";
            }
            textBoxBossID.Text = data?.BossId.ToString() ?? "";
            textBoxBossName.Text = data?.Name ?? "";
            textBoxSuccessMsg.Text = data?.SuccessMsg ?? "<boss> kill: <log>";
            textBoxFailMsg.Text = data?.FailMsg ?? "<boss> pull: <log>";
            textBoxIcon.Text = data?.Icon ?? "";
            switch(data?.Type ?? BossType.None)
            {
                case BossType.Raid:
                    radioButtonTypeRaid.Checked = true;
                    break;
                case BossType.Fractal:
                    radioButtonTypeFractal.Checked = true;
                    break;
                case BossType.Strike:
                    radioButtonTypeStrike.Checked = true;
                    break;
                case BossType.Golem:
                    radioButtonTypeGolem.Checked = true;
                    break;
                case BossType.WvW:
                    radioButtonTypeWvW.Checked = true;
                    break;
                default:
                    radioButtonTypeNone.Checked = true;
                    break;
            }
            checkBoxEvent.Checked = data?.Event ?? false;
        }

        private void FormEditBossData_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (int.TryParse(textBoxBossID.Text, out int bossId))
            {
                if (textBoxBossName.Text != "")
                {
                    if (data == null)
                    {
                        BossType type = BossType.None;
                        if (radioButtonTypeRaid.Checked)
                        {
                            type = BossType.Raid;
                        }
                        else if (radioButtonTypeFractal.Checked)
                        {
                            type = BossType.Fractal;
                        }
                        else if (radioButtonTypeStrike.Checked)
                        {
                            type = BossType.Strike;
                        }
                        else if (radioButtonTypeGolem.Checked)
                        {
                            type = BossType.Golem;
                        }
                        else if (radioButtonTypeWvW.Checked)
                        {
                            type = BossType.WvW;
                        }
                        allBosses[reservedId] = new BossData()
                        {
                            BossId = bossId,
                            Name = textBoxBossName.Text,
                            SuccessMsg = textBoxSuccessMsg.Text,
                            FailMsg = textBoxFailMsg.Text,
                            Icon = textBoxIcon.Text,
                            Type = type,
                            Event = checkBoxEvent.Checked
                        };
                        editLink.listViewBosses.Items.Add(new ListViewItem() { Name = reservedId.ToString(), Text = textBoxBossName.Text });
                    }
                    else
                    {
                        var boss = allBosses[reservedId];
                        boss.BossId = bossId;
                        boss.Name = textBoxBossName.Text;
                        boss.SuccessMsg = textBoxSuccessMsg.Text;
                        boss.FailMsg = textBoxFailMsg.Text;
                        boss.Icon = textBoxIcon.Text;
                        BossType type = BossType.None;
                        if(radioButtonTypeRaid.Checked)
                        {
                            type = BossType.Raid;
                        }
                        else if (radioButtonTypeFractal.Checked)
                        {
                            type = BossType.Fractal;
                        }
                        else if (radioButtonTypeStrike.Checked)
                        {
                            type = BossType.Strike;
                        }
                        else if (radioButtonTypeGolem.Checked)
                        {
                            type = BossType.Golem;
                        }
                        else if (radioButtonTypeWvW.Checked)
                        {
                            type = BossType.WvW;
                        }
                        boss.Type = type;
                        boss.Event = checkBoxEvent.Checked;
                        editLink.listViewBosses.Items[editLink.listViewBosses.Items.IndexOfKey(reservedId.ToString())].Text = textBoxBossName.Text;
                    }
                }
            }
        }
    }
}
