Imports AnyCAD.Platform
Imports AnyCAD.Presentation


Public Class Form1
    Dim renderView As AnyCAD.Presentation.RenderWindow3d
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        renderView = New AnyCAD.Presentation.RenderWindow3d
        renderView.Location = New System.Drawing.Point(0, 0)
        renderView.Size = SplitContainer1.Panel2.Size
        renderView.TabIndex = 1
        SplitContainer1.Panel2.Controls.Add(renderView)

        'GlobalInstance.EventListener.OnSelectElementEvent += OnSelectionChanged;
        'AddHandler AnyCAD.Platform.EventListener.SelectElementEvent, AddressOf OnSelectionChanged

        renderView.ExecuteCommand("ShadeWithEdgeMode")
        renderView.ShowCoordinateAxis(True)
        renderView.SetPickMode(CType(AnyCAD.Platform.EnumPickMode.RF_Face, Integer))
        renderView.RequestDraw()
    End Sub

    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        If renderView IsNot Nothing Then
            renderView.Size = SplitContainer1.Panel2.Size
        End If
    End Sub
End Class
