﻿using ICSharpCode.AvalonEdit;
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
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Xaml;

namespace Compiler
{
    internal abstract class ViewModel : MarkupExtension, INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
            var handlers = PropertyChanged;
            if (handlers is null) return;

            var invocation_list = handlers.GetInvocationList();

            var arg = new PropertyChangedEventArgs(PropertyName);
            foreach (var action in invocation_list)
                if (action.Target is DispatcherObject disp_object)
                    disp_object.Dispatcher.Invoke(action, this, arg);
                else
                    action.DynamicInvoke(this, arg);
        }

        protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string PropertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(PropertyName);
            return true;
        }

        public override object ProvideValue(IServiceProvider sp)
        {
            var value_target_service = sp.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            var root_object_service = sp.GetService(typeof(IRootObjectProvider)) as IRootObjectProvider;

            OnInitialized(
                value_target_service?.TargetObject,
                value_target_service?.TargetProperty,
                root_object_service?.RootObject);

            return this;
        }

        private WeakReference _TargetRef;
        private WeakReference _RootRef;

        public object TargetObject => _TargetRef.Target;

        public object RootObject => _RootRef.Target;

        protected virtual void OnInitialized(object Target, object Property, object Root)
        {
            _TargetRef = new WeakReference(Target);
            _RootRef = new WeakReference(Root);
        }


        public void Dispose()
        {
            Dispose(true);
        }

        private bool _Disposed;
        protected virtual void Dispose(bool Disposing)
        {
            if (!Disposing || _Disposed) return;
            _Disposed = true;
            // Освобождение управляемых ресурсов
        }
    }

    internal class MainWindowModel : ViewModel
    {
        //public ObservableCollection<TabModel> Tabs { get; } = new();

        //private TabModel? _selectedTab;

        public int[] _isDarkTheme = new int[4];
        public int _DarkTheme = 50;


        public MainWindowModel()
        {
            //BindingOperations.EnableCollectionSynchronization(Tabs, new());
            string FileName = "fir.cs";
            ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(FileName));
            
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        //public async Task OpenFile()
        //{
        //    if (GetFile() is { } file)
        //    {
        //    }
        //}

        //private void CloseTab(TabModel tabViewModel)
        //{
        //    Tabs.Remove(tabViewModel);
        //}





    }
}
