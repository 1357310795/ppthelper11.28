Imports System.Timers
Imports System.Windows.Ink
Imports System.Windows.Interop
Imports System.Windows.Media.Animation
Imports Microsoft.Office.Interop

Class MainWindow1
    Private Declare Function FindWindow Lib "user32" Alias "FindWindowA" (ByVal lpClassName As String, ByVal lpWindowName As String) As Integer
    Private Declare Function SetForegroundWindow Lib "user32" Alias "SetForegroundWindow" (ByVal hwnd As Int32) As Int32
    Private Declare Function GetWindowRect Lib "user32" Alias "GetWindowRect" (ByVal hwnd As IntPtr, ByRef lpRect As RECT) As Integer
    Private Declare Function GetClientRect Lib "user32" Alias "GetClientRect" (ByVal hwnd As IntPtr, ByRef lpRect As RECT) As Integer
    Private Declare Function MoveWindow Lib "user32" Alias "MoveWindow" (ByVal hwnd As IntPtr, ByVal x As Integer, ByVal y As Integer, ByVal nWidth As Integer, ByVal nHeight As Integer, ByVal bRepaint As Boolean) As Integer

    Public pen As DrawingAttributes
    Public marker As DrawingAttributes
    Public eraser As DrawingAttributes
    Public settingwindow As UserControl
    Private now_state As Now_state_enum
    Private Save_leftclicked As Boolean
    Public ppt_obj As PowerPoint.Application
    Friend ppt_rect As RECT
    Public ppt_view As PowerPoint.SlideShowView
    Public ppt_hwnd As Int32
    Public currentpage As Int32
    Private inks As StrokeCollection()
    Private animation_timer As Timer
    Dim update_timer As New Timer
    Public erroccured As Boolean

    Private Const ThreasholdNearbyDistance As Integer = 1
    Private ReadOnly _currentCanvasStrokes As Dictionary(Of Integer, Stroke)
    Private _strokeHitTester As IncrementalStrokeHitTester
    Private _addingStroke As Stroke

    Enum Now_state_enum As Integer
        Cursor = 1
        Pen = 2
        Marker = 4
        Eraser = 8
        Sel = 16
    End Enum

    Public Sub New()
        pen = New DrawingAttributes With {
            .Color = Color.FromRgb(245, 63, 54),
            .Height = 4,
            .Width = 4,
            .FitToCurve = True,
            .IsHighlighter = False,
            .StylusTip = StylusTip.Ellipse
        }
        marker = New DrawingAttributes With {
            .Color = Colors.Yellow,
            .Height = 25,
            .Width = 10,
            .FitToCurve = False,
            .IsHighlighter = True,
            .StylusTip = StylusTip.Rectangle
        }
        eraser = New DrawingAttributes With {
            .Color = Colors.White,
            .Height = 25,
            .Width = 25,
            .FitToCurve = False,
            .IsHighlighter = True,
            .StylusTip = StylusTip.Ellipse
        }

        InitializeComponent()
        InkCanvas1.EraserShape = New Ink.RectangleStylusShape(40, 60)

        _currentCanvasStrokes = New Dictionary(Of Integer, Stroke)()
        AddHandler InkCanvas1.TouchDown, AddressOf OnTouchDown
        AddHandler InkCanvas1.TouchUp, AddressOf OnTouchUp
        AddHandler InkCanvas1.TouchMove, AddressOf OnTouchMove

        _history = New Stack(Of StrokesHistoryNode)()
        _redoHistory = New Stack(Of StrokesHistoryNode)()
        ' 在 InitializeComponent() 调用之后添加任何初始化。
    End Sub
    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        'logogrid.Visibility = Visibility.Hidden
        'animation_timer = New Timer
        'animation_timer.Interval = 100
        'AddHandler animation_timer.Elapsed, AddressOf animation_timer_tick
        'animation_timer.Start()

        MoveWindow(New WindowInteropHelper(Me).Handle,
                                   ppt_rect.Left,
                                    ppt_rect.Top,
                                    ppt_rect.Right - ppt_rect.Left,
                                    ppt_rect.Bottom - ppt_rect.Top,
                                    True)

        ReDim inks(GetTotalSlideCount() + 2)
        For i = 0 To GetTotalSlideCount() + 2
            inks(i) = New StrokeCollection
        Next
        updatepage(1)
        InkCanvas1.Strokes = inks(currentpage)
        AddHandler InkCanvas1.Strokes.StrokesChanged, AddressOf StrokesChanged

        Dim PenColorBinding As Binding = New Binding
        PenColorBinding.Source = pen
        PenColorBinding.Path = New PropertyPath("Color")
        PenColorBinding.Converter = New ColorValueConverter
        PenColorTip.SetBinding(Shape.FillProperty, PenColorBinding)

        Dim MarkerColorBinding As Binding = New Binding
        MarkerColorBinding.Source = marker
        MarkerColorBinding.Path = New PropertyPath("Color")
        MarkerColorBinding.Converter = New ColorValueConverter
        MarkerColorTip.SetBinding(Shape.FillProperty, MarkerColorBinding)

        SetForegroundWindow(New WindowInteropHelper(Me).Handle)


        update_timer.Interval = 100
        AddHandler update_timer.Elapsed, AddressOf update_timer_Tick
        update_timer.Start()
    End Sub

