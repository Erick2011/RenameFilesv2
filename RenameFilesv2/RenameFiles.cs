namespace RenameFilesv2
{
    using RenameFilesv2.Models;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;

    public partial class RenameFiles : Form
    {
        #region Attributes
        List<FileInf> originalList = new List<FileInf>();
        List<FileInf> checkedItems = new List<FileInf>();
        List<RecoveryList> recoveryList = new List<RecoveryList>();
        #endregion

        #region Constructor
        public RenameFiles()
        {
            InitializeComponent();
        }
        #endregion

        #region Methods
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
            btnSelectAll.Text = "Seleccionar Todo";
            btnSelectAll.Enabled = false;
        }

        private void clOriginalList_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effect = DragDropEffects.All;
            }
        }

        private void clOriginalList_DragDrop(object sender, DragEventArgs e)
        {
            string[] droppedFiles = ((string[])e.Data.GetData(DataFormats.FileDrop));

            foreach (var file in droppedFiles)
            {
                btnSelectAll.Enabled = true;

                if (!isFolder(file))
                {
                    if (!isListed(file))
                    {
                        FileInf fileInfo = new FileInf();
                        fileInfo.fullPath = file;
                        fileInfo.fileName = Path.GetFileName(file);
                        fileInfo.extension = Path.GetExtension(file);
                        originalList.Add(fileInfo);
                        clOriginalList.Items.Add(fileInfo.fileName);
                    }
                }
                if (isFolder(file))
                {
                    FillOriginalListByDrag(file);
                }
            }
        }

        private void FillOriginalListByDrag(string folderPath)
        {
            if (txtPathFolder.Text.Equals(string.Empty))
            {
                txtPathFolder.Text = folderPath;
            }

            DirectoryInfo dir = new DirectoryInfo(folderPath);

            if (dir.GetFiles().Length > 0)
            {
                lblDragMessage.Visible = false;
            }

            foreach (var file in dir.GetFiles())
            {
                if (!isListed(file.FullName))
                {
                    FileInf fileInfo = new FileInf();
                    fileInfo.fileName = file.Name;
                    fileInfo.fullPath = file.FullName;
                    fileInfo.extension = file.Extension;
                    originalList.Add(fileInfo);
                    clOriginalList.Items.Add(fileInfo.fileName);
                }
            }
        }

        private bool isListed(string filename)
        {
            foreach (FileInf item in originalList)
            {
                if (filename.Equals(item.fullPath))
                {
                    return true;
                }
            }
            return false;
        }

        private bool isFolder(string fullPath)
        {
            FileAttributes attr = File.GetAttributes(fullPath);
            bool isFolder = (attr & FileAttributes.Directory) == FileAttributes.Directory;
            return isFolder;
        }

        private void btnCopyPath_Click(object sender, EventArgs e)
        {
            ClearControls();
            txtPathFolder.Text = Clipboard.GetText();

            if (!Directory.Exists(txtPathFolder.Text))
            {
                MessageBox.Show(
                    "Ruta de carpeta no valida",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1);
                ClearControls();
                return;
            }

            lblDragMessage.Visible = false;
            FillOriginalList();
        }

        private void clOriginalList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            string fileName = clOriginalList.Items[e.Index].ToString();

            if (e.NewValue == CheckState.Checked)
            {
                FileInf fileInfo = FindFileByName(fileName);
                checkedItems.Add(fileInfo);
                lbRenamedList.Items.Add(fileInfo.fileName);
            }

            if (e.NewValue == CheckState.Unchecked)
            {
                checkedItems.RemoveAll(x => x.fileName == fileName);
                lbRenamedList.Items.Remove(fileName);
            }
            if (isAllChecked())
            {
                btnSelectAll.Text = "Deseleccionar Todo";
            }
            else
            {
                btnSelectAll.Text = "Seleccionar Todo";
            }               
        }

        private FileInf FindFileByName(string fileName)
        {         
           var result = from fileInfo in originalList
                        where fileInfo.fileName == fileName
                        select fileInfo;

            FileInf fileInf = new FileInf();
            fileInf.fileName = result.First().fileName;
            fileInf.fullPath = result.First().fullPath;
            fileInf.extension = result.First().extension;

            return fileInf;
        }

        private bool isAllChecked()
        {
            if(checkedItems.Count.Equals(clOriginalList.Items.Count)) 
            {
                return true;
            }
            return false;
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            if (btnSelectAll.Text.Equals("Seleccionar Todo"))
            {
                for (int i = 0; i < clOriginalList.Items.Count; i++)
                {
                    clOriginalList.SetItemChecked(i, true);
                }
                btnSelectAll.Text = "Deseleccionar Todo";
            }
            else
            {
                for (int i = 0; i < clOriginalList.Items.Count; i++)
                {
                    clOriginalList.SetItemChecked(i, false);
                }
                btnSelectAll.Text = "Seleccionar Todo";
            }
        }

        private void chkBegginingFrom_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBegginingFrom.Checked)
            {
                txtBeginningFrom.Text = string.Empty;
                txtBeginningFrom.Enabled = true;
            }
            else
            {
                txtBeginningFrom.Text = string.Empty;
                txtBeginningFrom.Enabled = false;
            }
        }

        private void btnRename_Click(object sender, EventArgs e)
        {
            if (txtSerieName.Text.Equals(""))
            {
                MessageBox.Show(
                    "Debe ingresar el nombre de la Serie/Película",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1);
                return;
            }
            if (checkedItems.Count == 0)
            {
                MessageBox.Show(
                    "Debe ingresar al menos un archivo a renombrar",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1);
                return;
            }
            if (chkBegginingFrom.Checked && txtBeginningFrom.Text.Equals(string.Empty))
            {
                MessageBox.Show(
                    "Debe ingresar el número correlativo",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1);
                txtBeginningFrom.Focus();
                return;
            }

            foreach (var file in lbRenamedList.Items)
            {

            }
        }

        #region Drag and Drop ListBox
        private void lbRenamedList_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.lbRenamedList.SelectedItem == null) return;
            this.lbRenamedList.DoDragDrop(this.lbRenamedList.SelectedItem, DragDropEffects.Move);
        }

        private void lbRenamedList_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void lbRenamedList_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                Point point = lbRenamedList.PointToClient(new Point(e.X, e.Y));
                int index = this.lbRenamedList.IndexFromPoint(point);
                if (index < 0) index = this.lbRenamedList.Items.Count - 1;
                object data = e.Data.GetData(typeof(string));
                this.lbRenamedList.Items.Remove(data);
                this.lbRenamedList.Items.Insert(index, data);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Acción no permitida",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1);
            }
        }

        #endregion

        #endregion       
    }
}
