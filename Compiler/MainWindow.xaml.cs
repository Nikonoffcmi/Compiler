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
using System.Diagnostics;
using System.Windows.Automation.Peers;
using System.Collections.ObjectModel;
using System.Data;

namespace Compiler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int inputFonsize = 14;
        private int outputFonsize = 10;
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
                    FontSize = inputFonsize
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
                EditOption.IsEnabled = true;
                CancelButton.IsEnabled = true;
                RepeatButton.IsEnabled = true;
                CopyButton.IsEnabled = true;
                CutButton.IsEnabled = true;
                PasteButton.IsEnabled = true;
                CloseBtn.IsEnabled = true;
                RunButton.IsEnabled = true;
                CloseOption.IsEnabled = true;
                fs.Close();
                Condition.Content = "Редактирование";
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
                    FontSize = inputFonsize,
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
                EditOption.IsEnabled = true;
                CancelButton.IsEnabled = true;
                RepeatButton.IsEnabled = true;
                CopyButton.IsEnabled = true;
                CutButton.IsEnabled = true;
                PasteButton.IsEnabled = true; 
                CloseBtn.IsEnabled = true;
                RunButton.IsEnabled = true;
                CloseOption.IsEnabled = true;

                Condition.Content = "Редактирование";
            }
        }


        private void CloseTab(object sender, RoutedEventArgs e)
        {
            var tb = tabCont.SelectedItem as TabItem;
            var te = tb.Content as ICSharpCode.AvalonEdit.TextEditor;
            var mb = MessageBox.Show("Сохранить изменения?", "Подтверждение сохранения", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (mb == MessageBoxResult.Yes)
            {
                using (StreamWriter writer = new StreamWriter(tb.ToolTip.ToString(), false))
                {
                    writer.WriteLineAsync(te.Text);
                }

                tabFilename.Remove(tb.ToolTip.ToString());
                tabCont.Items.Remove(tb);
            }
            else if (mb == MessageBoxResult.No)
            {
                tabFilename.Remove(tb.ToolTip.ToString());
                tabCont.Items.Remove(tb);
            }
            else
                return;

            if (tabCont.Items.Count <= 0)
            {
                SaveAsOption.IsEnabled = false;
                SaveButton.IsEnabled = false;
                SaveOption.IsEnabled = false;
                RunButton.IsEnabled = false;
                RunOption.IsEnabled = false;
                EditOption.IsEnabled = false;
                CancelButton.IsEnabled = false;
                RepeatButton.IsEnabled = false;
                CopyButton.IsEnabled = false;
                CutButton.IsEnabled = false;
                PasteButton.IsEnabled = false;
                CloseOption.IsEnabled = false; 
                RunButton.IsEnabled = false;
                CloseBtn.IsEnabled = false;
                Condition.Content = "Ожидание";
            }

        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (tabCont.Items.Count > 0)
            {
                var mb = MessageBox.Show("Сохранить изменения?", "Подтверждение сохранения", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (mb == MessageBoxResult.Yes)
                {
                    foreach (TabItem item in tabCont.Items)
                    {
                        var te = item.Content as ICSharpCode.AvalonEdit.TextEditor;
                        using (StreamWriter writer = new StreamWriter(item.ToolTip.ToString(), false))
                        {
                            writer.WriteLineAsync(te.Text);
                        }
                    }
                }
                else if (mb == MessageBoxResult.Cancel) {
                    e.Cancel = true;
                    return;
                }
                else
                {
                }
            }
        }

        private void OutputFont_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OutputFont.SelectedValue == null)
            {
                dataGridResult.FontSize = 10;
            }
            else
            {
                var size = Convert.ToInt32(OutputFont.SelectedValue.ToString().Replace("System.Windows.Controls.ComboBoxItem: ", ""));
                dataGridResult.FontSize = size;
                outputFonsize = size;
            }

        }
        private void InputFont_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (InputFont.SelectedValue == null)
            {
                foreach (TabItem item in tabCont.Items)
                {
                    var te = item.Content as ICSharpCode.AvalonEdit.TextEditor;
                    te.FontSize = 14;
                }
                inputFonsize = 14;
            }
            else
            {
                var size = Convert.ToInt32(InputFont.SelectedValue.ToString().Replace("System.Windows.Controls.ComboBoxItem: ", ""));
                foreach (TabItem item in tabCont.Items)
                {
                    var te = item.Content as ICSharpCode.AvalonEdit.TextEditor;
                    te.FontSize = size;
                }
                inputFonsize = size;
            }
        }

        private void Undo(object sender, RoutedEventArgs e)
        {
            var tb = tabCont.SelectedItem as TabItem;
            var te = tb.Content as ICSharpCode.AvalonEdit.TextEditor;
            te.Undo();
        }

        private void Redo(object sender, RoutedEventArgs e)
        {
            var tb = tabCont.SelectedItem as TabItem;
            var te = tb.Content as ICSharpCode.AvalonEdit.TextEditor;
            te.Redo();
        }

        private void Copy(object sender, RoutedEventArgs e)
        {
            var tb = tabCont.SelectedItem as TabItem;
            var te = tb.Content as ICSharpCode.AvalonEdit.TextEditor;
            te.Copy();
        }

        private void Paste(object sender, RoutedEventArgs e)
        {
            var tb = tabCont.SelectedItem as TabItem;
            var te = tb.Content as ICSharpCode.AvalonEdit.TextEditor;
            te.Paste();
        }

        private void Cut(object sender, RoutedEventArgs e)
        {
            var tb = tabCont.SelectedItem as TabItem;
            var te = tb.Content as ICSharpCode.AvalonEdit.TextEditor;
            te.Cut();
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            var tb = tabCont.SelectedItem as TabItem;
            var te = tb.Content as ICSharpCode.AvalonEdit.TextEditor;
            te.Delete();
        }

        private void SelectAll(object sender, RoutedEventArgs e)
        {
            var tb = tabCont.SelectedItem as TabItem;
            var te = tb.Content as ICSharpCode.AvalonEdit.TextEditor;
            te.SelectAll();
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

        private void CallHelp(object sender, RoutedEventArgs e)
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(@"HTMLPage1.html")
            {
                UseShellExecute = true
            };
            p.Start();
        }
        private void CallAbout(object sender, RoutedEventArgs e)
        {
            var callAbout = new About();
            callAbout.ShowDialog();
        }

        private void DockPanel_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var dialog = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (tabFilename.Contains(dialog[0]))
                {
                    tabCont.SelectedItem = tabCont.Items.GetItemAt(tabFilename.IndexOf(dialog[0]));
                    return;
                }

                var filename = System.IO.Path.GetFileName(dialog[0]);
                string text;

                using (StreamReader reader = new StreamReader(dialog[0]))
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
                    FontSize = inputFonsize,
                    Text = text
                };

                var newTab = new TabItem { Header = filename, Content = txtEdit, ToolTip = dialog[0] };
                var tt = tabCont.Items.Add(newTab);
                tabCont.SelectedItem = newTab;
                tabFilename.Add(dialog[0]);

                SaveAsOption.IsEnabled = true;
                SaveButton.IsEnabled = true;
                SaveOption.IsEnabled = true;
                RunButton.IsEnabled = true;
                RunOption.IsEnabled = true;
                EditOption.IsEnabled = true;
                CancelButton.IsEnabled = true;
                RepeatButton.IsEnabled = true;
                CopyButton.IsEnabled = true;
                CutButton.IsEnabled = true;
                PasteButton.IsEnabled = true;
                CloseBtn.IsEnabled = true;
                CloseOption.IsEnabled = true;
                RunButton.IsEnabled = true;
                Condition.Content = "Редактирование";
            }
        }

        private void Analyzer(object sender, RoutedEventArgs e)
        {
            var analyzer = new LexicalAnalyzer();
            var tb = tabCont.SelectedItem as TabItem;
            var te = tb.Content as ICSharpCode.AvalonEdit.TextEditor;
            analyzer.AnalysisText(te.Text);
            dataGridResult.ItemsSource = analyzer.Lexemes;
        }
    }
}