#Region "listboxTools"
    Private Sub Cursor_Selected(sender As Object, e As RoutedEventArgs)
        InkCanvas1.EditingMode = InkCanvasEditingMode.None
        now_state = Now_state_enum.Cursor
        InkCanvas1.Background = TryCast(Application.Current.Resources("TrueTransparent"), Brush)
    End Sub
    Private Sub Pen_Selected(sender As Object, e As RoutedEventArgs)
        InkCanvas1.EditingMode = InkCanvasEditingMode.None
        InkCanvas1.DefaultDrawingAttributes = pen
        now_state = Now_state_enum.Pen
        InkCanvas1.Background = TryCast(Application.Current.Resources("FakeTransparent"), Brush)

        'Dim noti As New UserControl1
        'noti.icon.Kind = MaterialDesignThemes.Wpf.PackIconKind.LeadPencil
        'noti.label.Text = "输入工具：画笔"
        'Dim c As New Canvas
        'c.Height = Double.NaN
        'c.Width = 0
        'c.Children.Add(noti)
        'NotiStackPanel.Children.Add(c)
        'startnotianimation(c, noti)
    End Sub
    Private Sub Select_Selected(sender As Object, e As RoutedEventArgs)
        InkCanvas1.EditingMode = InkCanvasEditingMode.Select
        now_state = Now_state_enum.Sel
        InkCanvas1.Background = TryCast(Application.Current.Resources("FakeTransparent"), Brush)

    End Sub
    Private Sub Marker_Selected(sender As Object, e As RoutedEventArgs)
        InkCanvas1.EditingMode = InkCanvasEditingMode.Ink
        InkCanvas1.DefaultDrawingAttributes = marker
        now_state = Now_state_enum.Marker
        InkCanvas1.Background = TryCast(Application.Current.Resources("FakeTransparent"), Brush)
    End Sub
    Private Sub Eraser_Selected(sender As Object, e As RoutedEventArgs)
        If InkCanvas1.EditingMode <> InkCanvasEditingMode.EraseByStroke And
                InkCanvas1.EditingMode <> InkCanvasEditingMode.EraseByPoint Then
            InkCanvas1.EditingMode = InkCanvasEditingMode.EraseByPoint
        End If
        now_state = Now_state_enum.Eraser
        InkCanvas1.Background = TryCast(Application.Current.Resources("FakeTransparent"), Brush)
    End Sub
    Private Sub Setting_Selected(sender As Object, e As RoutedEventArgs)
        TryCast(sender, RadioButton).IsChecked = False
        SettingPopup.IsPopupOpen = True
        GlobalSetting.inkcanvas1 = InkCanvas1
    End Sub
    Private Sub ListBoxItem_MouseUp(sender As Object, e As MouseEventArgs)
        'ListboxClickItem.IsSelected = True
    End Sub
    Private Sub ListBoxItem_PreviewMouseUp(sender As Object, e As MouseEventArgs)
        If TryCast(sender, RadioButton).IsChecked Then
            Select Case TryCast(sender, RadioButton).Tag
                Case "Pen"
                    'settingwindow = New PenSetting(pen)
                    PenSettingPopup.IsPopupOpen = True
                    PenSetting.initdrawer(pen)
                    PenSetting.popup = PenSettingPopup
                    Exit Sub
                Case "Marker"
                    MarkerSettingPopup.IsPopupOpen = True
                    MarkerSetting.initdrawer(marker)
                    MarkerSetting.popup = MarkerSettingPopup
                Case "Eraser"
                    EraserSettingPopup.IsPopupOpen = True
                    EraserSetting.initdrawerandcanvas(InkCanvas1, eraser, Me)
                Case "Cursor"
                    Exit Sub
            End Select
        End If
    End Sub

