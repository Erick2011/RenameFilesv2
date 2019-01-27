namespace RenameFilesv2
{
    using RenameFilesv2.Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Forms;

    public partial class RenameFiles : Form
    {
        List<FileInf> originalList = new List<FileInf>();
        List<FileInf> checkedItems = new List<FileInf>();
        List<RecoveryList> recoveryList = new List<RecoveryList>();

        public RenameFiles()
        {
            InitializeComponent();
        }

        private void btnSelectPath_Click(object sender, EventArgs e)
        {
            ClearControls();
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Seleccione la Carpeta";
            fbd.RootFolder = Environment.SpecialFolder.Desktop;

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                txtPathFolder.Text = fbd.SelectedPath;
                lblDragMessage.Visible = false;
                FillOriginalList();
            }
        }

        private void FillOriginalList()
        {
            DirectoryInfo dir = new DirectoryInfo(txtPathFolder.Text);
            if (dir.GetFiles().Length == 0)
            {
                btnSelectAll.Enabled = false;
                return;
            }

            btnSelectAll.Enabled = true;

            foreach (var file in dir.GetFiles())
            {
                clOriginalList.Items.Add(file.Name);

                FileInf fileInf = new FileInf();
                fileInf.fileName = file.Name;
                fileInf.fullPath = file.FullName;
                fileInf.extension = file.Extension;
                originalList.Add(fileInf);
            }
        }

        private void ClearControls()
        {
            txtPathFolder.Text = string.Empty;
            txtSeason.Text = string.Empty;
            txtSerieName.Text = string.Empty;
            clOriginalList.Items.Clear();
            lbRenamedList.Items.Clear();
            checkedItems.Clear();
            originalList.Clear();
            lblDragMessage.Visible = true;
            btnUndoChanges.Enabled = false;
        }
    }
}
