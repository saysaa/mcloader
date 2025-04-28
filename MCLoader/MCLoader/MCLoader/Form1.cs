using System;
using System.IO;
using System.Windows.Forms;

namespace MCLoader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            AllowDrop = true;
            DragEnter += Form1_DragEnter;
            DragDrop += Form1_DragDrop;
            Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop)
                ? DragDropEffects.Copy
                : DragDropEffects.None;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data == null) return;

            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files == null || files.Length == 0) return;

            string sourcePath = files[0];

            try
            {
                if (File.Exists(sourcePath))
                {
                    HandleFileDrop(sourcePath);
                }
                else if (Directory.Exists(sourcePath))
                {
                    HandleDirectoryDrop(sourcePath);
                }
                else
                {
                    ShowError("Chemin non valide.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Erreur lors de l'importation : {ex.Message}");
            }
        }

        private void HandleFileDrop(string sourcePath)
        {
            string extension = Path.GetExtension(sourcePath).ToLower();
            string targetPath;

            if (extension == ".schem")
            {
                targetPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    @".minecraft\config\worldedit\schematics"
                );
                CopyFileToTarget(sourcePath, targetPath, "Fichier .schem importé avec succès !");
            }
            else if (extension == ".jar")
            {
                targetPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    @".minecraft\mods"
                );
                CopyFileToTarget(sourcePath, targetPath, "Fichier .jar importé avec succès !");
            }
            else
            {
                ShowWarning("Type de fichier non pris en charge.");
            }
        }

        private void HandleDirectoryDrop(string sourcePath)
        {
            string targetPath = @"C:\Exemple\Chemin\Pour\Dossiers";

            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }

            string targetFolder = Path.Combine(targetPath, Path.GetFileName(sourcePath));
            DirectoryCopy(sourcePath, targetFolder, true);

            ShowSuccess("Dossier importé avec succès !");
        }

        private void CopyFileToTarget(string sourcePath, string targetPath, string successMessage)
        {
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }

            string targetFile = Path.Combine(targetPath, Path.GetFileName(sourcePath));
            File.Copy(sourcePath, targetFile, true);

            ShowSuccess(successMessage);
        }

        private void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException($"Le dossier source n'existe pas : {sourceDirName}");
            }

            Directory.CreateDirectory(destDirName);

            foreach (FileInfo file in dir.GetFiles())
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, true);
            }

            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dir.GetDirectories())
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, true);
                }
            }
        }

        private void ShowSuccess(string message)
        {
            MessageBox.Show(message, "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowWarning(string message)
        {
            MessageBox.Show(message, "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = "https://github.com/saysaa/mcloader";
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
    }
}