#End Region
#Region "PPTControl"
    Private Sub ppt_next()
        ppt_view.Next()
        updatepage(1)
        SetForegroundWindow(ppt_hwnd)
    End Sub
    Private Sub ppt_prev()
        ppt_view.Previous()
        updatepage(-1)
        SetForegroundWindow(ppt_hwnd)
    End Sub
    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        ppt_prev()
    End Sub
    Private Sub Button_Click_1(sender As Object, e As RoutedEventArgs)
        ppt_next()
    End Sub
    Private Function GetTotalSlideCount() As Int32
        Return ppt_obj.ActivePresentation.Slides.Count
    End Function
    Private Sub Exit_presentation(sender As Object, e As RoutedEventArgs)
        update_timer.Stop()
        ppt_view.Exit()
    End Sub
    Private Sub Window_PreviewMouseWheel(sender As Object, e As MouseWheelEventArgs)
        If e.Delta < 0 Then
            ppt_next()
        Else
            ppt_prev()
        End If
    End Sub
    Private Sub Window_PreviewKeyDown(sender As Object, e As KeyEventArgs)
        If e.Key = Key.Left Or e.Key = Key.PageUp Or e.Key = Key.Up Then
            ppt_prev()
        ElseIf e.Key = Key.Right Or e.Key = Key.PageDown Or e.Key = Key.Down Then
            ppt_next()
        ElseIf e.Key = Key.Escape Then
            ppt_view.Exit()
        End If
    End Sub
    Private Sub updatepage(Optional isnext As Int32 = 0)
        Try
            Dim tmp, tmp1 As Int32
            tmp = ppt_view.CurrentShowPosition
            If tmp = 0 Then
                tmp1 = currentpage + isnext
            Else
                tmp1 = tmp
            End If
            If tmp1 <> currentpage Then
                currentpage = tmp1
                Me.Dispatcher.Invoke(Sub()
                                         TextPage.Text = currentpage & "/" & GetTotalSlideCount()
                                         InkCanvas1.Strokes = inks(currentpage)
                                         ClearHistory()
                                     End Sub)
            End If
        Catch ex As Exception
            Console.WriteLine(ex.Message)
            If update_timer IsNot Nothing Then
                update_timer.Stop()
            End If

            Dim t As System.Threading.Thread = New System.Threading.Thread(AddressOf err)
            t.Start()
        End Try
    End Sub
    Private Sub update_timer_Tick(sender As Object, e As EventArgs)
        updatepage()
        'Try
        'Catch ex As Exception
        '    Console.WriteLine(ex.Message)
        '    TryCast(sender, Timer).Stop()
        '    Dim t As System.Threading.Thread = New System.Threading.Thread(AddressOf err)
        '    t.Start()
        'End Try
    End Sub
    Private Sub err()
        'Me.Dispatcher.Invoke(Async Sub()
        'If MainDialogHost1.IsOpen Then
        '    Exit Sub
        'End If
        'Dim res As String
        'res = Await MaterialDesignThemes.Wpf.DialogHost.Show(New YesNoDialog(300, "程序出现内部错误，是否继续运行？"), "MainDialogHost1")
        'Console.WriteLine(res)
        'If res = "OK" Then
        '    System.Threading.Thread.Sleep(1000)
        '    update_timer.Start()
        'Else
        '    Application.Current.Shutdown()
        'End If

        'End Sub)
        If erroccured Then Exit Sub
        erroccured = True
        System.Threading.Thread.Sleep(2000)
        If update_timer IsNot Nothing Then update_timer.Start()
    End Sub
