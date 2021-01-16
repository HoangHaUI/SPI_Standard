using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SPI_AOI.Views.MainConfigWindow
{
    public partial class AlarmForm : Form
    {
        private System.Timers.Timer mTimer = new System.Timers.Timer(200);
        private bool mIsInTimer = false;
        private bool mIsShow = true;
        private string[] mAlarmBits = new string[0];
        private string[] mAlarmMsg = new string[0];
        private string[] mAlarmSolution = new string[0];
        private Devices.PLCComm mPLCComm = new Devices.PLCComm();
        public AlarmForm()
        {
            InitializeComponent();
        }

        
        private void AlarmForm_Load(object sender, EventArgs e)
        {
            mAlarmBits = Heal.UI.MachineIssueForm.GetErrorBits();
            mAlarmMsg = Heal.UI.MachineIssueForm.GetErrorMessages();
            mAlarmSolution = Heal.UI.MachineIssueForm.GetErrorSolutions();
            mTimer.Elapsed += OnTimedEvent;
            mTimer.Enabled = true;
        }
        private void OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (mIsInTimer)
                return;
            mIsInTimer = true;
            mTimer.Enabled = false;
            QueryAlarm();
            mIsInTimer = false;
            if(mIsShow)
                mTimer.Enabled = true;
        }
        private void QueryAlarm()
        {
            string erCodes = string.Empty;
            string erMsg = string.Empty;
            string erSolution = string.Empty;
            Utils.MachineAlarmResult result = new Utils.MachineAlarmResult();
            result.Success = true;
            for (int i = 0; i < mAlarmBits.Length; i++)
            {
                int flag = mPLCComm.GetDevice(mAlarmBits[i]);
                if (flag == 1)
                {
                    if (!string.IsNullOrEmpty(result.ErCode))
                    {
                        result.ErCode += ", ";
                    }
                    result.ErCode += mAlarmBits[i];
                    if (!string.IsNullOrEmpty(result.ErMessages))
                    {
                        result.ErMessages += System.Environment.NewLine;
                    }
                    result.ErMessages += string.Format("({0}): {1}", mAlarmBits[i], mAlarmMsg[i]);
                    if (!string.IsNullOrEmpty(result.ErSolution))
                    {
                        result.ErSolution += System.Environment.NewLine;
                    }
                    if (mAlarmSolution[i] != string.Empty)
                    {
                        result.ErSolution += string.Format("({0}): {1}", mAlarmBits[i], mAlarmSolution[i]);
                    }
                    else
                    {
                        result.ErSolution += string.Format("({0}): Not yet solution...", mAlarmBits[i]);
                    }

                }
                else if (flag == -1)
                {
                    result.Success = false;
                    break;
                }
            }
            if(mPLCComm.Get_Error_Machine() == 0)
            {
                this.Invoke(new Action(() =>
                {
                    this.Close();
                }));
            }
            if(result.Success)
            {
                
                try
                {
                    this.Invoke(new Action(() => {
                        txtErrorCode.Text = result.ErCode;
                        txtErrorName.Text = result.ErMessages;
                        txtSolution.Text = result.ErSolution;
                        this.Update();
                    }));
                }
                catch { }
                
            }
        }
        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void AlarmForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            mIsShow = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
