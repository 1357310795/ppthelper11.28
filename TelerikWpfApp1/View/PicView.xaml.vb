Imports System.Windows.Ink
Imports System.Windows.Media.Animation
Imports System.Math

Public Class PicView
    Inherits UserControl
    Public Edit_Mode As Edit_Mode_Enum
    Public scale = 1

    Public Sub New()
        ' 此调用是设计器所必需的。
        InitializeComponent()
        InkCanvas1.EraserShape = New Ink.RectangleStylusShape(30, 50)
        ' 在 InitializeComponent() 调用之后添加任何初始化。
        _currentCanvasStrokes = New Dictionary(Of Integer, Stroke)()
        _lasttimestamp = New Dictionary(Of Integer, Double)
        _lastpoint = New Dictionary(Of Integer, StylusPoint)
        AddHandler InkCanvas1.TouchDown, AddressOf OnTouchDown
        AddHandler InkCanvas1.TouchUp, AddressOf OnTouchUp
        AddHandler InkCanvas1.TouchMove, AddressOf OnTouchMove
        AddHandler InkCanvas1.PreviewMouseDown, AddressOf OnMouseDown
        AddHandler InkCanvas1.MouseUp, AddressOf OnMouseUp
        AddHandler InkCanvas1.MouseLeave, AddressOf OnMouseUp
    End Sub

    Private Sub InkCanvas1_SizeChanged(sender As Object, e As SizeChangedEventArgs) Handles InkCanvas1.SizeChanged
        If e.NewSize.Width <> BackImage.ActualWidth Then
            InkCanvas1.Width = BackImage.ActualWidth
        End If
        If e.NewSize.Height <> BackImage.ActualHeight Then
            InkCanvas1.Height = BackImage.ActualHeight
        End If
    End Sub
#Region "FreeScale"
    Private TotalScale = 1, TotalRotation, TotalX, TotalY As Double

    Private Sub Canvas1_ManipulationStarting(ByVal sender As Object, ByVal e As ManipulationStartingEventArgs)
        e.ManipulationContainer = Canvas1
    End Sub

    Private Sub Canvas1_ManipulationStarted(ByVal sender As Object, ByVal e As ManipulationStartedEventArgs)
    End Sub

    Private Sub Canvas1_ManipulationDelta(ByVal sender As Object, ByVal e As ManipulationDeltaEventArgs)
        If Edit_Mode <> Edit_Mode_Enum.Cursor Then Exit Sub
        Dim scale = e.DeltaManipulation.Scale
        Dim expansion = e.DeltaManipulation.Expansion
        Dim rotation = e.DeltaManipulation.Rotation
        Dim translation = e.DeltaManipulation.Translation
        Dim cumulativeScale = e.CumulativeManipulation.Scale
        Dim cumulativeExpansion = e.CumulativeManipulation.Expansion
        Dim cumulativeRotation = e.CumulativeManipulation.Rotation
        Dim cumulativeRranslation = e.CumulativeManipulation.Translation

        TotalScale *= scale.X
        TotalRotation += rotation
        TotalX += translation.X
        TotalY += translation.Y

        Do While TotalRotation < 0
            TotalRotation += 360
        Loop
        Do While TotalRotation >= 360
            TotalRotation -= 360
        Loop

        Dim h As Double = Grid1.ActualHeight
        Dim w As Double = Grid1.ActualWidth
        Dim t As Double = TotalRotation / 180 * PI
        Dim w1 As Double = Abs(h * Sin(t)) + Abs(w * Cos(t)) * TotalScale
        Dim h1 As Double = Abs(h * Cos(t)) + Abs(w * Sin(t)) * TotalScale

        ScaleTransform.ScaleX *= scale.X
        ScaleTransform.ScaleY *= scale.Y
        RotateTransform.Angle += rotation
        Canvas.SetLeft(Grid1, TotalX + w / 2 - w1 / 2)
        Canvas.SetTop(Grid1, TotalY + h / 2 - h1 / 2)
        't1.Text = TotalX.ToString + "," + TotalY.ToString
        't2.Text = x.ToString + "," + y.ToString
    End Sub

    Private Sub Canvas1_ManipulationCompleted(ByVal sender As Object, ByVal e As ManipulationCompletedEventArgs)

    End Sub

    Private Function max4(ByVal x1 As Double, ByVal x2 As Double, ByVal x3 As Double, ByVal x4 As Double)
        Return Math.Max(Math.Max(x1, x2), Math.Max(x3, x4))
    End Function

    Private Function min4(ByVal x1 As Double, ByVal x2 As Double, ByVal x3 As Double, ByVal x4 As Double)
        Return Math.Min(Math.Min(x1, x2), Math.Min(x3, x4))
    End Function
