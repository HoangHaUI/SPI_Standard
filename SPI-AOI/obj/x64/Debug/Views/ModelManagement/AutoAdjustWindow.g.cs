﻿#pragma checksum "..\..\..\..\..\Views\ModelManagement\AutoAdjustWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "A1943ED749B55665697A65D12082D721D230FA40"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using SPI_AOI.Views;
using SPI_AOI.Views.ModelManagement;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace SPI_AOI.Views.ModelManagement {
    
    
    /// <summary>
    /// AutoAdjustWindow
    /// </summary>
    public partial class AutoAdjustWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 21 "..\..\..\..\..\Views\ModelManagement\AutoAdjustWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label label;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\..\..\..\Views\ModelManagement\AutoAdjustWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cbGerber;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\..\..\..\Views\ModelManagement\AutoAdjustWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label label_Copy;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\..\..\..\Views\ModelManagement\AutoAdjustWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cbCad;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\..\..\..\Views\ModelManagement\AutoAdjustWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/SPI-AOI;component/views/modelmanagement/autoadjustwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Views\ModelManagement\AutoAdjustWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 11 "..\..\..\..\..\Views\ModelManagement\AutoAdjustWindow.xaml"
            ((SPI_AOI.Views.ModelManagement.AutoAdjustWindow)(target)).Initialized += new System.EventHandler(this.Window_Initialized);
            
            #line default
            #line hidden
            return;
            case 2:
            this.label = ((System.Windows.Controls.Label)(target));
            return;
            case 3:
            this.cbGerber = ((System.Windows.Controls.ComboBox)(target));
            
            #line 22 "..\..\..\..\..\Views\ModelManagement\AutoAdjustWindow.xaml"
            this.cbGerber.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.cbGerber_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 4:
            this.label_Copy = ((System.Windows.Controls.Label)(target));
            return;
            case 5:
            this.cbCad = ((System.Windows.Controls.ComboBox)(target));
            
            #line 33 "..\..\..\..\..\Views\ModelManagement\AutoAdjustWindow.xaml"
            this.cbCad.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.cbCad_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 6:
            this.button = ((System.Windows.Controls.Button)(target));
            
            #line 43 "..\..\..\..\..\Views\ModelManagement\AutoAdjustWindow.xaml"
            this.button.Click += new System.Windows.RoutedEventHandler(this.button_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

