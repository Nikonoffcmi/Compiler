﻿using Microsoft.VisualBasic.FileIO;
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
using ICSharpCode.AvalonEdit.Rendering;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using MaterialDesignThemes.Wpf;
using System.Windows.Threading;

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
                    SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinition("C#"),
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
                    SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinition("C#"),
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
            p.StartInfo = new ProcessStartInfo(@"..\..\..\HTMLPage1.html")
            {
                UseShellExecute = true
            };
            p.Start();
            p.Close();
        }
        private void CallAbout(object sender, RoutedEventArgs e)
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(@"..\..\..\About.html")
            {
                UseShellExecute = true
            };
            p.Start();
            p.Close();
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
                    SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinition("C#"),
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
            }
        }


        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(@"..\..\..\problem.html")
            {
                UseShellExecute = true
            };
            p.Start();
            p.Close();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(@"..\..\..\Grammar.html")
            {
                UseShellExecute = true
            };
            p.Start();
            p.Close();
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(@"..\..\..\Classification.html")
            {
                UseShellExecute = true
            };
            p.Start();
            p.Close();
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(@"..\..\..\methodAnalysis.html")
            {
                UseShellExecute = true
            };
            p.Start();
            p.Close();
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(@"..\..\..\Neutralizing.html")
            {
                UseShellExecute = true
            };
            p.Start();
            p.Close();
        }

        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(@"..\..\..\testExamples.html")
            {
                UseShellExecute = true
            };
            p.Start();
            p.Close();
        }

        private void MenuItem_Click_6(object sender, RoutedEventArgs e)
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(@"..\..\..\LitList.html")
            {
                UseShellExecute = true
            };
            p.Start();
            p.Close();
        }

        private void MenuItem_Click_7(object sender, RoutedEventArgs e)
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(@"https://github.com/Nikonoffcmi/Compiler")
            {
                UseShellExecute = true
            };
            p.Start();
            p.Close();
        }

        private void Analyzer(object sender, RoutedEventArgs e)
        {
            var parcer = new Parser();
            var tb = tabCont.SelectedItem as TabItem;
            var te = tb.Content as ICSharpCode.AvalonEdit.TextEditor;
            parcer.Parse(te.Text);

            dataGridResult.ItemsSource = parcer.GetErrors();

            if (dataGridResult.Items.Count < 1)
            {
                MessageBox.Show("Ошибки не обнаружились!", "Успех!", MessageBoxButton.OK);
            }

            //te.TextArea.TextView.LineTransformers.Add(new ColorizeAvalonEdit());
        }

        private void dataGridResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tb = tabCont.SelectedItem as TabItem;
            var te = tb.Content as ICSharpCode.AvalonEdit.TextEditor;
            var pe = dataGridResult.SelectedValue as ParseError;
            if (pe != null)
            {
                int offs = pe.start;
                int offsend = pe.end + 1;
                for (int i = 1; i < pe.line; i++) 
                {
                    offs += te.Document.GetLineByNumber(i).TotalLength;
                    offsend += te.Document.GetLineByNumber(i).TotalLength;
                }
                te.Focus();
                int j = 1;
                while (offsend - offs < 0 && j <= pe.line)
                    offsend += te.Document.GetLineByNumber(j++).TotalLength;

                te.SelectionStart = offs - 1;
                te.SelectionLength = offsend - offs;
            }
        }

        private void dataGridResult_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var tb = tabCont.SelectedItem as TabItem;
            var te = tb.Content as ICSharpCode.AvalonEdit.TextEditor;
            var pe = dataGridResult.SelectedValue as ParseError;
            if (pe != null)
            {
                int offs = pe.start;
                int offsend = pe.end + 1;
                for (int i = 1; i < pe.line; i++)
                {
                    offs += te.Document.GetLineByNumber(i).TotalLength;
                    offsend += te.Document.GetLineByNumber(i).TotalLength;
                }
                te.Focus();

                int j = 1;
                while (offsend - offs < 0 && j <= pe.line)
                    offsend += te.Document.GetLineByNumber(j++).TotalLength;

                te.SelectionStart = offs - 1;
                te.SelectionLength = offsend - offs;
            }
        }

    }
    public class ColorizeAvalonEdit : DocumentColorizingTransformer
    {
        string text = "";
        protected override void ColorizeLine(DocumentLine line)
        {
            int lineStartOffset = line.Offset;
            text += CurrentContext.Document.GetText(line);
            if (line.NextLine != null)
                text += CurrentContext.Document.GetText(line);
            else
            {
                int start = 0;
                int index;
                var parcer = new Parser();
                parcer.Parse(text);
                foreach (var ep in parcer.GetErrors())
                {
                    base.ChangeLinePart(
                        lineStartOffset + ep.start, // startOffset
                        lineStartOffset + ep.end, // endOffset
                        (VisualLineElement element) =>
                        {
                            // This lambda gets called once for every VisualLineElement
                            // between the specified offsets.
                            Typeface tf = element.TextRunProperties.Typeface;
                            TextDecoration myUnderline = new TextDecoration();

                            // Create a linear gradient pen for the text decoration.
                            Pen myPen = new Pen(Brushes.Red, 1);
                            myPen.Thickness = 1.5;
                            myPen.DashStyle = DashStyles.Dash;
                            myUnderline.Pen = myPen;
                            myUnderline.PenThicknessUnit = TextDecorationUnit.FontRecommended;

                            // Set the underline decoration to a TextDecorationCollection and add it to the text block.
                            TextDecorationCollection myCollection = new TextDecorationCollection();
                            myCollection.Add(myUnderline);
                            element.TextRunProperties.SetTextDecorations(myCollection);
                            element.TextRunProperties.SetBaselineAlignment(BaselineAlignment.Center);
                        });
                }
            }
        }
    }
}