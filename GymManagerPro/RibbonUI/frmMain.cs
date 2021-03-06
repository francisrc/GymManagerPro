﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace GymManagerPro.RibbonUI
{
    public partial class frmMain : Form
    {
        DataTable dataset;
        bool form_loaded;
        int trainer_id;
        int member_id = 0;
        DataLayer.Plan plan;

        public frmMain()
        {
            InitializeComponent();
        }

        // loads data for the specified member
        public void LoadMember(int id)
        {
            DataLayer.Members members = new DataLayer.Members();
            DataLayer.Member member = new DataLayer.Member();

            try
            {
                // retrieves member data from db
                member = members.GetMember(id);

                // populate controls with the data  
                txtCardNumber.Text = member.CardNumber.ToString();
                txtCardNumber2.Text = member.CardNumber.ToString();
                txtLastName.Text = member.LName;
                txtFirstName.Text = member.FName;

                if (member.Sex == "male")
                {
                    rbMale.Checked = true;
                }
                else if (member.Sex == "female")
                {
                    rbFemale.Checked = true;
                }

                txtDateOfBirth.Value = member.DateOfBirth;
                txtStreet.Text = member.Street;
                txtSuburb.Text = member.Suburb;
                txtCity.Text = member.City;
                txtPostalCode.Text = member.PostalCode.ToString();
                txtHomePhone.Text = member.HomePhone;
                txtCellPhone.Text = member.CellPhone;
                txtEmail.Text = member.Email;
                txtOccupation.Text = member.Occupation;
                txtNotes.Text = member.Notes;
                //cbPersonalTrainer.Text = member.PersonalTrainer;
                cbPersonalTrainer.SelectedValue = member.PersonalTrainer;
                lblName.Text = member.FName + " " + member.LName;
                txtMemberId.Text = id.ToString();

                //display the member's picture
                pictureBoxMemberManager.Image = null; // clears the picturebox
                byte[] img = member.Image;
                if (member.Image != null)
                {
                    try
                    {
                        MemoryStream mstream = new MemoryStream(img);
                        pictureBoxMemberManager.Image = Image.FromStream(mstream);
                    }
                    catch { }
                }

                //load membership data
                LoadMembership(id);
                
                //resetTextBoxes();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        // loads membership data for the specified member
        public void LoadMembership(int id)
        {
            //load membership data
            DataTable table = DataLayer.Memberships.GetMembershipByMemberId(id);
            dataGridViewMemberships.DataSource = table;

            // add column to show if membership is active
            table.Columns.Add(new DataColumn("Status", typeof(string)));

            // loop through all rows to calculate and display if membership is active
            foreach (DataGridViewRow row in dataGridViewMemberships.Rows)
            {
                DateTime end_date = Convert.ToDateTime(row.Cells["End Date"].Value).Date;
                DateTime now = DateTime.Now.Date;
                TimeSpan diff = now - end_date;
                if (diff.TotalDays > 0)
                {
                    row.Cells["Status"].Value = "Inactive";
                   // dataGridViewMemberships.Columns["Status"].DefaultCellStyle.ForeColor = Color.Red;
                }

                else
                {
                    row.Cells["Status"].Value = "Active";
                }
                    
            }
        }

        // populates listbox with the names of all the trainers
        public void LoadAllTrainerNames()
        {
            listBoxTrainers.DataSource = new BindingSource(DataLayer.Trainers.GetAllTrainers(), null);
            listBoxTrainers.DisplayMember = "Value";
            listBoxTrainers.ValueMember = "Key";
        }

        // loads data for the specified trainer
        public void LoadTrainer(int id)
        {
            DataLayer.Trainer trainer = new DataLayer.Trainer();
            try
            {
                // display trainer details
                trainer = DataLayer.Trainers.GetTrainer(id);
                SetUpTrainerTextBoxes(trainer);

                // display associated members
                DataTable membersTable = DataLayer.Trainers.GetAssociatedMembers(id);
                amTrainersDataGridViewX.DataSource = membersTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        // populates textboxes with trainer's data
        public void SetUpTrainerTextBoxes(DataLayer.Trainer trainer)
        {
            txtTrainerFName.Text = trainer.FName;
            txtTrainerLName.Text = trainer.LName;
            txtTrainerCellPhone.Text = trainer.CellPhone;
            txtTrainerCity.Text = trainer.City;
            dtpTrainerDOB.Value = trainer.DateOfBirth.Date;
            txtTrainerEmail.Text = trainer.Email;
            txtTrainerHomePhone.Text = trainer.HomePhone;
            //txtId.Text = id.ToString();
            txtTrainerNotes.Text = trainer.Notes;
            if (trainer.Sex == "Male")
                rbTrainerMale.Checked = true;
            else if (trainer.Sex == "Female")
                rbTrainerFemale.Checked = true;
            //txtPostalCode.Text = trainer.PostalCode.ToString();
            txtTrainerSalary.Text = trainer.Salary.ToString();
            txtTrainerSuburb.Text = trainer.Suburb;
            //label15.Text = txtTrainerFName.Text + " " + txtTrainerLName.Text + " is set as a Personal Trainer for the following members.";

            //display the trainer's picture
            //pictureBox1.Image = null; // clears the picturebox
            //byte[] img = trainer.Image;
            //if (trainer.Image != null)
            //{
            //    try
            //    {
            //        MemoryStream mstream = new MemoryStream(img);
            //        pictureBox1.Image = Image.FromStream(mstream);
            //    }
            //    catch { }
            //}
        }

        // populates richTextBox Attedance with checkins data
        public void SetUpAttedance()
        {
            //get data from db
            richTextBoxAttedance.Text = DataLayer.Members.GetAttedance().ToString();
            // format textbox
            string active = "Active - Entrance allowed";
            string inactive = "Inactive - Entrance denied";
            Utility.HighlightText(richTextBoxAttedance, active, Color.Green);
            Utility.HighlightText(richTextBoxAttedance, inactive, Color.Red);
        }

        // sets up auto-complete search boxes
        private void SetUpSearch()
        {
            txtMembersSearch.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtMembersSearch.AutoCompleteSource = AutoCompleteSource.CustomSource;
            AutoCompleteStringCollection coll = new AutoCompleteStringCollection();
            coll.AddRange(DataLayer.Members.AutoCompleteSearch().ToArray());
            txtMembersSearch.AutoCompleteCustomSource = coll;

            txtAttedanceSearch.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtAttedanceSearch.AutoCompleteSource = AutoCompleteSource.CustomSource;
            AutoCompleteStringCollection coll2 = new AutoCompleteStringCollection();
            coll2.AddRange(DataLayer.Members.AutoCompleteMemberIdSearch().ToArray());
            txtAttedanceSearch.AutoCompleteCustomSource = coll2;
        }

        // displays notifications in member manager
        private void SetUpNotifications()
        {
            lblNotifications.Text = "";                                         // clear
            foreach (DataGridViewRow row in dataGridViewMemberships.Rows)
            {
                DateTime exp_date = (DateTime)row.Cells["End Date"].Value;      // get end date
                String membership = row.Cells["Name"].Value.ToString();         // get membership name
                double days = (exp_date - DateTime.Today).TotalDays;            // calculate the difference between today and end date
                if (days > 0)
                    // membership is active
                    lblNotifications.Text += membership + " expires in " + (int)days + " days" + Environment.NewLine;
                else
                    // membership has expired
                    lblNotifications.Text += membership + " has expired!" + Environment.NewLine;
            }
        }

        // reloads data to AllMembers datagridview to refresh
        private void RefreshAllMembersDataGrid()
        {
            // get all members and bind them to the members datagridview to reload
            BindingSource bSource = new BindingSource();
            dataset = DataLayer.Members.GetAllMembers();
            bSource.DataSource = dataset;
            membersDataGridViewX.DataSource = bSource;

            //set comboboxes to default value
            cbFindPersonalTrainer.SelectedIndex = 0;
            cbFindPlan.SelectedIndex = 0;
        }

        // shows the specified panel
        private void SwitchToPanel(Panel panel)
        {
            // hide all panels
            panelPlans.Visible = false;
            panelAllMembers.Visible = false;
            panelMemberManager.Visible = false;
            panelTrainers.Visible = false;
            panelAttedance.Visible = false;
            panelNewMemberWizard.Visible = false;
            panelReports.Visible = false;

            panel.Visible = true;
        }

        // sets the textboxes as editables for editing trainer details
        private void EditTrainer()
        {
            if (panelTrainers.Visible)
            {
                if (btnTrainersEdit.Text == "Edit")
                {
                    txtTrainerFName.ReadOnly = false;
                    txtTrainerLName.ReadOnly = false;
                    txtTrainerCellPhone.ReadOnly = false;
                    txtTrainerCity.ReadOnly = false;
                    txtTrainerEmail.ReadOnly = false;
                    txtTrainerHomePhone.ReadOnly = false;
                    txtTrainerNotes.ReadOnly = false;
                    txtTrainerSalary.ReadOnly = false;
                    txtTrainerStreet.ReadOnly = false;
                    txtTrainerSuburb.ReadOnly = false;
                    dtpTrainerDOB.Enabled = true;

                    btnTrainersEdit.Text = "Cancel";
                    btnTrainersEdit.Icon = null;
                    btnTrainersEdit.Tooltip = "Cancel editing";
                }
                else if (btnTrainersEdit.Text == "Cancel")
                {
                    txtTrainerFName.ReadOnly = true;
                    txtTrainerLName.ReadOnly = true;
                    txtTrainerCellPhone.ReadOnly = true;
                    txtTrainerCity.ReadOnly = true;
                    txtTrainerEmail.ReadOnly = true;
                    txtTrainerHomePhone.ReadOnly = true;
                    txtTrainerNotes.ReadOnly = true;
                    txtTrainerSalary.ReadOnly = true;
                    txtTrainerStreet.ReadOnly = true;
                    txtTrainerSuburb.ReadOnly = true;
                    dtpTrainerDOB.Enabled = false;

                    //change button text and icon
                    btnTrainersEdit.Text = "Edit";
                    ComponentResourceManager resources = new ComponentResourceManager(typeof(frmMain));
                    btnTrainersEdit.Icon = ((System.Drawing.Icon)(resources.GetObject("btnTrainersEdit.Icon")));
                }
            }
            else
            {
                MessageBox.Show("Please select a trainer first!");
                SwitchToPanel(panelTrainers);
            }
        }

        // sets controls in member manager as read only
        private void DoNotAllowMemberEdit()
        {
            txtLastName.ReadOnly = true;
            txtFirstName.ReadOnly = true;
            txtHomePhone.ReadOnly = true;
            txtStreet.ReadOnly = true;
            txtSuburb.ReadOnly = true;
            txtCity.ReadOnly = true;
            txtCellPhone.ReadOnly = true;
            txtOccupation.ReadOnly = true;
            txtNotes.ReadOnly = true;
            txtEmail.ReadOnly = true;
            txtPostalCode.ReadOnly = true;
            txtDateOfBirth.Enabled = false;
            txtCardNumber.Enabled = false;
            cbPersonalTrainer.Enabled = false;

            // change button text and icon
            btnMembersEdit.Text = "Edit";
            ComponentResourceManager resources = new ComponentResourceManager(typeof(frmMain));
            btnMembersEdit.Icon = ((System.Drawing.Icon)(resources.GetObject("btnMembersEdit.Icon")));
        }


        
        // --------------------------------- EVENTS ------------------------------------ //

        private void frmMain_Load(object sender, EventArgs e)
        {
            // get all members and bind them to the members datagridview
            BindingSource bSource = new BindingSource();
            dataset = DataLayer.Members.GetAllMembers();
            bSource.DataSource = dataset;
            membersDataGridViewX.DataSource = bSource;

            //load trainers
            LoadAllTrainerNames();

            // get all plans and bind them to listbox, in Plans panel
            listBoxPlans.DataSource = DataLayer.Plan.GetAllPlans().ToList();
            listBoxPlans.ValueMember = "Key";
            listBoxPlans.DisplayMember = "Value";

            // load check ins
            SetUpAttedance();

            // set up autocomplete members search box in ribbon
            SetUpSearch();

            // fill combobox with all plans, in ribbon find tab
            SortedDictionary<int, string> plans = new SortedDictionary<int, string>(DataLayer.Plan.GetAllPlans());           // get all the plans and put them to a sorted dictionary
            plans.Add(0, "All");                                                                                            // add 'All' entry to dictionary
            cbFindPlan.DataSource = new BindingSource(plans, null);                                                             // bind dictionary to combobox
            cbFindPlan.DisplayMember = "Value";                                                                                 // name of the plan
            cbFindPlan.ValueMember = "Key";                                                                                 // id of the plan

            // fill personal trainer combobox with all trainers, in ribbon find tab
            SortedDictionary<int, string> trainers = new SortedDictionary<int, string>(DataLayer.Trainers.GetAllTrainers());  // get id and name of all trainers and put them to a sorted dictionary
            trainers.Add(0, "All");                                                                 // add 'All' entry
            cbFindPersonalTrainer.DataSource = new BindingSource(trainers, null);                       // bind dictionary to combobox
            cbFindPersonalTrainer.DisplayMember = "Value";                                              // name of the trainer
            cbFindPersonalTrainer.ValueMember = "Key";                                                  // id of the trainer

            // fill personal trainer combobox with trainers in member manager
            trainers.Remove(0);                                                                 // remove 'All' entry
            cbPersonalTrainer.DataSource = new BindingSource(trainers, null);                       // bind dictionary to combobox
            cbPersonalTrainer.DisplayMember = "Value";                                              // name of the trainer
            cbPersonalTrainer.ValueMember = "Key";  

            // fill combobox with all plans, in new member wizard
            plans.Remove(0);                                                    //remove 'All' option
            plans.Add(0, "None");                                               // add 'None' option 
            cbWizardPlans.DataSource = new BindingSource(plans, null);
            cbWizardPlans.DisplayMember = "Value";
            cbWizardPlans.ValueMember = "Key";

            SwitchToPanel(panelAllMembers);
            ribbonTabFind.Select();                                             //switch to Find ribbon tab
        }

        private void membersDataGridViewX_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // get member's id from selected row
            member_id = int.Parse(((DataGridView)sender).Rows[e.RowIndex].Cells["Id"].Value.ToString());

            // load member
            LoadMember(member_id);
        }

        private void membersDataGridViewX_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // get member's id from selected row
            member_id = int.Parse(((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value.ToString());

            // load member
            LoadMember(member_id);

            // switch to member manager and ribbon tab Members
            SwitchToPanel(panelMemberManager);
            ribbonTabMembers.Select();
        }

        private void listBoxTrainers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (form_loaded)
            {
                if (listBoxTrainers.SelectedIndex != -1)
                {
                    //get trainer's id and load trainer's data
                    trainer_id = Convert.ToInt32(listBoxTrainers.SelectedValue.ToString());
                    LoadTrainer(trainer_id);
                }
            }
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            form_loaded = true;
        }

        private void listBoxPlans_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (form_loaded)
            {
                if (listBoxPlans.SelectedIndex != -1)
                {
                    // get selected plan
                    int planId = Convert.ToInt32(listBoxPlans.SelectedValue.ToString());
                    plan = DataLayer.Plan.GetPlan(planId);

                    //populate textboxes with plan's data
                    txtPlanName.Text = plan.Name;
                    txtPlanDuration.Text = plan.Duration.ToString();
                    txtPlanPrice.Text = plan.Price.ToString();
                    if (plan.Notes != null)
                        txtPlanNotes.Text = plan.Notes.ToString();
                    else
                        txtPlanNotes.Text = null;
                }
            }
        }

        private void btnViewAllMembers_Click(object sender, EventArgs e)
        {
            SwitchToPanel(panelAllMembers);
        }

        private void btnViewCheckins_Click(object sender, EventArgs e)
        {
            SwitchToPanel(panelAttedance);
        }

        private void btnViewTrainers_Click(object sender, EventArgs e)
        {
            SwitchToPanel(panelTrainers);
        }

        private void btnViewPlans_Click(object sender, EventArgs e)
        {
            SwitchToPanel(panelPlans);
        }

        private void btnTrainersAddMember_Click(object sender, EventArgs e)
        {
            if (form_loaded)
            {
                if (panelTrainers.Visible)
                {

                    if (listBoxTrainers.SelectedIndex != -1)
                    {
                        TrainerAssignmentDialog tad = new TrainerAssignmentDialog(trainer_id, amTrainersDataGridViewX);
                        tad.Show();
                    }
                    else
                    {
                        MessageBox.Show("Please select a trainer first!", "Gym Manager Pro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Please select a trainer first!", "Gym Manager Pro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    SwitchToPanel(panelTrainers);
                }
            }
        }

        private void btnTrainersRefresh_Click(object sender, EventArgs e)
        {
            // load again to refresh
            LoadAllTrainerNames();
        }

        private void txtFindLastName_KeyDown(object sender, KeyEventArgs e)
        {
            SwitchToPanel(panelAllMembers);

            // filter datagridview data by last name
            DataView dv = new DataView(dataset);
            dv.RowFilter = string.Format("LastName LIKE '%{0}%'", txtFindLastName.Text);
            membersDataGridViewX.DataSource = dv;
        }

        private void txtFindFirstName_KeyDown(object sender, KeyEventArgs e)
        {
            SwitchToPanel(panelAllMembers);

            // filter datagridview data by first name
            DataView dv = new DataView(dataset);
            dv.RowFilter = string.Format("FirstName LIKE '%{0}%'", txtFindFirstName.Text);
            membersDataGridViewX.DataSource = dv;
        }

        private void cbFindPlan_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (form_loaded)
            {
                SwitchToPanel(panelAllMembers);

                // retrieve the members who have the selected plan and bind them to datagridview
                int plan_id = Int32.Parse(cbFindPlan.SelectedValue.ToString());                 // get id of the selected plan

                if (plan_id != 0)                   // if the selected plan is not 'All'
                {
                    BindingSource bSource = new BindingSource();
                    dataset = DataLayer.Members.GetMembersByPlan(plan_id);
                    bSource.DataSource = dataset;
                    membersDataGridViewX.DataSource = bSource;
                }
                else     // if the selected plan is 'ALL'
                {
                    // get all members and bind them to the datagridview
                    BindingSource bSource = new BindingSource();
                    dataset = DataLayer.Members.GetAllMembers();
                    bSource.DataSource = dataset;
                    membersDataGridViewX.DataSource = bSource;
                }

                // set personal trainer filter combobox to default value
                cbFindPersonalTrainer.SelectedIndex = 0;
            }
        }

        private void cbFindPersonalTrainer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (form_loaded)
            {
                SwitchToPanel(panelAllMembers);

                // retrieve the members who are assigned to the the selected trainer and bind them to datagridview
                int trainer_id = Int32.Parse(cbFindPersonalTrainer.SelectedValue.ToString());                         // get id of the selected trainer

                if (trainer_id != 0)                                                           // if the selected trainer is not set to 'All'
                {
                    BindingSource bSource = new BindingSource();
                    dataset = DataLayer.Members.GetMembersByPersonalTrainer(trainer_id);
                    bSource.DataSource = dataset;
                    membersDataGridViewX.DataSource = bSource;
                }
                else
                {
                    // get all members and bind them to the datagridview
                    BindingSource bSource = new BindingSource();
                    dataset = DataLayer.Members.GetAllMembers();
                    bSource.DataSource = dataset;
                    membersDataGridViewX.DataSource = bSource;
                }

                // set plan filter combobox to default value
                cbFindPlan.SelectedIndex = 0;
            }
        }

        private void btnFindRefresh_Click(object sender, EventArgs e)
        {
            RefreshAllMembersDataGrid();
            txtFindFirstName.Text = null;
            txtFindLastName.Text = null;
        }

        private void btnMembersNext_Click(object sender, EventArgs e)
        {
            SwitchToPanel(panelMemberManager);

            // Retrieves and displays the next member (if any)
            if (DataLayer.Members.MemberHasNext(member_id))
            {
                member_id = DataLayer.Members.GetNextMember(member_id);
                LoadMember(member_id);
            }
            else
            {
                MessageBox.Show("There are no more members!", "Gym Manager Pro", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnMembersPrev_Click(object sender, EventArgs e)
        {
            SwitchToPanel(panelMemberManager);

            if (DataLayer.Members.MemberHasPrevious(member_id))
            {
                member_id = DataLayer.Members.GetPrevMember(member_id);
                LoadMember(member_id);
            }
            else
            {
                MessageBox.Show("There are no more members!", "Gym Manager Pro", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnMembersSearch_Click(object sender, EventArgs e)
        {
            // loads specified member
            LoadMember(DataLayer.Members.QuickSearch(txtMembersSearch.Text));
            SwitchToPanel(panelMemberManager);
        }

        private void btnCheckIn_Click(object sender, EventArgs e)
        {
            if (panelAllMembers.Visible || panelMemberManager.Visible)
            {
                if (member_id != 0)             // if a member has been selected
                {
                    if (DataLayer.Members.MemberCheckin(member_id) > 0)
                    {
                        MessageBox.Show(lblName.Text + " just Checked-in!", "Gym Manager Pro", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Console.Beep();
                        //refresh
                        SetUpAttedance();
                    }
                    else
                    {
                        MessageBox.Show("Couldnot check-in. Please try again", "Gym Manager Pro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Please select a user first!", "Gym Manager Pro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please select a member first!");
                SwitchToPanel(panelAllMembers);
            }
        }

        private void btnAttedanceRefresh_Click(object sender, EventArgs e)
        {
            // get check ins again to refresh
            SetUpAttedance();
        }

        private void btnMembersDelete_Click(object sender, EventArgs e)
        {
            if (panelMemberManager.Visible)
            {
                // Deletes the current member
                DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this member? All related data will be lost!!!", "Gym Manager Pro", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                {
                    DataLayer.Members.DeleteMember(member_id);
                    MessageBox.Show("Member deleted!", "Gym Manager Pro", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // display the previous member
                    member_id = DataLayer.Members.GetPrevMember(member_id);
                    LoadMember(member_id);
                }
            }
            else if (panelAllMembers.Visible)
            {
                if (membersDataGridViewX.SelectedRows.Count > 0)
                {
                    //if a row is selected
                    //display confirmation dialog
                    DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this member? All related data will be lost!!!", "Gym Manager Pro", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                    {
                        // get member's id from selected row
                        member_id = int.Parse(membersDataGridViewX.SelectedRows[0].Cells[0].Value.ToString());

                        // delete selected member
                        if (DataLayer.Members.DeleteMember(member_id) > 0)
                        {
                            MessageBox.Show("Member deleted!", "Gym Manager Pro", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            RefreshAllMembersDataGrid();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a member first!");
                SwitchToPanel(panelAllMembers);
            }
        }

        private void btnMembersSave_Click(object sender, EventArgs e)
        {
            // create a new member and save its details to the db
            DataLayer.Members members = new DataLayer.Members();
            DataLayer.Member member = new DataLayer.Member();

            if (!String.IsNullOrEmpty(txtLastName.Text))
            {
                // fill the properties of member object based on textboxes text
                member.MemberID = this.member_id;
                member.CardNumber = Int32.Parse(txtCardNumber.Text);
                member.LName = txtLastName.Text;
                member.FName = txtFirstName.Text;
                if (rbMale.Checked)
                {
                    member.Sex = "male";
                }
                else if (rbFemale.Checked)
                {
                    member.Sex = "female";
                }
                member.DateOfBirth = txtDateOfBirth.Value;
                member.Street = txtStreet.Text;
                member.Suburb = txtSuburb.Text;
                member.City = txtCity.Text;
                if (txtPostalCode.Text.Length > 0)
                {
                    member.PostalCode = Int32.Parse(txtPostalCode.Text);
                }
                else
                {
                    member.PostalCode = 0;
                }
                member.HomePhone = txtHomePhone.Text;
                member.CellPhone = txtCellPhone.Text;
                member.Email = txtEmail.Text;
                member.Occupation = txtOccupation.Text;
                member.Notes = txtNotes.Text;
                member.PersonalTrainer = int.Parse( cbPersonalTrainer.SelectedValue.ToString());

                // holds the member's picture
                //byte[] imageBt = null;
                if (pictureBoxMemberManager.ImageLocation != null)
                {
                    FileStream fstream = new FileStream(pictureBoxMemberManager.ImageLocation, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fstream);
                    member.Image = br.ReadBytes((int)fstream.Length);
                }
                else
                {
                    byte[] empty_array = new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };
                    member.Image = empty_array;
                }


                if (DataLayer.Members.UpdateMember(member) > 0)
                {
                    MessageBox.Show("Member Updated successfully!", "Gym Manager Pro", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to Update Member!", "Gym Manager Pro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // set textboxes to readonly
                DoNotAllowMemberEdit();
            }
            else
            {
                MessageBox.Show("Last Name cannot be empty!", "Gym Manager Pro", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnMembersEdit_Click(object sender, EventArgs e)
        {
            if (panelAllMembers.Visible)
                SwitchToPanel(panelMemberManager);

            if (panelMemberManager.Visible)
            {
                if (btnMembersEdit.Text == "Edit")
                {
                    txtLastName.ReadOnly = false;
                    txtFirstName.ReadOnly = false;
                    txtHomePhone.ReadOnly = false;
                    txtStreet.ReadOnly = false;
                    txtSuburb.ReadOnly = false;
                    txtCity.ReadOnly = false;
                    txtCellPhone.ReadOnly = false;
                    txtOccupation.ReadOnly = false;
                    txtEmail.ReadOnly = false;
                    txtNotes.ReadOnly = false;
                    txtPostalCode.ReadOnly = false;
                    txtDateOfBirth.Enabled = true;
                    txtCardNumber.Enabled = true;
                    cbPersonalTrainer.Enabled = true;

                    btnMembersEdit.Text = "Cancel";
                    btnMembersEdit.Icon = null;
                    btnMembersEdit.Tooltip = "Cancel editing";
                }
                else if (btnMembersEdit.Text == "Cancel")
                {
                    DoNotAllowMemberEdit();
                }
            }
            else
            {
                MessageBox.Show("Pleaase select a member first!");
                SwitchToPanel(panelAllMembers);
            }
        }

        private void buttonXNewMembership_Click(object sender, EventArgs e)
        {
            AddNewMembership newContract = new AddNewMembership(member_id, dataGridViewMemberships);
            newContract.Show();
        }

        private void buttonXDeleteMembership_Click(object sender, EventArgs e)
        {
            // deletes/expires the selected membership
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this membership? The selected membership will expire and the operation cannot be undone!!!", "Gym Manager Pro", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                //get id of the selected membership
                int delid = (int)dataGridViewMemberships.SelectedRows[0].Cells["Membership Id"].Value;

                // delete the membership
                DataLayer.Memberships.DeleteMembership(delid);
                MessageBox.Show("Membership removed!", "Gym Manager Pro", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // reload data
                LoadMembership(member_id);

            }
        }

        private void btnAttedanceCheckin_Click(object sender, EventArgs e)
        {
            if (txtAttedanceSearch.Text.Length > 0)
            {
                if ( DataLayer.Members.CheckIfIdExists(Int32.Parse(txtAttedanceSearch.Text)) >0 )
                {
                    if (DataLayer.Members.MemberCheckin(Int32.Parse(txtAttedanceSearch.Text)) > 0)
                    {
                        MessageBox.Show("Member just Checked-In!", "Gym Manager Pro", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to check-in. Please try again", "Gym Manager Pro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("This user does not exist.");
                }
            }
            else
            {
                MessageBox.Show("Please enter a member's card number first!", "Gym Manager Pro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnMembersNew_Click(object sender, EventArgs e)
        {
            SwitchToPanel(panelNewMemberWizard);
        }

        private void cbWizardPlans_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (form_loaded)
            {
                int plan_id = Int32.Parse(cbWizardPlans.SelectedValue.ToString());                                           // get id of the selected plan

                txtWizardMembershipFee.Text = DataLayer.Plan.GetPlanPrice(plan_id).ToString();                                    // get membership's cost
                dtpWizardEndPlan.Value = dtpWizardStartPlan.Value;                                                                // get membership's start date
                dtpWizardEndPlan.Value = dtpWizardEndPlan.Value.AddMonths(DataLayer.Plan.GetPlanDuration(plan_id));               // calculate membership duration
                dtpWizardEndPlan.Value = dtpWizardEndPlan.Value.AddDays(-1);                                                      // subtract one day
            }
        }

        private void txtWizardInitiationFee_TextChanged(object sender, EventArgs e)
        {
            int plan_id = Int32.Parse(cbWizardPlans.SelectedValue.ToString());                                           // get id of the selected plan
            decimal programmefee = DataLayer.Plan.GetPlanPrice(plan_id);                                                // get selected plan's price
            try
            {
                if (txtWizardInitiationFee.Text.Trim().Length == 0)                                                           // if initationfee textbox is empty
                    txtWizardInitiationFee.Text = "0";
                else
                {
                    decimal totalfee = (decimal)Decimal.Parse(txtWizardInitiationFee.Text.ToString()) + programmefee;         // calculate the total fee by adding the initation fee to the plan's fee
                    txtWizardTotalFees.Text = totalfee.ToString();                                                            // display total fee
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void wizard1_FinishButtonClick(object sender, CancelEventArgs e)
        {
            //////////////// CREATE A NEW MEMBER //////////////

            // create a new member
            DataLayer.Member member = new DataLayer.Member();

            // fill member properties from textboxes
            member.CardNumber = Int32.Parse(txtWizardCardNumber.Text);
            member.LName = txtWizardLastName.Text;
            member.FName = txtWizardFirstName.Text;
            if (rbWizardMale.Checked)
            {
                member.Sex = "male";
            }
            else if (rbWizardFemale.Checked)
            {
                member.Sex = "female";
            }
            member.DateOfBirth = dtpWizardDOB.Value;
            member.Street = txtWizardStreet.Text;
            member.Suburb = txtWizardSuburb.Text;
            member.City = txtWizardCity.Text;
            if (txtPostalCode.Text.Length > 0)
            {
                member.PostalCode = Int32.Parse(txtWizardPostalCode.Text);
            }
            member.HomePhone = txtWizardHomePhone.Text;
            member.CellPhone = txtWizardCellPhone.Text;
            member.Email = txtWizardEmail.Text;
            member.Occupation = txtWizardOccupation.Text;
            member.Notes = txtWizardNotes.Text;

            // holds the member's picture
            //byte[] imageBt = null;
            if (picWizard.ImageLocation != null)
            {
                FileStream fstream = new FileStream(picWizard.ImageLocation, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fstream);
                member.Image = br.ReadBytes((int)fstream.Length);
            }
            else
            {
                byte[] empty_array = new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };
                member.Image = empty_array;
            }


            //add member to db
            if (DataLayer.Members.AddNewMember(member) > 0)
            {
                //a new member has been added without any membership
                //MessageBox.Show("A New Member has been added. You can add a membership at any time from Member Manager", "Gym Manager Pro", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Failed to add new member. Please try again", "Gym Manager Pro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }



            ////////////// CREATE A NEW MEMBERSHIP FOR THE SAME MEMBER //////////////

            if (cbWizardPlans.Text != "None")
            {
                // create a new membership and fill with data
                DataLayer.Membership membership = new DataLayer.Membership();

                //to find member's id we get the last inserted id and increment by 1 because we haven't inserted the new member yet
                membership.MemberId = DataLayer.Members.GetLastInsertedMember();        // member's id
                //membership.MemberId++;
                membership.Plan = (int)cbWizardPlans.SelectedValue;                 // id of the selected plan
                membership.start = dtpWizardStartPlan.Value;                           // when the membership starts
                membership.end = dtpWizardEndPlan.Value;                               // when the membership expires

                if (DataLayer.Memberships.NewMembership(membership) > 0)
                {
                    MessageBox.Show("A New Member has been added successfully!", "Gym Manager Pro", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to add new membership. Please add manually from Member Manager", "Gym Manager Pro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }


            RefreshAllMembersDataGrid();            // refresh AllMembers datagridview
            panelNewMemberWizard.Visible = false;
            panelAllMembers.Visible = true;         // show all members datagrid view

        }

        private void dtpWizardStartPlan_ValueChanged(object sender, EventArgs e)
        {
            // set the expiration date of the membership based on the start date
            dtpWizardEndPlan.Value = dtpWizardStartPlan.Value;                                                            // get membership's start date
            int plan_id = Int32.Parse(cbWizardPlans.SelectedValue.ToString());                                      // get id of the selected plan
            dtpWizardEndPlan.Value = dtpWizardEndPlan.Value.AddMonths(DataLayer.Plan.GetPlanDuration(plan_id));           // calculate membership duration
            dtpWizardEndPlan.Value = dtpWizardEndPlan.Value.AddDays(-1);
        }

        private void wizardPage2_NextButtonClick(object sender, CancelEventArgs e)
        {
            if (String.IsNullOrEmpty(txtWizardLastName.Text) && String.IsNullOrEmpty(txtWizardCardNumber.Text))
            {
                MessageBox.Show("The Last Name and Card Number cannot be empty!", "Gym Manager Pro", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                wizard1.NavigateBack();
            }
        }

        private void wizardPage2_CancelButtonClick(object sender, CancelEventArgs e)
        {
            panelNewMemberWizard.Visible = false;
            wizard1.NavigateBack();
        }

        private void wizard1_CancelButtonClick(object sender, CancelEventArgs e)
        {
            panelNewMemberWizard.Visible = false;
        }

        private void wizardPage3_CancelButtonClick(object sender, CancelEventArgs e)
        {
            panelNewMemberWizard.Visible = false;
            wizard1.NavigateBack();
        }

        private void buttonItem13_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonItem1_Click(object sender, EventArgs e)
        {
            panelAllMembers.Visible = false;
            panelMemberManager.Visible = false;
            panelTrainers.Visible = false;
            panelPlans.Visible = false;
            panelAttedance.Visible = false;
            panelNewMemberWizard.Visible = true;
        }

        private void btnAttedanceSaveTxt_Click(object sender, EventArgs e)
        {
            SwitchToPanel(panelAttedance);

            // save file as a txt document
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Text Files | *.txt";
            sfd.DefaultExt = "txt";
            sfd.Title = "Save as text file";
            if (sfd.ShowDialog() == DialogResult.OK)
                System.IO.File.WriteAllText(sfd.FileName, richTextBoxAttedance.Text);
        }

        private void dataGridViewMemberships_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            //set up membership expire notifications in member manager
            SetUpNotifications();
        }

        private void btnPlansNew_Click(object sender, EventArgs e)
        {
            EditPlan ep = new EditPlan(listBoxPlans);
            ep.ShowDialog();
            SwitchToPanel(panelPlans);
        }

        private void btnPlansEdit_Click(object sender, EventArgs e)
        {
            if (form_loaded)
            {
                if (panelPlans.Visible)
                {
                    if (listBoxPlans.SelectedIndex != -1)
                    {
                        // get selected plan
                        int planId = Convert.ToInt32(listBoxPlans.SelectedValue.ToString());
                        EditPlan ep = new EditPlan(plan, listBoxPlans);
                        ep.ShowDialog();
                    }
                }
                else
                {
                    MessageBox.Show("Please select a plan first");
                    SwitchToPanel(panelPlans);
                }
            }
        }

        private void btnPlansDelete_Click(object sender, EventArgs e)
        {
            if (panelPlans.Visible)
            {
                DialogResult dialogResult = MessageBox.Show("Warning! Deleting the selected plan will also expire (delete) all the associated memberships! Are you sure you want to continue?", "Gym Manager Pro", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (dialogResult == DialogResult.Yes)
                {
                    DataLayer.Plan.DeletePlan(plan.Id);

                    // refresh the listbox
                    listBoxPlans.DataSource = DataLayer.Plan.GetAllPlans().ToList();
                    listBoxPlans.ValueMember = "Key";
                    listBoxPlans.DisplayMember = "Value";
                }
            }
            else
            {
                MessageBox.Show("Please select a plan first!");
                SwitchToPanel(panelPlans);
            }
        }

        private void btnTrainersRemoveMember_Click(object sender, EventArgs e)
        {
            if (panelTrainers.Visible)
            {
                if (amTrainersDataGridViewX.SelectedRows.Count != 0)
                {
                    // get the id of the member to be removed
                    int memberToRemove = (int)amTrainersDataGridViewX.SelectedCells[0].Value; //first cell of the selected row

                    if (DataLayer.Trainers.RemoveMember(memberToRemove) > 0)
                    {
                        MessageBox.Show("Member removed!", "Gym Manager Pro", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // reload associated members
                        DataTable membersTable = DataLayer.Trainers.GetAssociatedMembers(trainer_id);
                        amTrainersDataGridViewX.DataSource = membersTable;
                    }
                    else
                    {
                        MessageBox.Show("Failed to remove member. Please try again", "Gym Manager Pro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please Select a Trainer first!");
                SwitchToPanel(panelTrainers);
            }
        }

        private void txtWizardLastName_TextChanged(object sender, EventArgs e)
        {
            lblWizardName.Text = txtWizardFirstName.Text +" "+ txtWizardLastName.Text;
        }

        private void txtWizardFirstName_TextChanged(object sender, EventArgs e)
        {
            lblWizardName.Text = txtWizardFirstName.Text + " " + txtWizardLastName.Text;
        }

        private void btnTrainersEdit_Click(object sender, EventArgs e)
        {
            EditTrainer();
        }

        private void btnTrainersSave_Click(object sender, EventArgs e)
        {
            if (panelTrainers.Visible)
            {
                if (listBoxTrainers.SelectedIndex != 0)
                {
                    // create a new trainer object
                    DataLayer.Trainer trainer = new DataLayer.Trainer();

                    if (!String.IsNullOrEmpty(txtTrainerLName.Text))
                    {
                        trainer.TrainerID = this.trainer_id;
                        trainer.FName = txtTrainerFName.Text.Trim();
                        trainer.LName = txtTrainerLName.Text.Trim();
                        trainer.CellPhone = txtTrainerCellPhone.Text.Trim();
                        trainer.City = txtTrainerCity.Text.Trim();
                        trainer.DateOfBirth = dtpTrainerDOB.Value;
                        trainer.Email = txtTrainerEmail.Text.Trim();
                        trainer.HomePhone = txtTrainerHomePhone.Text.Trim();
                        if (txtTrainerSalary.Text.Length > 0)
                        {
                            trainer.Salary = Decimal.Parse(txtTrainerSalary.Text.ToString());
                        }
                        else
                        {
                            trainer.Salary = 0;
                        }
                        trainer.Street = txtTrainerStreet.Text.Trim();
                        trainer.Suburb = txtTrainerSuburb.Text.Trim();
                        if (rbTrainerMale.Checked)
                        {
                            trainer.Sex = "Male";
                        }
                        else if (rbTrainerFemale.Checked)
                        {
                            trainer.Sex = "Female";
                        }
                        trainer.Notes = txtTrainerNotes.Text.Trim();

                        //save data to db
                        if (DataLayer.Trainers.UpdateTrainer(trainer) > 0)
                        {
                            MessageBox.Show("Trainer Updated successfully!", "Gym Manager Pro", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // refresh Trainers listbox
                            LoadAllTrainerNames();
                        }
                        else
                        {
                            MessageBox.Show("Failed to Update Trainer!", "Gym Manager Pro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Last Name cannot be empty!", "Gym Manager Pro", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                    // set textboxes to read only
                    txtTrainerFName.ReadOnly = true;
                    txtTrainerLName.ReadOnly = true;
                    txtTrainerCellPhone.ReadOnly = true;
                    txtTrainerCity.ReadOnly = true;
                    txtTrainerEmail.ReadOnly = true;
                    txtTrainerHomePhone.ReadOnly = true;
                    txtTrainerNotes.ReadOnly = true;
                    txtTrainerSalary.ReadOnly = true;
                    txtTrainerStreet.ReadOnly = true;
                    txtTrainerSuburb.ReadOnly = true;

                    //change button text and icon of btnTrainersEdit
                    btnTrainersEdit.Text = "Edit";
                    ComponentResourceManager resources = new ComponentResourceManager(typeof(frmMain));
                    btnTrainersEdit.Icon = ((System.Drawing.Icon)(resources.GetObject("btnTrainersEdit.Icon")));
                }
            }
        }

        private void btnTrainersDelete_Click(object sender, EventArgs e)
        {
            if (panelTrainers.Visible)
            {
                if (listBoxTrainers.SelectedIndex != 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this Trainer? This cannot be undone!", "Gym Manager Pro", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                    {
                        if (DataLayer.Trainers.DeleteTrainer(this.trainer_id) > 0)
                        {
                            MessageBox.Show("Trainer deleted!", "Gym Manager Pro", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // refresh trainers listbox
                            LoadAllTrainerNames();
                        }
                        else
                        {
                            MessageBox.Show("Could not delete. Please try again.", "Gym Manager Pro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please select a trainer first!");
                }
            }
            else
            {
                MessageBox.Show("Please select a trainer first!");
                SwitchToPanel(panelTrainers);
            }
        }

        private void btnTrainersAdd_Click(object sender, EventArgs e)
        {
            // switch to trainers panel
            if (!panelTrainers.Visible)
                SwitchToPanel(panelTrainers);

            // create a new trainer
            DataLayer.Trainer trainer = new DataLayer.Trainer();
            trainer.FName = "New Trainer";
            trainer.LName = "New Trainer";
            trainer.Sex = "Male";
            trainer.DateOfBirth = DateTime.Now;
            trainer.Street = String.Empty;
            trainer.Suburb = String.Empty;
            trainer.Salary = 0;
            trainer.HomePhone = String.Empty;
            trainer.CellPhone = String.Empty;
            trainer.Notes = String.Empty;
            trainer.City = String.Empty;
            trainer.Email = String.Empty;

            // add to db
            if (DataLayer.Trainers.NewTrainer(trainer) > 0)
            {
                //MessageBox.Show("success");
                trainer_id = DataLayer.Trainers.GetLastInsertedTrainer();
                LoadAllTrainerNames();
                this.listBoxTrainers.SelectedIndex = this.listBoxTrainers.Items.Count - 1;
            }
            else
            {
                MessageBox.Show("could not add");
            }

            // set textboxes editable
            EditTrainer();
            
        }

        private void menuAbout_Click(object sender, EventArgs e)
        {
            new AboutBox().Show();
        }

        private void btnFindSearch_Click(object sender, EventArgs e)
        {
            if (!panelAllMembers.Visible)
                SwitchToPanel(panelAllMembers);

            if (txtFindSearch.Text != "")
            {
                BindingSource bSource = new BindingSource();
                dataset = DataLayer.Members.AdvancedSearch(cbFindSearchBy.SelectedItem.ToString(), txtFindSearch.Text);
                bSource.DataSource = dataset;
                membersDataGridViewX.DataSource = bSource;
            }
            else
            {
                RefreshAllMembersDataGrid();
            }
        }

        private void menuNewMember_Click(object sender, EventArgs e)
        {
            SwitchToPanel(panelNewMemberWizard);
        }

        private void pictureBoxMemberManager_Click(object sender, EventArgs e)
        {
            //loads an image for the member
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "JPG Files(*.jpg)|*.jpg|PNG Files(*.png)|*.png|All Files(*.*)|*.*";
            if (fd.ShowDialog() == DialogResult.OK)
            {
                string picLoc = fd.FileName.ToString();
                pictureBoxMemberManager.ImageLocation = picLoc;
            }
        }

        private void btnReportsOverview_Click(object sender, EventArgs e)
        {
            SwitchToPanel(panelReports);
        }

        private void btnFindExport_Click(object sender, EventArgs e)
        {
            
            saveFileDialog1.InitialDirectory = "C:";
            saveFileDialog1.Title = "Save as Excel File";
            saveFileDialog1.FileName = "data";
            saveFileDialog1.Filter = "Excel Files(2003)|*.xls";
            if (saveFileDialog1.ShowDialog() != DialogResult.Cancel)
            {
                Utility.ToCsV(membersDataGridViewX, saveFileDialog1.FileName);
                MessageBox.Show("Data exported successfully.");
            }
        }
      
    }

}


    static class Utility
    {
        // highlights specified text
        // http://stackoverflow.com/questions/11851908/highlight-all-searched-word-in-richtextbox-c-sharp
        //
        public static void HighlightText(this RichTextBox myRtb, string word, Color color)
        {
            int s_start = myRtb.SelectionStart, startIndex = 0, index;

            while ((index = myRtb.Text.IndexOf(word, startIndex)) != -1)
            {
                myRtb.Select(index, word.Length);
                myRtb.SelectionColor = color;

                startIndex = index + word.Length;
            }
            myRtb.SelectionStart = s_start;
            myRtb.SelectionLength = 0;
            myRtb.SelectionColor = Color.Black;
        }

        /// <summary>
        /// exports data to xls file 
        /// (http://www.codeproject.com/Tips/545456/Exporting-DataGridview-To-Excel)
        /// </summary>
        /// <param name="dGV">datagridview</param>
        /// <param name="filename">filename</param>
        public static void ToCsV(DataGridView dGV, string filename)
        {
            string stOutput = "";
            // Export titles:
            string sHeaders = "";

            for (int j = 0; j < dGV.Columns.Count; j++)
                sHeaders = sHeaders.ToString() + Convert.ToString(dGV.Columns[j].HeaderText) + "\t";
            stOutput += sHeaders + "\r\n";
            // Export data.
            for (int i = 0; i < dGV.RowCount - 1; i++)
            {
                string stLine = "";
                for (int j = 0; j < dGV.Rows[i].Cells.Count; j++)
                    stLine = stLine.ToString() + Convert.ToString(dGV.Rows[i].Cells[j].Value) + "\t";
                stOutput += stLine + "\r\n";
            }
            Encoding utf16 = Encoding.GetEncoding(1254);
            byte[] output = utf16.GetBytes(stOutput);
            FileStream fs = new FileStream(filename, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(output, 0, output.Length); //write the encoded file
            bw.Flush();
            bw.Close();
            fs.Close();
        }
}

