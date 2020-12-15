using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using NLog;
using System.Threading;

namespace Heal.UI
{
    public partial class MachineIssueForm : Form
    {
        private static SQLiteConnection mConn = new SQLiteConnection(string.Format("Data Source={0};Version=3;", DefineTbl.DBName));
        private Logger mLog = Heal.LogCtl.GetInstance();
        public MachineIssueForm()
        {
            InitializeComponent();
        }
        private void Errors_Load(object sender, EventArgs e)
        {
            CreateTable(DefineTbl.TblName, new string[] { DefineTbl.ColBit, DefineTbl.ColMessage, DefineTbl.ColSolution });
            txtBit_TextChanged(null, null);
            UpdateTable();
        }
        public bool Insert(string TableName, string[] Values)
        {
            bool rep = true;
            mConn.Open();
            string values = string.Empty;
            for (int i = 0; i < Values.Length; i++)
            {
                values += string.Format("{0}", Values[i]);
                if (Values.Length - 1 != i)
                {
                    values += ",";
                }
            }
            using (SQLiteCommand command = mConn.CreateCommand())
            {
                try
                {
                    command.CommandText = string.Format("INSERT INTO {0} VALUES({1})", TableName, values);
                    command.ExecuteNonQuery();
                }
                catch (SQLiteException ex)
                {
                    mLog.Error(ex.Message);
                    rep = false;
                }
            }
            mConn.Close();
            return rep;
        }
        public object FindOne(string TableName, string ColSearch, string ColCondition, string Values)
        {
            object result = null;
            mConn.Open();
            using (SQLiteCommand command = mConn.CreateCommand())
            {
                try
                {
                    command.CommandText = string.Format("SELECT {0} FROM {1} WHERE {2}={3}", ColSearch, TableName, ColCondition, Values);
                    result = command.ExecuteScalar();
                }
                catch (SQLiteException ex)
                {
                    mLog.Error(ex.Message);
                }
            }
            mConn.Close();
            return result;
        }
        public List<object[]> FindMany(string TableName, string ColSearch, string ColCondition = null, string Values = null)
        {
            mConn.Open();
            List<object[]> result = new List<object[]>();
            using (SQLiteCommand command = mConn.CreateCommand())
            {
                try
                {
                    if (ColCondition == null)
                    {
                        command.CommandText = string.Format("SELECT {0} FROM {1}", ColSearch, TableName);
                    }
                    else
                    {
                        command.CommandText = string.Format("SELECT {0} FROM {1} WHERE {2}={3}", ColSearch, TableName, ColCondition, Values);
                    }
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        object[] obj = new object[reader.VisibleFieldCount];
                        for (int i = 0; i < obj.Length; i++)
                        {
                            obj[i] = reader.GetValue(i);
                        }
                        result.Add(obj);
                    }
                    reader.Close();
                }
                catch (SQLiteException ex)
                {
                    mLog.Error(ex.Message);
                }

            }
            mConn.Close();
            return result;
        }
        public bool UpdateOne(string TableName, string ColSet, string NewValue, string ColCondition, string Values)
        {
            bool rep = true;
            mConn.Open();
            using (SQLiteCommand command = mConn.CreateCommand())
            {
                try
                {
                    command.CommandText = string.Format("UPDATE {0} SET {1} = {2} WHERE {3} = {4}", TableName, ColSet, NewValue, ColCondition, Values);
                    command.ExecuteNonQuery();
                }
                catch (SQLiteException ex)
                {
                    mLog.Error(ex.Message);
                    rep = false;
                }
            }
            mConn.Close();
            return rep;
        }
        public bool UpdateMany(string TableName, string ColCondition, string Values, string[] ColSet, string[] NewValue)
        {
            bool rep = true;
            string cmd = "";
            if (ColSet.Length != NewValue.Length)
                return false;
            for (int i = 0; i < ColSet.Length; i++)
            {
                cmd += string.Format("{0} = {1}", ColSet[i], NewValue[i]);
                if (i != ColSet.Length - 1)
                    cmd += ",";
            }

            mConn.Open();
            using (SQLiteCommand command = mConn.CreateCommand())
            {
                try
                {
                    command.CommandText = string.Format("UPDATE {0} SET {1} WHERE {2} = {3}", TableName, cmd, ColCondition, Values);
                    command.ExecuteNonQuery();
                }
                catch (SQLiteException ex)
                {
                    mLog.Error(ex.Message);
                    rep = false;
                }
            }
            mConn.Close();
            Thread.Sleep(10);
            return rep;
        }
        public bool Delete(string TableName, string ColCondition, string Values)
        {
            bool rep = true;
            mConn.Open();
            using (SQLiteCommand command = mConn.CreateCommand())
            {
                try
                {
                    command.CommandText = string.Format("DELETE FROM {0} WHERE {1} = {2}", TableName, ColCondition, Values);
                    command.ExecuteNonQuery();
                }
                catch (SQLiteException ex)
                {
                    mLog.Error(ex.Message);
                    rep = false;
                }
            }
            mConn.Close();
            return rep;
        }
        public DataSet FillData(string TableName)
        {
            DataSet ds = null;
            mConn.Open();
            using (SQLiteCommand command = mConn.CreateCommand())
            {
                try
                {
                    string cmd = string.Format("SELECT * FROM {0}", TableName);
                    SQLiteDataAdapter da = new SQLiteDataAdapter(cmd, mConn);
                    ds = new DataSet();
                    da.Fill(ds);
                }
                catch (SQLiteException ex)
                {
                    mLog.Error(ex.Message);
                }
            }
            mConn.Close();
            return ds;
        }
        