#End Region
#Region "MultiTouch"
    Private Sub StrokeHit(sender As Object, argsHitTester As StrokeHitEventArgs)
        Dim eraseResults = argsHitTester.GetPointEraseResults()
        InkCanvas1.Strokes.Remove(argsHitTester.HitStroke)
        InkCanvas1.Strokes.Add(eraseResults)
    End Sub
    Private Sub OnTouchDown(ByVal sender As Object, ByVal touchEventArgs As TouchEventArgs)
        If now_state = Now_state_enum.Sel Or now_state = Now_state_enum.Cursor Then Exit Sub
        Dim touchPoint = touchEventArgs.GetTouchPoint(Me)
        Dim point = touchPoint.Position

        If InkCanvas1.EditingMode = InkCanvasEditingMode.EraseByPoint Then

            If _strokeHitTester Is Nothing Then
                _strokeHitTester = InkCanvas1.Strokes.GetIncrementalStrokeHitTester(InkCanvas1.EraserShape)
                AddHandler _strokeHitTester.StrokeHit, AddressOf StrokeHit
            End If

            _strokeHitTester.AddPoint(point)
            Return
        End If

        _addingStroke = New Stroke(New StylusPointCollection(New List(Of Point) From {
            point
        }), InkCanvas1.DefaultDrawingAttributes.Clone)

        If Not _currentCanvasStrokes.ContainsKey(touchPoint.TouchDevice.Id) Then
            _currentCanvasStrokes.Add(touchPoint.TouchDevice.Id, _addingStroke)
            InkCanvas1.Strokes.Add(_addingStroke)
        End If
    End Sub
    Private Sub OnTouchUp(ByVal sender As Object, ByVal touchEventArgs As TouchEventArgs)
        If now_state = Now_state_enum.Sel Or now_state = Now_state_enum.Cursor Then Exit Sub
        Dim touchPoint = touchEventArgs.GetTouchPoint(Me)
        _currentCanvasStrokes.Remove(touchPoint.TouchDevice.Id)
        _strokeHitTester = Nothing
        PushToHistory()
    End Sub
    Private Sub OnTouchMove(ByVal sender As Object, ByVal touchEventArgs As TouchEventArgs)
        If now_state = Now_state_enum.Sel Or now_state = Now_state_enum.Cursor Then Exit Sub
        Dim touchPoint = touchEventArgs.GetTouchPoint(Me)
        Dim point = touchPoint.Position

        If InkCanvas1.EditingMode = InkCanvasEditingMode.EraseByPoint Then

            If _strokeHitTester IsNot Nothing Then
                _strokeHitTester.AddPoint(point)
            End If

            Return
        End If

        If _currentCanvasStrokes.ContainsKey(touchPoint.TouchDevice.Id) Then
            Dim stroke = _currentCanvasStrokes(touchPoint.TouchDevice.Id)
            Dim nearbyPoint = IsNearbyPoint(stroke, point)

            If Not nearbyPoint Then
                stroke.StylusPoints.Add(New StylusPoint(point.X, point.Y))
            End If
        End If
    End Sub
    Private Shared Function IsNearbyPoint(ByVal stroke As Stroke, ByVal point As Point) As Boolean
        Return stroke.StylusPoints.Any(Function(p) (Math.Abs(p.X - point.X) <= ThreasholdNearbyDistance) AndAlso (Math.Abs(p.Y - point.Y) <= ThreasholdNearbyDistance))
    End Function
