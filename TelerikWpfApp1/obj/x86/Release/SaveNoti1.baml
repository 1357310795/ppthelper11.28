   M S B A M L   `   `   `  ����   TelerikWpfApp1    TelerikWpfApp1.SaveNoti1   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     V RMaterialDesignThemes.Wpf, Version=3.1.3.1513, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  
������ $��98��$	��279��$��5q�.��#  MaterialDesignThemes.Wpf.Card      UniformCornerRadius$  5Q�+  %MaterialDesignThemes.Wpf.ShadowAssist   ShadowDepth$ Depth3=�.��� .+�7  1MaterialDesignThemes.Wpf.Transitions.Transitioner  -   $��Transitioner�� �� SelectedIndex$ 1��.@���  �� Orientation$ Vertical=�$��Center=�.+��  %  !MaterialDesignCircularProgressBar#��[   � Value$ 0Q�$��40�� � IsIndeterminate . $��Center=�$��40��$��10q��� $��正在保存��$��20(�$��Center=��  � RowDefinitions �� $
��Auto ��� �� $
��Auto ��� $
��Auto �.+��� $ 
Horizontal=�$��0��$
��Left=�$	��Top=�$��5q�.+�'  !MaterialDesignThemes.Wpf.PackIcon     Kind$! CheckboxMarkedCircleOutline=�$��32��$��32�����	#FF00A024�� $��保存成功��$��20(�$��Center=�$��5,0,5,0q�  -   $ WindowClose=�$��32��$��32��$��0�� � Cursor$
 Hand��$	��Top=�$��Right=�$��5q����	#FFFF4D4D
��������� -   	 ��Name$	 succeedText��$��文件已保存于……��$��15(�$��1��$��	10,0,10,5q��� $��0,1,0,0q�$��0q�$��2��  MaterialDesignDivider#���  � $��3��
 � ColumnDefinitions
 �� $��1* ��� $
��Auto ��� $��1* �.+��� -     MaterialDesignFlatButton#��[ $��30��$��15(�$��0��.��打开文件�� $��1,0,0,0q�$��0q�$��1��#���  �� -   #��[ $��30��$��15(�$��2��.��打开文件夹�  �� $
��Auto ��� �� $
��Auto ��� $
��Auto �.+��� $ 
Horizontal=�$��0��$
��Left=�$	��Top=�$��5q�.+�  $ WarningDecagramOutline=�$��32��$��32��
���  ���� $��保存失败��$��20(�$��Center=�$��5,0,0,0q�  -   $ WindowClose=�$��32��$��32��$��0��$
 Hand��$	��Top=�$��Right=�$��5q����	#FFFF4D4D�� $��文件未保存。��$��15(�$��1��$��10,0,0,5q��� $��0,1,0,0q�$��0q�$��2��#���  �� -   #��[ $��30��$��15(�$��3��.��查看失败原因