   M S B A M L   `   `   `  ���� I  ETelerikWpfApp1, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null/clr-namespace:TelerikWpfApp1TelerikWpfApp1  /clr-namespace:TelerikWpfApp1TelerikWpfApp1       TelerikWpfApp1.MainWindow1   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     7telerikwpfapp1clr-namespace:TelerikWpfApp1     V RMaterialDesignThemes.Wpf, Version=3.1.3.1513, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  (localclr-namespace:TelerikWpfApp1  -     � Title$,  &HandyDraw BlackBoard Ver. by Hikari✨��$
��1080��$
��1920�� � Icon$ Resources/项目2.ico�� � WindowStyle$
 None=�
������  � AllowsTransparency .  � ShowInTaskbar .   � Topmost .  � 	Resources    ListBoxItemStyle �� 
TargetType ! MaterialDesignToolRadioButton �� BasedOn��%%   &	        7  �� " 	�8   .�-   .��!   TelerikWpfApp1.DpiDecorator  ._�)  #MaterialDesignThemes.Wpf.DialogHost  -   	  Name$	 MainDialogHost1��
   
Identifier$
 MainDialogHost1��
������ .��  -   $	 MainDialogHost2��$
 MainDialogHost2��
������ .��� -    �Name$ MainGrid��
������ .+��� -    ��Name$ MainContentControl��
������ �� -    ��Name$ 
InkCanvas1��$y�Visible=�  FakeTransparent �� 
Background# [  J� IsHitTestVisible .  J� RenderTransformOrigin$ 0.5,0.5&� � ContextMenu �� .@�m� -   $��	上一页��m� -	   $��	下一页��m� -
   $��结束放映��� -   $ 	BoardGrid��$	��999��$
��Left=�$	��Top=�$y�	Collapsed=�����  �� ElementName$ MainGrid�� �� Path$ ActualWidth�.+�   TelerikWpfApp1.BoardView  -     Name$ bv��� -   $ 	GridTools��$��Center=�$��Bottom=�$��172,0,173,10q�$ 0.5,1&�$��50�� J� Opacity$	 0.6Q� � ColumnDefinitions �� $
��Auto ��� $
��Auto �.+��� -    ��Name$ VisibilityControl��$y�	Collapsed=�$��0��$��0��$��12q�#  MaterialDesignThemes.Wpf.Card     UniformCornerRadius$ 3Q�$��0q�$��1��.���� -    ��Name$ listboxTools��$
��Left=� �� Orientation$ 
Horizontal=�$��0q�$ 0.5,0.5&�z�V� .~��� �� �� T�  T� Y$ 0Q�.+�	� -    	�Name$ CursorRadioButton��#��[   d� 	IsChecked$
 TrueY�
 � Tag$ Cursor�� 	� 	GroupName$ Tools������ $ VisibilityControl��$ Padding�.���   � RowDefinitions  �� $
��Auto ��� $
��Auto �.+�'  !MaterialDesignThemes.Wpf.PackIcon  $��Center=�$��0��!   Kind$! CursorDefault=�$��24��$��24���� $��1��$��指针��$��7,4,7,0q�$��14(�
���   �y��� $ VisibilityControl��$ 
Visibility�	� -   $ SelectRadioButton��#��[  $ FalseY�$ select��$ Tools������ $ VisibilityControl��$ Padding�y��� $ VisibilityControl��$ 
Visibility�.���   �� $
��Auto ��� $
��Auto �.+�  $��Center=�$��0��$! Select=�$��24��$��24���� $��1��$��选择��$��7,4,7,0q�$��14(�
���   �y��� $ VisibilityControl��$ 
Visibility��� -   " ��Name$" FreeScaleButton��  MaterialDesignFlatButton#��[ $
��Auto��y��� $ VisibilityControl��$ 
Visibility����� $ VisibilityControl��$ Padding�.���   �� $
��Auto ��� $
��Auto �.+�  $��Center=�$��0��$! ArrowTopRightBottomLeft=�$��24��$��24�����	#FF7C7C7C�� $��1��$��缩放��$��7,4,7,0q����	#FF787878$��14(�y��� $ VisibilityControl��$ 
Visibility�	� -   $ PenRadioButton��#��[  $ FalseY�$	 Pen��$ Tools������ $ VisibilityControl��$ Padding�.���   �� $
��Auto ��� $
��Auto �.+�  $��Center=�$! 
LeadPencil=�$��24��$��24���� $��1��$��标注��$��7,4,7,0q�$��14(�
���   �y��� $ VisibilityControl��$ 
Visibility�'  !MaterialDesignThemes.Wpf.PopupBox  -   #  Name$# PenSettingPopup��$   	StaysOpen$ . {�. $��2��%   PlacementMode$% TopAndAlignLeftEdges=�&   PopupHorizontalOffset$& -5Q�'   PopupVerticalOffset$' 40Q���Z� (   ToggleContent( Z� )   PopupContent.) #   TelerikWpfApp1.PenSettingMenu  -   *  Name$* 
PenSetting��A� -   + A�Name$+ PenColorTip��
���   �$��7��$��7��$��Bottom=�$��Right=�$��0��$��0,0,1,0q�	� -   $ MarkerRadioButton��#��[  $ FalseY�$ Marker��$ Tools������ $ VisibilityControl��$ Padding�y��� $ VisibilityControl��$ 
Visibility�.���   �� $
��Auto ��� $
��Auto �.+�  $��Center=�$! Marker=�$��Center=�$��24��$��24���� $��1��$��	荧光笔��$��7,4,7,0q�$��14(�
���   �y��� $ VisibilityControl��$ 
Visibility�  -   $# MarkerSettingPopup��$ . {�. $��2��$% TopAndAlignLeftEdges=�$& -5Q�$' 40Q���Z� ( Z� .) &    TelerikWpfApp1.MarkerSettingMenu  -   ,  Name$, MarkerSetting���� -   - ��Name$- MarkerColorTip��
���   �$��7��$��7��$��Bottom=�$��Right=�$��0��$��0,0,1,0q�	� -   $ EraserRadioButton��#��[  $ FalseY�$ Eraser��$ Tools������ $ VisibilityControl��$ Padding�.���   �� $
��Auto ��� $
��Auto �.+�  $��Center=�$! Eraser=�$��24��$��24���� $��1��$��擦除��$��7,4,7,0q�$��14(�
���   �y��� $ VisibilityControl��$ 
Visibility�  -   $# EraserSettingPopup��$ . {�. $��2��$% TopAndAlignLeftEdges=�$& -5Q�$' 40Q���Z� ( Z� .) &	    TelerikWpfApp1.EraserSettingMenu	  -   . 	 Name$. EraserSetting��	� -   #��[  $ FalseY�$
 Undo��$ Tools1������ $ VisibilityControl��$ Padding�.���   �� $
��Auto ��� $
��Auto �.+�  $��Center=�$��0��$
! Undo=�$��24��$��24���� $��1��$��撤销��$��7,4,7,0q�$��14(�
���   �y��� $ VisibilityControl��$ 
Visibility�	� -   #��[  $ FalseY�$
 Redo��$ Tools1������ $ VisibilityControl��$ Padding�y��� $ VisibilityControl��$ 
Visibility�.���   �� $
��Auto ��� $
��Auto �.+�  $��Center=�$��0��$
! Redo=�$��24��$��24���� $��1��$��重做��$��7,4,7,0q�$��14(�
���   �y��� $ VisibilityControl��$ 
Visibility�	� -    #��[  $ FalseY�$ Setting��$ Setting������ $ VisibilityControl��$ Padding�.���   �� $
��Auto ��� $
��Auto �.+�  $��Center=�$
! Apps=�$��24��$��24���� $��1��$��更多��$��7,4,7,0q�$��14(�
���   �y��� $ VisibilityControl��$ 
Visibility�  -!   $# 	MorePopup��$ . {�. $��2��$% TopAndAlignLeftEdges=�$& -5Q�$' 40Q���Z� ( Z� .) 
   TelerikWpfApp1.MoreMenu
  -"   / 
 Name$
/ More���� -#   #��[ $
��Auto������ $ VisibilityControl��$ Padding�.���   �� $
��Auto ��� $
��Auto �.+�  $��Center=�$��0��$! Presentation=�$��24��$��24��
���   �  $��0��$! WindowClose=�$��Center=�$��Center=�
���   �$��11��$��11��$��0,0,0,5q��� $��1��$��结束放映��$��2,4,2,0q�$��14(�
���   �y��� $ VisibilityControl��$ 
Visibility�  $ 3Q�$��0q�$��0,0,12,0q�.���� $��Center=�$ 
Horizontal=�$��0q�.+��� -$   #��[ $
��Auto��$
��Auto������ $ VisibilityControl��$ Padding�.���   �� $
��Auto ��� $
��Auto �.+�  -%   0  Name$0 ButtonBoardIcon��$��Center=�$��0��$! WebAsset=�$��24��$��24��
���   ��� -&   1 ��Name$1 ButtonBoardText��$��1��$��白板��$��7,4,7,0q�$��14(�
���   �y��� $ VisibilityControl��$ 
Visibility��� -'   #��[ $
��Auto��$
��Auto������ $ VisibilityControl��$ Padding�y��� $ VisibilityControl��$ 
Visibility�.���   �� $
��Auto ��� $
��Auto �.+�  -(   $0 ButtonCameraIcon��$��Center=�$��0��$! 	WebCamera=�$��24��$��24��
���   ��� -)   $1 ButtonCameraText��$��1��$��视频展台��$��2,4,2,0q�$��14(�
���   �y��� $ VisibilityControl��$ 
Visibility��� -*   $ NotiStackPanel��$
��Auto��$
��Auto��$	��Top=�$��Right=�$��	0,15,15,0q�$y�	Collapsed=�� -+   $ GridLeft��$
��Left=�$��Bottom=�$��
15,0,15,10q�$ 0.5,0.5&�$��60��$	 0.3Q�.+�  $ 3Q�$��0q�.���� -,   $ listboxLeft��$��Center=�$ 
Horizontal=�$��0q�$ 0.5,0.5&�z�V� .~��� �� �� T� $ 0Q�.+��� --   #��[ $��60��.��� $	��0,2q�  �� �� .+�  $��Center=�$��0��$! KeyboardArrowLeft=�$��24��$��24��
���   ��� $��Center=�$��1��$��	上一页��$��0,5,0,0q�
���   �$��14(�� $��5,0,5,-1q�  �� �� .+��� -.   $1 TextPageLeft��$��Center=�$��0��$��0,10,0,0q�
���   �$��16(����� $ TextPage��$
 Text��� $��Center=�$��1��$��页码��$��0,5,0,0q�
���   �$��14(��� -/   #��[ $��60��.��� $	��0,2q�  �� �� .+�  $��Center=�$��0��$��24��$��24��
���   �! �� $ PageControlNextIcon��$
 Kind��� $��Center=�$��1��$��0,5,0,0q�
���   ����� $ PageControlNextText��$
 Text�� -0   $ 	GridRight��$��Right=�$��Bottom=�$��
15,0,15,10q�$ 0.5,0.5&�$��60�� �� $ GridLeft��$ Opacity�.+�  $ 3Q�$��0q�.���� -1   $ listboxRight��$��Center=�$ 
Horizontal=�$��0q�$ 0.5,0.5&�z�V� .~��� �� �� T� $ 0Q�.+��� -2   #��[ $��60��.��� $	��0,2q�  �� �� .+�  $��Center=�$��0��$! KeyboardArrowLeft=�$��24��$��24��
���   ��� $��Center=�$��1��$��	上一页��$��0,5,0,0q�
���   �$��14(�� $��5,0,5,-1q�  �� �� .+��� -3   $1 TextPage��$��Center=�$��0��$	��1/1��$��0,10,0,0q�
���   �$��16(��� $��Center=�$��1��$��页码��$��0,5,0,0q�
���   �$��14(��� -4   #��[ $��60��.��� $	��0,2q�  �� �� .+�  -5   $0 PageControlNextIcon��$��Center=�$��0��$! KeyboardArrowRight=�$��24��$��24��
���   ��� -6   $1 PageControlNextText��$��Center=�$��1��$��	下一页��$��0,5,0,0q�
���   �� -7   $ logogrid�����	#00000000$y�	Collapsed=� �� $��1* ��� $��2* ��� $��1* �  �� $��1* ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��1.2* �.+��� -8   2 ��Name$2 logo1��$'��!Resources/HandyDrawPPTHelper1.png��$��1��$��1���� -9   $2 logo2��$ ��Resources/开源免费.png��$��1��$��2��z�V� .~��� �� �� T� $ 40Q�� -:   3 �Name$3 loadprog��$��6��$
��1920��$��0,1,0,0q����	#FFFF6875���	#FFFF6976���	#FFFFD29A$��0��$��3��$��3��4 � Value$4 17Q