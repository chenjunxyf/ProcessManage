﻿#pragma checksum "..\..\..\3在线用户\OnLineUsersPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "C33BF638DDE4B4B86EFBAE033A3B7AD5"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:2.0.50727.4963
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using Processes_Manage;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
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


namespace Processes_Manage {
    
    
    /// <summary>
    /// OnLineUsersPage
    /// </summary>
    public partial class OnLineUsersPage : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 62 "..\..\..\3在线用户\OnLineUsersPage.xaml"
        internal System.Windows.Controls.Button connectServer_Button;
        
        #line default
        #line hidden
        
        
        #line 63 "..\..\..\3在线用户\OnLineUsersPage.xaml"
        internal System.Windows.Controls.Button readUsers_Button;
        
        #line default
        #line hidden
        
        
        #line 67 "..\..\..\3在线用户\OnLineUsersPage.xaml"
        internal System.Windows.Controls.ListView usersListView;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Processes_Manage;component/3%e5%9c%a8%e7%ba%bf%e7%94%a8%e6%88%b7/onlineuserspage" +
                    ".xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\3在线用户\OnLineUsersPage.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 5 "..\..\..\3在线用户\OnLineUsersPage.xaml"
            ((Processes_Manage.OnLineUsersPage)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Page_Loaded);
            
            #line default
            #line hidden
            return;
            case 3:
            this.connectServer_Button = ((System.Windows.Controls.Button)(target));
            
            #line 62 "..\..\..\3在线用户\OnLineUsersPage.xaml"
            this.connectServer_Button.Click += new System.Windows.RoutedEventHandler(this.connectServer_Button_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.readUsers_Button = ((System.Windows.Controls.Button)(target));
            
            #line 63 "..\..\..\3在线用户\OnLineUsersPage.xaml"
            this.readUsers_Button.Click += new System.Windows.RoutedEventHandler(this.readUsers_Button_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.usersListView = ((System.Windows.Controls.ListView)(target));
            return;
            }
            this._contentLoaded = true;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 2:
            
            #line 51 "..\..\..\3在线用户\OnLineUsersPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.LookOver_Button_Click);
            
            #line default
            #line hidden
            break;
            case 6:
            
            #line 80 "..\..\..\3在线用户\OnLineUsersPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.UserManage_Button_Click);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}