        public bool CreateTable(string TableName, string[] TableContent)
        {
            bool rep = true;
            mConn.Open();
            string contentTbl = string.Empty;
            for (int i = 0; i < TableContent.Length; i++)
            {
                contentTbl += TableContent[i];
                if (i != TableContent.Length - 1)
                {
                    contentTbl += ",";
                }
            }
            using (SQLiteCommand command = mConn.CreateCommand())
            {
                try
                {
                    command.CommandText = string.Format("CREATE TABLE IF NOT EXISTS {0} ({1});", TableName, contentTbl);
                    var rsl = command.ExecuteNonQuery();
                }
                catch (SQLiteException ex)
                {
                    mLog.Error(ex.Message);
                    rep = false;
                }
            }
            mConn.Close();
            return rep;
        }
        private void btAdd_Click(object sender, EventArgs e)
        {
            if(txtBit.Text != "")
            {
                if(txtMessage.Text != "")
                {
                    Insert(DefineTbl.TblName, new string[] { "\'" + txtBit.Text + "\'", "\'" + txtMessage.Text + "\'" , "\'" + txtSolution.Text + "\'" });
                    MessageBox.Show("Add bit \"" + txtBit.Text + "\" with \"" + txtMessage.Text + "\" message successfuly!", "INFOMATION", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtBit.Text = "";
                    txtMessage.Text = "";
                    txtSolution.Text = "";
                    UpdateTable();

                }
                else
                {
                    MessageBox.Show("Please enter message error...", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please enter Bit error ...", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void txtBit_TextChanged(object sender, EventArgs e)
        {
            object msg = FindOne(DefineTbl.TblName, DefineTbl.ColMessage,
                DefineTbl.ColBit, string.Format("'{0}'", txtBit.Text));
            object solution = FindOne(DefineTbl.TblName, DefineTbl.ColSolution,
                DefineTbl.ColBit, string.Format("'{0}'", txtBit.Text));
            if (msg != null)
            {
                txtMessage.Text = Convert.ToString(msg);
                txtSolution.Text = Convert.ToString(solution);
            }
            else
            {
                txtMessage.Text = string.Empty;
                txtSolution.Text = string.Empty;
            }
            btAdd.Visible = !(msg != null);
            btModify.Visible = (msg != null);
            btDelete.Visible = (msg != null);
        }

        private void btModify_Click(object sender, EventArgs e)
        {
            UpdateOne(DefineTbl.TblName, DefineTbl.ColMessage, string.Format("'{0}'", 
                txtMessage.Text), DefineTbl.ColBit, string.Format("'{0}'", txtBit.Text));
            UpdateOne(DefineTbl.TblName, DefineTbl.ColSolution, string.Format("'{0}'",
                txtSolution.Text), DefineTbl.ColBit, string.Format("'{0}'", txtBit.Text));
            MessageBox.Show("Update successfuly!", "INFOMATION", MessageBoxButtons.OK, MessageBoxIcon.Information);
            txtBit.Text = "";
            txtMessage.Text = "";
            txtSolution.Text = "";
            UpdateTable();
            
        }
        private void UpdateTable()
        {
            List<object[]> reader = FindMany(DefineTbl.TblName, "*");
            dgvErrors.Rows.Clear();
            for (int i = 0; i < reader.Count; i++)
            {
                dgvErrors.Rows.Add(new object[] {
                reader[i][0],
                reader[i][1],
                reader[i][2]
                });
            }
        }

        private void dgvErrors_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex > -1 && e.RowIndex < dgvErrors.RowCount -1)
            {
                
                DataGridViewRow row = new DataGridViewRow();
                row = dgvErrors.Rows[e.RowIndex];
                txtBit.Text = row.Cells[0].Value.ToString();
                txtMessage.Text = row.Cells[1].Value.ToString();
                txtSolution.Text = row.Cells[2].Value.ToString();
            }
            
            
        }

        private void btDelete_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Are You want to delete \"" + txtBit.Text + "\" with \"" + txtMessage.Text + "\" message?", "INFOMATION", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Delete(DefineTbl.TblName, DefineTbl.ColBit, string.Format("'{0}'", txtBit.Text));
                MessageBox.Show("Delete successfuly!", "INFOMATION", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtBit.Text = "";
                txtMessage.Text = "";
                txtSolution.Text = "";
                UpdateTable();
                
            }
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
    class DefineTbl
    {
        public static string TblName = "ERROR_MACHINE";
        public static string ColBit = "Bit";
        public static string ColMessage = "Message";
        public static string ColSolution = "Solution";
        public static string DBName = "db.sqlite";
    }
}