#End Region
#Region "Animation"
    'Private Sub animation_timer_tick()
    '    animation_timer.Stop()
    '    Me.Dispatcher.Invoke(AddressOf startanimation)
    'End Sub

    'Private Sub startanimation()
    '    logogrid.Visibility = Visibility.Visible

    '    Dim doubleKeyFrame1 As DoubleKeyFrame = New LinearDoubleKeyFrame()
    '    doubleKeyFrame1.KeyTime = TimeSpan.FromSeconds(0.0)
    '    doubleKeyFrame1.Value = 40
    '    Dim splineDoubleKeyFrame As SplineDoubleKeyFrame = New SplineDoubleKeyFrame()
    '    splineDoubleKeyFrame.KeyTime = TimeSpan.FromSeconds(1.2)
    '    Dim controlPoint As Point = New Point(0, 0.25) 'cubic-bezier(0,.25,.36,1)
    '    Dim controlPoint2 As Point = New Point(0.36, 1)
    '    splineDoubleKeyFrame.KeySpline = New KeySpline() With {.ControlPoint1 = controlPoint, .ControlPoint2 = controlPoint2}
    '    splineDoubleKeyFrame.Value = 0
    '    Dim doubleKeyFramea As DoubleKeyFrame = New LinearDoubleKeyFrame()
    '    doubleKeyFramea.KeyTime = TimeSpan.FromSeconds(1.7)
    '    doubleKeyFramea.Value = 0
    '    Dim doubleKeyFrame2 As DoubleKeyFrame = New LinearDoubleKeyFrame()
    '    doubleKeyFrame2.KeyTime = TimeSpan.FromSeconds(2.2)
    '    doubleKeyFrame2.Value = 40
    '    Dim logo2animation2 = New DoubleAnimationUsingKeyFrames
    '    logo2animation2.KeyFrames.Add(doubleKeyFrame1)
    '    logo2animation2.KeyFrames.Add(splineDoubleKeyFrame)
    '    logo2animation2.KeyFrames.Add(doubleKeyFramea)
    '    logo2animation2.KeyFrames.Add(doubleKeyFrame2)
    '    Dim x As New TranslateTransform
    '    logo2.RenderTransform = x


    '    Dim doubleKeyFrame3 As DoubleKeyFrame = New LinearDoubleKeyFrame()
    '    doubleKeyFrame3.KeyTime = TimeSpan.FromSeconds(0.0)
    '    doubleKeyFrame3.Value = 0
    '    Dim splineDoubleKeyFrame2 As SplineDoubleKeyFrame = New SplineDoubleKeyFrame()
    '    splineDoubleKeyFrame2.KeyTime = TimeSpan.FromSeconds(1.2)
    '    Dim controlPoint3 As Point = New Point(0, 0.25)
    '    Dim controlPoint4 As Point = New Point(0.36, 1)
    '    splineDoubleKeyFrame2.KeySpline = New KeySpline() With {.ControlPoint1 = controlPoint3, .ControlPoint2 = controlPoint4}
    '    splineDoubleKeyFrame2.Value = 1
    '    Dim doubleKeyFrameb As DoubleKeyFrame = New LinearDoubleKeyFrame()
    '    doubleKeyFrameb.KeyTime = TimeSpan.FromSeconds(1.7)
    '    doubleKeyFrameb.Value = 1
    '    Dim doubleKeyFrame4 As DoubleKeyFrame = New LinearDoubleKeyFrame()
    '    doubleKeyFrame4.KeyTime = TimeSpan.FromSeconds(2.0)
    '    doubleKeyFrame4.Value = 0
    '    Dim logo2animation As New DoubleAnimationUsingKeyFrames
    '    logo2animation.KeyFrames.Add(doubleKeyFrame3)
    '    logo2animation.KeyFrames.Add(splineDoubleKeyFrame2)
    '    logo2animation.KeyFrames.Add(doubleKeyFrameb)
    '    logo2animation.KeyFrames.Add(doubleKeyFrame4)


    '    Dim doubleKeyFrame5 As DoubleKeyFrame = New LinearDoubleKeyFrame()
    '    doubleKeyFrame5.KeyTime = TimeSpan.FromSeconds(0.0)
    '    doubleKeyFrame5.Value = 0
    '    Dim splineDoubleKeyFrame3 As SplineDoubleKeyFrame = New SplineDoubleKeyFrame()
    '    splineDoubleKeyFrame3.KeyTime = TimeSpan.FromSeconds(1.2)
    '    Dim controlPoint5 As Point = New Point(0, 0.48) 'cubic-bezier(0,.48,.8,.99)
    '    Dim controlPoint6 As Point = New Point(0.8, 0.99)
    '    splineDoubleKeyFrame3.KeySpline = New KeySpline() With {.ControlPoint1 = controlPoint5, .ControlPoint2 = controlPoint6}
    '    splineDoubleKeyFrame3.Value = 1
    '    Dim doubleKeyFramec As DoubleKeyFrame = New LinearDoubleKeyFrame()
    '    doubleKeyFramec.KeyTime = TimeSpan.FromSeconds(1.7)
    '    doubleKeyFramec.Value = 1
    '    Dim doubleKeyFrame6 As DoubleKeyFrame = New LinearDoubleKeyFrame()
    '    doubleKeyFrame6.KeyTime = TimeSpan.FromSeconds(2.0)
    '    doubleKeyFrame6.Value = 0
    '    Dim logo1animation As New DoubleAnimationUsingKeyFrames
    '    logo1animation.KeyFrames.Add(doubleKeyFrame5)
    '    logo1animation.KeyFrames.Add(splineDoubleKeyFrame3)
    '    logo1animation.KeyFrames.Add(doubleKeyFramec)
    '    logo1animation.KeyFrames.Add(doubleKeyFrame6)

    '    Dim ColorKeyFrame7 As ColorKeyFrame = New LinearColorKeyFrame()
    '    ColorKeyFrame7.KeyTime = TimeSpan.FromSeconds(0.0)
    '    ColorKeyFrame7.Value = Color.FromArgb(0, 0, 0, 0)
    '    Dim splineColorKeyFrame4 As SplineColorKeyFrame = New SplineColorKeyFrame()
    '    splineColorKeyFrame4.KeyTime = TimeSpan.FromSeconds(1)
    '    Dim controlPoint7 As Point = New Point(0, 0.48) 'cubic-bezier(0,.48,.8,.99)
    '    Dim controlPoint8 As Point = New Point(0.8, 0.99)
    '    splineColorKeyFrame4.KeySpline = New KeySpline() With {.ControlPoint1 = controlPoint7, .ControlPoint2 = controlPoint8}
    '    splineColorKeyFrame4.Value = Color.FromArgb(150, 0, 0, 0)
    '    Dim ColorKeyFramed As ColorKeyFrame = New LinearColorKeyFrame()
    '    ColorKeyFramed.KeyTime = TimeSpan.FromSeconds(1.5)
    '    ColorKeyFramed.Value = Color.FromArgb(150, 0, 0, 0)
    '    Dim ColorKeyFrame8 As ColorKeyFrame = New LinearColorKeyFrame()
    '    ColorKeyFrame8.KeyTime = TimeSpan.FromSeconds(2.0)
    '    ColorKeyFrame8.Value = Color.FromArgb(0, 0, 0, 0)
    '    Dim backanimation As New ColorAnimationUsingKeyFrames
    '    backanimation.KeyFrames.Add(ColorKeyFrame7)
    '    backanimation.KeyFrames.Add(splineColorKeyFrame4)
    '    backanimation.KeyFrames.Add(ColorKeyFramed)
    '    backanimation.KeyFrames.Add(ColorKeyFrame8)

    '    Dim doubleKeyFrame7 As DoubleKeyFrame = New LinearDoubleKeyFrame()
    '    doubleKeyFrame7.KeyTime = TimeSpan.FromSeconds(1.2)
    '    doubleKeyFrame7.Value = 1
    '    Dim doubleKeyFramee As DoubleKeyFrame = New LinearDoubleKeyFrame()
    '    doubleKeyFramee.KeyTime = TimeSpan.FromSeconds(1.7)
    '    doubleKeyFramee.Value = 1
    '    Dim doubleKeyFrame8 As DoubleKeyFrame = New LinearDoubleKeyFrame()
    '    doubleKeyFrame8.KeyTime = TimeSpan.FromSeconds(2.0)
    '    doubleKeyFrame8.Value = 0
    '    Dim progressanimation As New DoubleAnimationUsingKeyFrames
    '    progressanimation.KeyFrames.Add(doubleKeyFrame7)
    '    progressanimation.KeyFrames.Add(doubleKeyFramee)
    '    progressanimation.KeyFrames.Add(doubleKeyFrame8)

    '    Dim doubleKeyFrame11 As DoubleKeyFrame = New LinearDoubleKeyFrame()
    '    doubleKeyFrame11.KeyTime = TimeSpan.FromSeconds(0)
    '    doubleKeyFrame11.Value = 0
    '    Dim doubleKeyFrame12 As DoubleKeyFrame = New LinearDoubleKeyFrame()
    '    doubleKeyFrame12.KeyTime = TimeSpan.FromSeconds(2.0)
    '    doubleKeyFrame12.Value = 100
    '    Dim proganimation As New DoubleAnimationUsingKeyFrames
    '    proganimation.KeyFrames.Add(doubleKeyFrame11)
    '    proganimation.KeyFrames.Add(doubleKeyFrame12)

    '    AddHandler progressanimation.Completed, AddressOf animationend
    '    x.BeginAnimation(TranslateTransform.YProperty, logo2animation2)
    '    logo2.BeginAnimation(UIElement.OpacityProperty, logo2animation)
    '    logo1.BeginAnimation(UIElement.OpacityProperty, logo1animation)
    '    logogrid.Background.BeginAnimation(SolidColorBrush.ColorProperty, backanimation)
    '    loadprog.BeginAnimation(UIElement.OpacityProperty, progressanimation)
    '    loadprog.BeginAnimation(ProgressBar.ValueProperty, proganimation)
    'End Sub

    'Private Sub animationend()
    '    MainGrid.Children.Remove(logogrid)
    '    GC.Collect()
    'End Sub
