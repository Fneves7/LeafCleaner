using System;
using System.Windows;
using System.Windows.Shapes;
using System.IO;
using System.Linq;
using Path = System.IO.Path;
using System.ComponentModel;

namespace LeafCleaner
{
    /// <summary>
    /// Interação lógica para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DateTime TimeNow = DateTime.Now;
        private Boolean _isWaiting;
        public Boolean IsWaiting
        {
            get { return _isWaiting; }
            set { _isWaiting = value; }
        }

        public MainWindow() {
            InitializeComponent();
        }

        private void CleanButton_Click(object sender, RoutedEventArgs e) {
            try {
                int counter = 0;
                string tempFolder = Path.GetTempPath();
                long initialSpace = GetDirectorySize(tempFolder);

                foreach (string file in Directory.GetFiles(tempFolder)) {
                    try
                    {
                        File.Delete(file);
                    } catch (IOException)
                    {
                        counter++;
                    }                    
                }

                long cleanedSpace = initialSpace - GetDirectorySize(tempFolder);
                double cleanedSpaceGB = (double)cleanedSpace / (1024 * 1024 * 1024);
                if (cleanedSpaceGB > 0)
                {
                    CleanedSpaceTextBox.Text = $"{cleanedSpaceGB:F2} GB cleaned successfully!\nFailed to clean {counter} files...";
                } else
                {
                    CleanedSpaceTextBox.Text = $"Failed to clean {counter} files...";
                }

                MessageBox.Show("Job done!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            } catch (Exception ex) {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenTempFolderButton_Click(object sender, RoutedEventArgs e) {
            try {
                string tempFolder = Path.GetTempPath();
                System.Diagnostics.Process.Start("explorer.exe", tempFolder);
            } catch (Exception ex) {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AnalyzeButton_Click(object sender, RoutedEventArgs e) {
            IsWaiting = true;

            string folderPath = Path.GetTempPath();
            CleanedSpaceTextBox.Clear();
            CleanedSpaceTextBox.Text = "Getting the folder size, please wait...";

            if (Directory.Exists(folderPath)) {
                long folderSizeBytes = GetDirectorySize(folderPath);
                double folderSizeGB = folderSizeBytes / (1024.0 * 1024.0 * 1024.0);
                CleanedSpaceTextBox.Text = $"Folder Size: {folderSizeGB:F2} GB";
            } else {
                MessageBox.Show("The specified folder does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            IsWaiting = false;
        }

        private long GetDirectorySize(string folderPath) {
            try {
                return Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories)
                                .Sum(filePath => (new FileInfo(filePath)).Length);
            } catch (UnauthorizedAccessException) {
                MessageBox.Show("Access to some files or directories is restricted. Try running the application as an administrator.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return -1;
            } catch (Exception) {
                MessageBox.Show("An error occurred while calculating the folder size.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return -1;
            }
        }


        private void Window_Closing(object sender, CancelEventArgs e) {
            var message = MessageBox.Show($"You are at middle of something, you sure you want to stop and quit the application?", "Error", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            switch (message)
            {
                case MessageBoxResult.Yes:
                    Application.Current.Shutdown();
                    break;
                default:
                    break;
            }
        }
    }
}