#End Region
#Region "MultiTouch"
    Private Const ThreasholdNearbyDistance As Double = 0.01
    Private ReadOnly _currentCanvasStrokes As Dictionary(Of Integer, Stroke)
    Private ReadOnly _lasttimestamp As Dictionary(Of Integer, Double)
    Private ReadOnly _lastpoint As Dictionary(Of Integer, StylusPoint)
    Private _strokeHitTester As IncrementalStrokeHitTester
    Private _addingStroke As Stroke
    Private maxv As Double = 200
    Private Sub StrokeHit(sender As Object, argsHitTester As StrokeHitEventArgs)
        Dim eraseResults = argsHitTester.GetPointEraseResults()
        InkCanvas1.Strokes.Remove(argsHitTester.HitStroke)
        InkCanvas1.Strokes.Add(eraseResults)
    End Sub
#Disable Warning BC40005 ' 成员隐藏基类型中的可重写的方法
    Private Sub OnTouchDown(ByVal sender As Object, ByVal touchEventArgs As TouchEventArgs)
        Console.WriteLine("OnTouchDown")
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

        If Edit_Mode = Edit_Mode_Enum.Pen Then
            _addingStroke = New Stroke(New StylusPointCollection(New List(Of StylusPoint) From {
                New StylusPoint(point.X, point.Y, 0.5)
            }), InkCanvas1.DefaultDrawingAttributes.Clone)

            If Not _currentCanvasStrokes.ContainsKey(touchPoint.TouchDevice.Id) Then
                _currentCanvasStrokes.Add(touchPoint.TouchDevice.Id, _addingStroke)
                InkCanvas1.Strokes.Add(_addingStroke)
                _lasttimestamp.Add(touchPoint.TouchDevice.Id, DateTime.Now.Ticks / 1000000D)
                _lastpoint.Add(touchPoint.TouchDevice.Id, _addingStroke.StylusPoints(0))
            End If
        End If
    End Sub

    Private Sub OnTouchUp(ByVal sender As Object, ByVal touchEventArgs As TouchEventArgs)
        Console.WriteLine("OnTouchUp")

        If InkCanvas1.EditingMode = InkCanvasEditingMode.EraseByPoint Then
            _strokeHitTester = Nothing
            Return
        End If

        If Edit_Mode = Edit_Mode_Enum.Pen Then
            Dim touchPoint = touchEventArgs.GetTouchPoint(Me)
            Dim spc As StylusPointCollection = _currentCanvasStrokes(touchPoint.TouchDevice.Id).StylusPoints
            Console.WriteLine(spc.Count)
            If (spc.Count > 5) Then
                If (spc(spc.Count - 2).PressureFactor < 0.8) Then
                    For i = 1 To 1 Step -1
                        Dim t As StylusPoint = spc(spc.Count - i)
                        t.PressureFactor = 0.1F + (spc(spc.Count - 2).PressureFactor - 0.1F) * (i - 1) / 2
                        spc(spc.Count - i) = t
                    Next
                End If
            End If
            _currentCanvasStrokes.Remove(touchPoint.TouchDevice.Id)
            _lasttimestamp.Remove(touchPoint.TouchDevice.Id)
            _lastpoint.Remove(touchPoint.TouchDevice.Id)
        End If
    End Sub

    Private Sub OnTouchMove(ByVal sender As Object, ByVal touchEventArgs As TouchEventArgs)
        'Console.WriteLine("OnTouchMove")
        Dim touchPoint = touchEventArgs.GetTouchPoint(Me)
        Dim point = touchPoint.Position

        If InkCanvas1.EditingMode = InkCanvasEditingMode.EraseByPoint Then
            If _strokeHitTester IsNot Nothing Then
                _strokeHitTester.AddPoint(point)
            End If
            Return
        End If

        If Edit_Mode = Edit_Mode_Enum.Pen Then
            If _currentCanvasStrokes.ContainsKey(touchPoint.TouchDevice.Id) Then
                Dim stroke = _currentCanvasStrokes(touchPoint.TouchDevice.Id)
                Dim nearbyPoint = IsNearbyPoint(stroke, point)

                If Not nearbyPoint Then
                    Dim sp As StylusPoint = New StylusPoint(point.X, point.Y)
                    Dim nowtime As Double = DateTime.Now.Ticks / 1000000D
                    Dim v = (point - _lastpoint(touchPoint.TouchDevice.Id).ToPoint).Length / (nowtime - _lasttimestamp(touchPoint.TouchDevice.Id))
                    If (Double.IsNaN(v) Or v > maxv) Then
                        sp.PressureFactor = 0.2F
                    Else
                        sp.PressureFactor = CType((0.8F - (0.6F / maxv) * v), Single)
                    End If
                    stroke.StylusPoints.Add(sp)
                    _lastpoint(touchPoint.TouchDevice.Id) = sp
                    _lasttimestamp(touchPoint.TouchDevice.Id) = nowtime
                    Application.Current.Resources("speed") = v.ToString()
                    If (Not Double.IsNaN(v) And Not Double.IsPositiveInfinity(v)) Then
                        Application.Current.Resources("speedint") = v
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub OnMouseDown(ByVal sender As Object, ByVal e As MouseEventArgs)
        Console.WriteLine("OnMouseDown")
        If e.StylusDevice IsNot Nothing Then Return
        Dim point = e.GetPosition(InkCanvas1)
        If Edit_Mode = Edit_Mode_Enum.Pen Then
            InkCanvas1.EditingMode = InkCanvasEditingMode.Ink
            InkCanvas1.CaptureMouse()
        End If
    End Sub

    Private Sub OnMouseUp(ByVal sender As Object, ByVal e As MouseEventArgs)
        Console.WriteLine("OnMouseUp")
        CompareStrokes()
        PushToHistory()
        If e.StylusDevice IsNot Nothing Then Return
        If Edit_Mode = Edit_Mode_Enum.Pen Then
            InkCanvas1.EditingMode = InkCanvasEditingMode.None
            InkCanvas1.ReleaseMouseCapture()
        End If
    End Sub


    Private Shared Function IsNearbyPoint(ByVal stroke As Stroke, ByVal point As Point) As Boolean
        Return stroke.StylusPoints.Any(Function(p) (Math.Abs(p.X - point.X) <= ThreasholdNearbyDistance) AndAlso (Math.Abs(p.Y - point.Y) <= ThreasholdNearbyDistance))
    End Function
