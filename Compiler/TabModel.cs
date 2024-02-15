using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    internal class TabModel
    {
        private string? _title;

        private TextDocument? _textDocument;

        public void OnTextDocumentChanged(TextDocument? oldValue, TextDocument? newValue)
        {
            if (oldValue is not null)
            {
                oldValue.Changed -= TextDocumentChanged;
            }
            if (newValue is not null)
            {
                newValue.Changed += TextDocumentChanged;
            }

            void TextDocumentChanged(object? sender, DocumentChangeEventArgs e)
            {
                //IsDirty = true;
            }
        }

        private IHighlightingDefinition? _highlightingDefinition;

        private bool _isDirty;

        public TabModel()
        {
        }


        public async Task LoadFileAsync(FileInfo file)
        {
            //Title = file.Name;
            //HighlightingDefinition = HighlightingManager.Instance.GetDefinitionByExtension(file.Extension);
            //TextDocument ??= new();
            //TextDocument.FileName = file.FullName;
            //using StreamReader reader = file.OpenText();
            //TextDocument.Text = await reader.ReadToEndAsync();
            //IsDirty = false;
        }

        public void Close()
        {
        }

        //public async Task Save()
        //{
        //    if (TextDocument is not null)
        //    {
        //        using StreamWriter writer = new(TextDocument.FileName);
        //        await writer.WriteAsync(TextDocument.Text);
        //        IsDirty = false;
        //    }
        //}

        //private bool CanSave() => IsDirty;
    }
}