#End Region
#Region "History"
    Private ReadOnly _history As Stack(Of StrokesHistoryNode)
    Private ReadOnly _redoHistory As Stack(Of StrokesHistoryNode)
    Private _ignoreStrokesChange As Boolean
    Private strokeadded, strokeremoved As New StrokeCollection

    Private Sub Undo()
        If strokeadded.Count <> 0 Or strokeremoved.Count <> 0 Then PushToHistory()
        If Not CanUndo() Then Return

        Dim last = Pop(_history)
        _ignoreStrokesChange = True

        If last.Type = StrokesHistoryNodeType.Added Then
            InkCanvas1.Strokes.Remove(last.Strokes)
        Else
            InkCanvas1.Strokes.Add(last.Strokes)
        End If

        _ignoreStrokesChange = False
        Push(_redoHistory, last)
    End Sub

    Private Sub Redo()
        If Not CanRedo() Then Return
        Dim last = Pop(_redoHistory)
        _ignoreStrokesChange = True

        If last.Type = StrokesHistoryNodeType.Removed Then
            InkCanvas1.Strokes.Remove(last.Strokes)
        Else
            InkCanvas1.Strokes.Add(last.Strokes)
        End If

        _ignoreStrokesChange = False
        Push(_history, last)
    End Sub

    Private Shared Sub Push(ByVal collection As Stack(Of StrokesHistoryNode), ByVal node As StrokesHistoryNode)
        collection.Push(node)
    End Sub

    Private Shared Function Pop(ByVal collection As Stack(Of StrokesHistoryNode)) As StrokesHistoryNode
        Return If(collection.Count = 0, Nothing, collection.Pop())
    End Function

    Private Function CanUndo() As Boolean
        Return _history.Count <> 0
    End Function

    Private Function CanRedo() As Boolean
        Return _redoHistory.Count <> 0
    End Function

    Private Sub StrokesChanged(ByVal sender As Object, ByVal e As StrokeCollectionChangedEventArgs)
        If _ignoreStrokesChange Then Exit Sub
        For Each i In e.Added
            strokeadded.Add(i)
        Next
        For Each i In e.Removed
            strokeremoved.Add(i)
        Next
        'strokeadded = TryCast(strokeadded.Concat(e.Added), StrokeCollection)
        'strokeremoved = TryCast(strokeremoved.Concat(e.Removed), StrokeCollection)
    End Sub

    Private Sub PushToHistory()
        If strokeadded.Count <> 0 Then
            Push(_history, New StrokesHistoryNode(strokeadded, StrokesHistoryNodeType.Added))
        End If
        If strokeremoved.Count <> 0 Then
            Push(_history, New StrokesHistoryNode(strokeremoved, StrokesHistoryNodeType.Removed))
        End If

        strokeadded = New StrokeCollection
        strokeremoved = New StrokeCollection
        ClearHistory(_redoHistory)
    End Sub

    Private Sub ClearHistory()
        ClearHistory(_history)
        ClearHistory(_redoHistory)
    End Sub

    Private Shared Sub ClearHistory(ByVal collection As Stack(Of StrokesHistoryNode))
        collection?.Clear()
    End Sub

    Public Sub Clear()
        InkCanvas1.Strokes.Clear()
        ClearHistory()
        FlushMemory.Flush()
    End Sub

    Private Sub AnimatedClear()
        Dim ani = New DoubleAnimation(0, New Duration(New TimeSpan(0, 0, 0, 0, 3)))
        AddHandler ani.Completed, AddressOf ClearAniComplete
        InkCanvas1.BeginAnimation(OpacityProperty, ani)
    End Sub

    Private Sub ClearAniComplete(ByVal sender As Object, ByVal e As EventArgs)
        Clear()
        InkCanvas1.BeginAnimation(OpacityProperty, New DoubleAnimation(1, New Duration(New TimeSpan(0, 0, 0, 0, 3))))
    End Sub

    Private Sub Redo_Selected(sender As Object, e As RoutedEventArgs)
        TryCast(sender, RadioButton).IsChecked = False
        Redo()
    End Sub

    Private Sub Undo_Selected(sender As Object, e As RoutedEventArgs)
        TryCast(sender, RadioButton).IsChecked = False
        Undo()
    End Sub
