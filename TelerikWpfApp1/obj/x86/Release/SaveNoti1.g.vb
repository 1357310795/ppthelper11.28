﻿#ExternalChecksum("..\..\..\SaveNoti1.xaml","{8829d00f-11b8-4213-878b-770e8597ac16}","9C352A252AF874DA7F14D297C3EF834BE9B240C1B032C8FC4E8B2389D6CED3A6")
'------------------------------------------------------------------------------
' <auto-generated>
'     此代码由工具生成。
'     运行时版本:4.0.30319.42000
'
'     对此文件的更改可能会导致不正确的行为，并且如果
'     重新生成代码，这些更改将会丢失。
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict Off
Option Explicit On

Imports MaterialDesignThemes.Wpf
Imports MaterialDesignThemes.Wpf.Converters
Imports MaterialDesignThemes.Wpf.Transitions
Imports System
Imports System.Diagnostics
Imports System.Windows
Imports System.Windows.Automation
Imports System.Windows.Controls
Imports System.Windows.Controls.Primitives
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Ink
Imports System.Windows.Input
Imports System.Windows.Markup
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Media.Effects
Imports System.Windows.Media.Imaging
Imports System.Windows.Media.Media3D
Imports System.Windows.Media.TextFormatting
Imports System.Windows.Navigation
Imports System.Windows.Shapes
Imports System.Windows.Shell


'''<summary>
'''SaveNoti1
'''</summary>
<Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>  _
Partial Public Class SaveNoti1
    Inherits System.Windows.Controls.UserControl
    Implements System.Windows.Markup.IComponentConnector
    
    
    #ExternalSource("..\..\..\SaveNoti1.xaml",10)
    <System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")>  _
    Friend WithEvents Transitioner As MaterialDesignThemes.Wpf.Transitions.Transitioner
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\SaveNoti1.xaml",33)
    <System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")>  _
    Friend WithEvents succeedText As System.Windows.Controls.TextBlock
    
    #End ExternalSource
    
    Private _contentLoaded As Boolean
    
    '''<summary>
    '''InitializeComponent
    '''</summary>
    <System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")>  _
    Public Sub InitializeComponent() Implements System.Windows.Markup.IComponentConnector.InitializeComponent
        If _contentLoaded Then
            Return
        End If
        _contentLoaded = true
        Dim resourceLocater As System.Uri = New System.Uri("/TelerikWpfApp1;component/savenoti1.xaml", System.UriKind.Relative)
        
        #ExternalSource("..\..\..\SaveNoti1.xaml",1)
        System.Windows.Application.LoadComponent(Me, resourceLocater)
        
        #End ExternalSource
    End Sub
    
    <System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never),  _
     System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes"),  _
     System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"),  _
     System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")>  _
    Sub System_Windows_Markup_IComponentConnector_Connect(ByVal connectionId As Integer, ByVal target As Object) Implements System.Windows.Markup.IComponentConnector.Connect
        If (connectionId = 1) Then
            Me.Transitioner = CType(target,MaterialDesignThemes.Wpf.Transitions.Transitioner)
            Return
        End If
        If (connectionId = 2) Then
            
            #ExternalSource("..\..\..\SaveNoti1.xaml",32)
            AddHandler CType(target,MaterialDesignThemes.Wpf.PackIcon).MouseUp, New System.Windows.Input.MouseButtonEventHandler(AddressOf Me.Close_MouseUp)
            
            #End ExternalSource
            Return
        End If
        If (connectionId = 3) Then
            Me.succeedText = CType(target,System.Windows.Controls.TextBlock)
            Return
        End If
        If (connectionId = 4) Then
            
            #ExternalSource("..\..\..\SaveNoti1.xaml",44)
            AddHandler CType(target,System.Windows.Controls.Button).Click, New System.Windows.RoutedEventHandler(AddressOf Me.ButtonOpen_Click)
            
            #End ExternalSource
            Return
        End If
        If (connectionId = 5) Then
            
            #ExternalSource("..\..\..\SaveNoti1.xaml",51)
            AddHandler CType(target,System.Windows.Controls.Button).Click, New System.Windows.RoutedEventHandler(AddressOf Me.ButtonOpenDir_Click)
            
            #End ExternalSource
            Return
        End If
        If (connectionId = 6) Then
            
            #ExternalSource("..\..\..\SaveNoti1.xaml",71)
            AddHandler CType(target,MaterialDesignThemes.Wpf.PackIcon).MouseUp, New System.Windows.Input.MouseButtonEventHandler(AddressOf Me.Close_MouseUp)
            
            #End ExternalSource
            Return
        End If
        If (connectionId = 7) Then
            
            #ExternalSource("..\..\..\SaveNoti1.xaml",77)
            AddHandler CType(target,System.Windows.Controls.Button).Click, New System.Windows.RoutedEventHandler(AddressOf Me.ButtonFail_Click)
            
            #End ExternalSource
            Return
        End If
        Me._contentLoaded = true
    End Sub
End Class

