using Microsoft.VisualBasic.FileIO;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.Xml.Linq;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit;
using Microsoft.Win32;

namespace Compiler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private List<string> tabFilename = new List<string>();
        private void CreateFileDialog(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.FileName = "File"; 
            dialog.DefaultExt = ".cpp";
            dialog.Filter = "All Files (*.*)|*.*"; 

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                if (tabFilename.Contains(dialog.FileName)){
                    FileStream fis = File.Create(dialog.FileName);
                    var tb = tabCont.Items.GetItemAt(tabFilename.IndexOf(dialog.FileName)) as TabItem;
                    tabCont.SelectedItem = tb;
                    var te = tb.Content as ICSharpCode.AvalonEdit.TextEditor;
                    te.Text = "";
                    fis.Close();
                    return;
                }

                var filename = System.IO.Path.GetFileName(dialog.FileName);

                FileStream fs = File.Create(dialog.FileName);
                var txtEdit = new TextEditor()
                {
                    WordWrap = true,
                    ShowLineNumbers = true,
                    LineNumbersForeground = Brushes.Black,
                    FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                    SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinitionByExtension(System.IO.Path.GetExtension(filename)),
                    FontSize = 13
                };

                var newTab = new TabItem { Header = filename, Content = txtEdit, ToolTip = dialog.FileName };
                var tt = tabCont.Items.Add(newTab);
                tabCont.SelectedItem = newTab;
                tabFilename.Add(dialog.FileName);

                SaveAsOption.IsEnabled = true;
                SaveButton.IsEnabled = true;
                SaveOption.IsEnabled = true;
                RunButton.IsEnabled = true;
                RunOption.IsEnabled = true;
                CloseFileOption.IsEnabled = true;
                EditOption.IsEnabled = true;
                CancelButton.IsEnabled = true;
                RepeatButton.IsEnabled = true;
                CopyButton.IsEnabled = true;
                CutButton.IsEnabled = true;
                PasteButton.IsEnabled = true;
                fs.Close();
            }
        }
        private void SaveAsFileDialog(object sender, RoutedEventArgs e)
        {
            var tb = tabCont.SelectedItem as TabItem;

            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.FileName = System.IO.Path.GetFileNameWithoutExtension(tb.Header.ToString());
            dialog.DefaultExt = System.IO.Path.GetExtension(tb.Header.ToString());
            dialog.Filter = "All Files (*.*)|*.*";

            var index = tabFilename.IndexOf(tb.ToolTip.ToString());
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                var filename = dialog.FileName;

                FileStream fs = File.Create(filename);
                fs.Close();
                var te = tb.Content as ICSharpCode.AvalonEdit.TextEditor;
                using (StreamWriter writer = new StreamWriter(filename, false))
                {
                    writer.WriteLineAsync(te.Text);
                }
                MessageBox.Show("Данные сохранены в " + filename);

                var name = System.IO.Path.GetFileName(filename);
                tb.Header = name;
                tabFilename[index] = filename;
            }
        }

        private void SaveFileDialog(object sender, RoutedEventArgs e)
        {
            var tb = tabCont.SelectedItem as TabItem;
            var te = tb.Content as ICSharpCode.AvalonEdit.TextEditor;
            using (StreamWriter writer = new StreamWriter(tb.ToolTip.ToString(), false))
            {
                writer.WriteLineAsync(te.Text);
            }

            MessageBox.Show("Данные сохранены в " + tb.ToolTip.ToString());
        }

        private void OpenFileDialog(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "All Files (*.*)|*.*"; 

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                if (tabFilename.Contains(dialog.FileName))
                {
                    tabCont.SelectedItem = tabCont.Items.GetItemAt(tabFilename.IndexOf(dialog.FileName));
                    return;
                }

                var filename = System.IO.Path.GetFileName(dialog.FileName);
                string text;

                using (StreamReader reader = new StreamReader(dialog.FileName))
                {
                    text = reader.ReadToEnd();
                }

                var txtEdit = new TextEditor()
                {
                    WordWrap = true,
                    ShowLineNumbers = true,
                    LineNumbersForeground = Brushes.Black,
                    FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                    SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinitionByExtension(System.IO.Path.GetExtension(filename)),
                    FontSize = 13,
                    Text = text
                };

                var newTab = new TabItem { Header = filename, Content = txtEdit, ToolTip = dialog.FileName };
                var tt = tabCont.Items.Add(newTab);
                tabCont.SelectedItem = newTab;
                tabFilename.Add(dialog.FileName);

                SaveAsOption.IsEnabled = true;
                SaveButton.IsEnabled = true;
                SaveOption.IsEnabled = true;
                RunButton.IsEnabled = true;
                RunOption.IsEnabled = true;
                CloseFileOption.IsEnabled = true;
                EditOption.IsEnabled = true;
                CancelButton.IsEnabled = true;
                RepeatButton.IsEnabled = true;
                CopyButton.IsEnabled = true;
                CutButton.IsEnabled = true;
                PasteButton.IsEnabled = true;
            }
        }


        private void CloseFile(object sender, RoutedEventArgs e)
        {
            //CloseFileWindow closeFileWindow = new CloseFileWindow();

            //if (closeFileWindow.ShowDialog() == true)
            //{
            //    using (StreamWriter writer = new StreamWriter(filename, false))
            //    {
            //        writer.WriteLineAsync(Input.Text);
            //    }
            //}
            //else if (closeFileWindow.IsCanceled) { }
            //else
            //{
            //    return;
            //}
            //Input.IsEnabled = false;
            //SaveAsOption.IsEnabled = false;
            //SaveButton.IsEnabled = false;
            //SaveOption.IsEnabled = false;
            //RunButton.IsEnabled = false;
            //RunOption.IsEnabled = false;
            //CloseFileOption.IsEnabled = false;
            //EditOption.IsEnabled = false;
            //CancelButton.IsEnabled = false;
            //RepeatButton.IsEnabled = false;
            //CopyButton.IsEnabled = false;
            //CutButton.IsEnabled = false;
            //PasteButton.IsEnabled = false;
            //filename = "";
        }
        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            //if (filename != "")
            //{
            //    CloseFileWindow closeFileWindow = new CloseFileWindow();
            //    e.Cancel = false;
            //    if (closeFileWindow.ShowDialog() == true)
            //    {
            //        using (StreamWriter writer = new StreamWriter(filename, false))
            //        {
            //            writer.WriteLineAsync(Input.Text);
            //        }
            //    }
            //    else if (closeFileWindow.IsCanceled) { }
            //    else if (closeFileWindow.IsClosed)
            //    {
            //        e.Cancel = true;
            //        return;
            //    }

            //}
        }

        private void Undo(object sender, RoutedEventArgs e)
        {
            //Input.Undo();
        }

        private void Redo(object sender, RoutedEventArgs e)
        {
            //Input.Redo();
        }

        private void Copy(object sender, RoutedEventArgs e)
        {
            //Input.Copy();
        }

        private void Paste(object sender, RoutedEventArgs e)
        {
            //Input.Paste();
        }

        private void Cut(object sender, RoutedEventArgs e)
        {
            //Input.Cut();
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            //Input.Cut();
            Clipboard.Clear();
        }

        private void SelectAll(object sender, RoutedEventArgs e)
        {
            //Input.SelectAll();
        }

        private static FileInfo? GetFile()
        {
            OpenFileDialog openFileDialog = new()
            {
                Title = "Open File",
                Filter = "All Files (*.*)|*.*",
                CheckFileExists = true,
                CheckPathExists = true,
                RestoreDirectory = true,
            };
            if (openFileDialog.ShowDialog() == true)
            {
                return new FileInfo(openFileDialog.FileName);
            }
            return null;
        }
    }
}