#End Region
#Region "History"
    Private ReadOnly _history As New Stack(Of StrokesHistoryNode)
    Private ReadOnly _redoHistory As New Stack(Of StrokesHistoryNode)
    Private _ignoreStrokesChange As Boolean
    Private strokeadded, strokeremoved As New StrokeCollection
    Private PreStrokes As New StrokeCollection

    Public Sub Undo()
        CompareStrokes()
        If strokeadded.Count <> 0 Or strokeremoved.Count <> 0 Then
            PushToHistory()
        End If
        If Not CanUndo() Then Return

        Dim last = Pop(_history)
        _ignoreStrokesChange = True

        InkCanvas1.Strokes.Add(last.StrokesRemoved)
        InkCanvas1.Strokes.Remove(last.StrokesAdded)
        PreStrokes.Add(last.StrokesRemoved)
        PreStrokes.Remove(last.StrokesAdded)

        _ignoreStrokesChange = False
        Push(_redoHistory, last)
    End Sub

    Public Sub Redo()
        CompareStrokes()
        If strokeadded.Count <> 0 Or strokeremoved.Count <> 0 Then
            PushToHistory()
            Return
        End If
        If Not CanRedo() Then Return
        Dim last = Pop(_redoHistory)
        _ignoreStrokesChange = True

        InkCanvas1.Strokes.Remove(last.StrokesRemoved)
        InkCanvas1.Strokes.Add(last.StrokesAdded)
        PreStrokes.Add(last.StrokesAdded)
        PreStrokes.Remove(last.StrokesRemoved)

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

    'Private Sub StrokesChanged(ByVal sender As Object, ByVal e As StrokeCollectionChangedEventArgs)
    '    If _ignoreStrokesChange Then Exit Sub
    '    For Each i In e.Added
    '        strokeadded.Add(i)
    '    Next
    '    For Each i In e.Removed
    '        strokeremoved.Add(i)
    '    Next
    '    'strokeadded = TryCast(strokeadded.Concat(e.Added), StrokeCollection)
    '    'strokeremoved = TryCast(strokeremoved.Concat(e.Removed), StrokeCollection)
    'End Sub
    Public Sub CompareStrokes()
        Dim t As New StrokeCollection
        For Each s In PreStrokes
            If Not InkCanvas1.Strokes.Contains(s) Then
                strokeremoved.Add(s)
                t.Add(s)
            End If
        Next
        For Each s In t
            PreStrokes.Remove(s)
        Next
        For Each s In InkCanvas1.Strokes
            If Not PreStrokes.Contains(s) Then
                strokeadded.Add(s)
                PreStrokes.Add(s)
            End If
        Next
    End Sub

    Private Sub PushToHistory()
        If strokeadded.Count = 0 And strokeremoved.Count = 0 Then Return
        Dim t As New StrokesHistoryNode()
        t.StrokesAdded = strokeadded
        t.StrokesRemoved = strokeremoved
        Push(_history, t)

        strokeadded = New StrokeCollection()
        strokeremoved = New StrokeCollection()
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
#End Region

End Class
