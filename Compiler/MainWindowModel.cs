using ICSharpCode.AvalonEdit;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Compiler
{
    public class ViewModel
    {
        private string _test;
        public string Test
        {
            get { return _test; }
            set { _test = value; }
        }
    }
    public sealed class AvalonEditBehaviour : Behavior<TextEditor>
    {
        public static readonly DependencyProperty GiveMeTheTextProperty =
            DependencyProperty.Register("GiveMeTheText", typeof(string), typeof(AvalonEditBehaviour),
            new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PropertyChangedCallback));

        public string GiveMeTheText
        {
            get { return (string)GetValue(GiveMeTheTextProperty); }
            set { SetValue(GiveMeTheTextProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject != null)
                AssociatedObject.TextChanged += AssociatedObjectOnTextChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (AssociatedObject != null)
                AssociatedObject.TextChanged -= AssociatedObjectOnTextChanged;
        }

        private void AssociatedObjectOnTextChanged(object sender, EventArgs eventArgs)
        {
            var textEditor = sender as TextEditor;
            if (textEditor != null)
            {
                if (textEditor.Document != null)
                    GiveMeTheText = textEditor.Document.Text;
            }
        }

        private static void PropertyChangedCallback(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var behavior = dependencyObject as AvalonEditBehaviour;
            if (behavior.AssociatedObject != null)
            {
                var editor = behavior.AssociatedObject as TextEditor;
                if (editor.Document != null)
                {
                    var caretOffset = editor.CaretOffset;
                    editor.Document.Text = dependencyPropertyChangedEventArgs.NewValue.ToString();
                    editor.CaretOffset = caretOffset;
                }
            }
        }
    }

    internal class MainWindowModel
    {
        public ObservableCollection<TabModel> Tabs { get; } = new();

        private TabModel? _selectedTab;

        private bool _isDarkTheme;


        public MainWindowModel()
        {
            BindingOperations.EnableCollectionSynchronization(Tabs, new());

        }

        public async Task OpenFile()
        {
            if (GetFile() is { } file)
            {
            }
        }

        private void CloseTab(TabModel tabViewModel)
        {
            Tabs.Remove(tabViewModel);
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