#End Region

    Private Class ColorValueConverter
        Implements IValueConverter
        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
            Dim c As Color = CType(value, Color)
            Dim b As SolidColorBrush = New SolidColorBrush(c)
            Return b
        End Function
        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
            Throw New NotImplementedException
        End Function
    End Class

    Private Async Sub Info_Click(sender As Object, e As RoutedEventArgs)
        Dim res As String
        Dim s As AboutDialog = New AboutDialog
        res = Await MaterialDesignThemes.Wpf.DialogHost.Show(s, "MainDialogHost1")
    End Sub
    'Private Sub startnotianimation(c As Canvas, n As UserControl1)
    '    Dim doubleKeyFrame1 As DoubleKeyFrame = New LinearDoubleKeyFrame()
    '    doubleKeyFrame1.KeyTime = TimeSpan.FromSeconds(0.0)
    '    doubleKeyFrame1.Value = 0
    '    Dim splineDoubleKeyFrame As SplineDoubleKeyFrame = New SplineDoubleKeyFrame()
    '    splineDoubleKeyFrame.KeyTime = TimeSpan.FromSeconds(0.5)
    '    Dim controlPoint As Point = New Point(0.3, 1.01) 'cubic-bezier(.3,1.01,.64,1.19)
    '    Dim controlPoint2 As Point = New Point(0.64, 1.19)
    '    splineDoubleKeyFrame.KeySpline = New KeySpline() With {.ControlPoint1 = controlPoint, .ControlPoint2 = controlPoint2}
    '    splineDoubleKeyFrame.Value = n.Width
    '    Console.WriteLine(n.Width)
    '    Dim doubleKeyFramea As DoubleKeyFrame = New LinearDoubleKeyFrame()
    '    doubleKeyFramea.KeyTime = TimeSpan.FromSeconds(2.5)
    '    doubleKeyFramea.Value = n.Width
    '    Dim doubleKeyFrame2 As DoubleKeyFrame = New LinearDoubleKeyFrame()
    '    doubleKeyFrame2.KeyTime = TimeSpan.FromSeconds(3)
    '    doubleKeyFrame2.Value = 0
    '    Dim a = New DoubleAnimationUsingKeyFrames
    '    a.KeyFrames.Add(doubleKeyFrame1)
    '    a.KeyFrames.Add(splineDoubleKeyFrame)
    '    a.KeyFrames.Add(doubleKeyFramea)
    '    a.KeyFrames.Add(doubleKeyFrame2)

    '    AddHandler a.Completed, AddressOf notianimationend
    '    c.BeginAnimation(Canvas.WidthProperty, a)
    'End Sub

    'Private Sub notianimationend(sender As Object, e As EventArgs)
    '    Dim a As DoubleAnimationUsingKeyFrames = TryCast(sender, DoubleAnimationUsingKeyFrames)
    '    a = Nothing
    '    NotiStackPanel.Children.Clear()
    'End Sub
    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        update_timer.Stop()
        update_timer = Nothing
    End Sub
End Class

