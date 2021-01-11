Imports System.Windows.Ink


Public Enum StrokesHistoryNodeType
        Removed
        Added
    End Enum

Friend Class StrokesHistoryNode
    Public Property Strokes As StrokeCollection
    Public Property Type As StrokesHistoryNodeType

    Public Sub New(strokes1 As StrokeCollection, type1 As StrokesHistoryNodeType)
        Strokes = strokes1
        Type = type1
    End Sub
End Class

