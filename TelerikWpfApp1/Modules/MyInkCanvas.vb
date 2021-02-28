Imports System.Windows.Ink
Imports System.Windows.Media.Animation

Public Class MyInkCanvas
    Inherits InkCanvas

    Public Edit_Mode As Edit_Mode_Enum
    Public Sub New()
        Me.EraserShape = New Ink.RectangleStylusShape(40, 60)
        _currentCanvasStrokes = New Dictionary(Of Integer, Stroke)()
        '_lasttimestamp = New Dictionary(Of Integer, Double)
        '_lastpoint = New Dictionary(Of Integer, StylusPoint)
        AddHandler Me.TouchDown, AddressOf OnTouchDown
        AddHandler Me.TouchUp, AddressOf OnTouchUp
        AddHandler Me.TouchMove, AddressOf OnTouchMove
        AddHandler Me.PreviewMouseDown, AddressOf OnMouseDown
        AddHandler Me.MouseUp, AddressOf OnMouseUp
        AddHandler Me.MouseLeave, AddressOf OnMouseUp
    End Sub

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
        Me.Strokes.Remove(argsHitTester.HitStroke)
        Me.Strokes.Add(eraseResults)
    End Sub
#Disable Warning BC40005 ' 成员隐藏基类型中的可重写的方法
    Private Sub OnTouchDown(ByVal sender As Object, ByVal touchEventArgs As TouchEventArgs)
        Console.WriteLine("OnTouchDown")
        Dim touchPoint = touchEventArgs.GetTouchPoint(Me)
        Dim point = touchPoint.Position

        If Me.EditingMode = InkCanvasEditingMode.EraseByPoint Then
            If _strokeHitTester Is Nothing Then
                _strokeHitTester = Me.Strokes.GetIncrementalStrokeHitTester(Me.EraserShape)
                AddHandler _strokeHitTester.StrokeHit, AddressOf StrokeHit
            End If
            _strokeHitTester.AddPoint(point)
            Return
        End If

        If Edit_Mode = Edit_Mode_Enum.Pen Then
            _addingStroke = New Stroke(New StylusPointCollection(New List(Of StylusPoint) From {
                New StylusPoint(point.X, point.Y, 0.5)
            }), Me.DefaultDrawingAttributes.Clone)

            If Not _currentCanvasStrokes.ContainsKey(touchPoint.TouchDevice.Id) Then
                _currentCanvasStrokes.Add(touchPoint.TouchDevice.Id, _addingStroke)
                Me.Strokes.Add(_addingStroke)
                _lasttimestamp.Add(touchPoint.TouchDevice.Id, DateTime.Now.Ticks / 1000000D)
                _lastpoint.Add(touchPoint.TouchDevice.Id, _addingStroke.StylusPoints(0))
            End If
        End If
    End Sub

    Private Sub OnTouchUp(ByVal sender As Object, ByVal touchEventArgs As TouchEventArgs)
        Console.WriteLine("OnTouchUp")

        If Me.EditingMode = InkCanvasEditingMode.EraseByPoint Then
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

        If Me.EditingMode = InkCanvasEditingMode.EraseByPoint Then
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
        Dim point = e.GetPosition(Me)
        If Edit_Mode = Edit_Mode_Enum.Pen Then
            Me.EditingMode = InkCanvasEditingMode.Ink
            Me.CaptureMouse()
        End If
    End Sub

    Private Sub OnMouseUp(ByVal sender As Object, ByVal e As MouseEventArgs)
        Console.WriteLine("OnMouseUp")
        CompareStrokes()
        PushToHistory()
        If e.StylusDevice IsNot Nothing Then Return
        If Edit_Mode = Edit_Mode_Enum.Pen Then
            Me.EditingMode = InkCanvasEditingMode.None
            Me.ReleaseMouseCapture()
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

        Me.Strokes.Add(last.StrokesRemoved)
        Me.Strokes.Remove(last.StrokesAdded)
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

        Me.Strokes.Remove(last.StrokesRemoved)
        Me.Strokes.Add(last.StrokesAdded)
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
            If Not Me.Strokes.Contains(s) Then
                strokeremoved.Add(s)
                t.Add(s)
            End If
        Next
        For Each s In t
            PreStrokes.Remove(s)
        Next
        For Each s In Me.Strokes
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

    Public Sub ClearHistory()
        ClearHistory(_history)
        ClearHistory(_redoHistory)
    End Sub

    Private Shared Sub ClearHistory(ByVal collection As Stack(Of StrokesHistoryNode))
        collection?.Clear()
    End Sub

    Public Sub Clear()
        Me.Strokes.Clear()
        ClearHistory()
        FlushMemory.Flush()
    End Sub

    Private Sub AnimatedClear()
        Dim ani = New DoubleAnimation(0, New Duration(New TimeSpan(0, 0, 0, 0, 3)))
        AddHandler ani.Completed, AddressOf ClearAniComplete
        Me.BeginAnimation(OpacityProperty, ani)
    End Sub

    Private Sub ClearAniComplete(ByVal sender As Object, ByVal e As EventArgs)
        Clear()
        Me.BeginAnimation(OpacityProperty, New DoubleAnimation(1, New Duration(New TimeSpan(0, 0, 0, 0, 3))))
    End Sub
#End Region
End Class
