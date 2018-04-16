Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Windows.Interactivity
Imports System.Windows
Imports DevExpress.Xpf.Grid
Imports System.Windows.Controls
Imports System.Windows.Input
Imports DevExpress.Xpf.Core
Imports DevExpress.Xpf.Core.ServerMode
Imports DevExpress.Xpf.Bars
Imports System.Data.Objects

Namespace EntityServer
	Public Class EntityServerModeCRUDBehavior
		Inherits Behavior(Of GridControl)
		Public Shared ReadOnly NewRowFormProperty As DependencyProperty = DependencyProperty.Register("NewRowForm", GetType(DataTemplate), GetType(EntityServerModeCRUDBehavior), New PropertyMetadata(Nothing))
		Public Shared ReadOnly EditRowFormProperty As DependencyProperty = DependencyProperty.Register("EditRowForm", GetType(DataTemplate), GetType(EntityServerModeCRUDBehavior), New PropertyMetadata(Nothing))
		Public Shared ReadOnly ObjectContextProperty As DependencyProperty = DependencyProperty.Register("ObjectContext", GetType(ObjectContext), GetType(EntityServerModeCRUDBehavior), New PropertyMetadata(Nothing))
		Public Shared ReadOnly EntityTypeProperty As DependencyProperty = DependencyProperty.Register("EntityType", GetType(Type), GetType(EntityServerModeCRUDBehavior), New PropertyMetadata(Nothing))
		Public Shared ReadOnly DataSourceProperty As DependencyProperty = DependencyProperty.Register("DataSource", GetType(EntityServerModeDataSource), GetType(EntityServerModeCRUDBehavior), New PropertyMetadata(Nothing))
		Public Shared ReadOnly AllowKeyDownActionsProperty As DependencyProperty = DependencyProperty.Register("AllowKeyDownActions", GetType(Boolean), GetType(EntityServerModeCRUDBehavior), New PropertyMetadata(False))

		Public Property NewRowForm() As DataTemplate
			Get
				Return CType(GetValue(NewRowFormProperty), DataTemplate)
			End Get
			Set(ByVal value As DataTemplate)
				SetValue(NewRowFormProperty, value)
			End Set
		End Property
		Public Property EditRowForm() As DataTemplate
			Get
				Return CType(GetValue(EditRowFormProperty), DataTemplate)
			End Get
			Set(ByVal value As DataTemplate)
				SetValue(EditRowFormProperty, value)
			End Set
		End Property
		Public Property ObjectContext() As ObjectContext
			Get
				Return CType(GetValue(ObjectContextProperty), ObjectContext)
			End Get
			Set(ByVal value As ObjectContext)
				SetValue(ObjectContextProperty, value)
			End Set
		End Property
		Public Property EntityType() As Type
			Get
				Return CType(GetValue(EntityTypeProperty), Type)
			End Get
			Set(ByVal value As Type)
				SetValue(EntityTypeProperty, value)
			End Set
		End Property
		Public Property DataSource() As EntityServerModeDataSource
			Get
				Return CType(GetValue(DataSourceProperty), EntityServerModeDataSource)
			End Get
			Set(ByVal value As EntityServerModeDataSource)
				SetValue(DataSourceProperty, value)
			End Set
		End Property
		Public Property AllowKeyDownActions() As Boolean
			Get
				Return CBool(GetValue(AllowKeyDownActionsProperty))
			End Get
			Set(ByVal value As Boolean)
				SetValue(AllowKeyDownActionsProperty, value)
			End Set
		End Property

		Private ReadOnly Property Grid() As GridControl
			Get
				Return AssociatedObject
			End Get
		End Property
		Public ReadOnly Property View() As TableView
			Get
				Return If(Grid IsNot Nothing, CType(Grid.View, TableView), Nothing)
			End Get
		End Property

		#Region "Commands"
		Private privateNewRowCommand As ICommand
		Public Property NewRowCommand() As ICommand
			Get
				Return privateNewRowCommand
			End Get
			Private Set(ByVal value As ICommand)
				privateNewRowCommand = value
			End Set
		End Property
		Private privateRemoveRowCommand As CustomCommand
		Public Property RemoveRowCommand() As CustomCommand
			Get
				Return privateRemoveRowCommand
			End Get
			Private Set(ByVal value As CustomCommand)
				privateRemoveRowCommand = value
			End Set
		End Property
		Private privateEditRowCommand As CustomCommand
		Public Property EditRowCommand() As CustomCommand
			Get
				Return privateEditRowCommand
			End Get
			Private Set(ByVal value As CustomCommand)
				privateEditRowCommand = value
			End Set
		End Property
		Protected Overridable Sub ExecuteNewRowCommand()
			AddNewRow()
		End Sub
		Protected Overridable Function CanExecuteNewRowCommand() As Boolean
			If ObjectContext Is Nothing OrElse DataSource Is Nothing Then
				Return False
			End If
			Return True
		End Function
		Protected Overridable Sub ExecuteRemoveRowCommand()
			RemoveSelectedRows()
		End Sub
		Protected Overridable Function CanExecuteRemoveRowCommand() As Boolean
            If ObjectContext Is Nothing OrElse DataSource Is Nothing OrElse Grid Is Nothing OrElse View Is Nothing OrElse Grid.CurrentItem Is Nothing Then
                Return False
            End If
			Return True
		End Function
		Protected Overridable Sub ExecuteEditRowCommand()
			EditRow()
		End Sub
		Protected Overridable Function CanExecuteEditRowCommand() As Boolean
			Return CanExecuteRemoveRowCommand()
		End Function
		#End Region

		Public Sub New()
			NewRowCommand = New DelegateCommand(AddressOf ExecuteNewRowCommand, AddressOf CanExecuteNewRowCommand)
			RemoveRowCommand = New CustomCommand(AddressOf ExecuteRemoveRowCommand, AddressOf CanExecuteRemoveRowCommand)
			EditRowCommand = New CustomCommand(AddressOf ExecuteEditRowCommand, AddressOf CanExecuteEditRowCommand)
		End Sub
		Public Overridable Function CreateNewRow() As Object
			Return Activator.CreateInstance(EntityType)
		End Function
		Public Sub AddNewRow(ByVal newRow As Object)
			If ObjectContext Is Nothing OrElse DataSource Is Nothing Then
				Return
			End If
			ObjectContext.AddObject(EntityType.Name & "s", newRow)
			ObjectContext.SaveChanges()
			DataSource.Reload()
		End Sub
		Public Sub RemoveRow(ByVal row As Object)
			If ObjectContext Is Nothing OrElse DataSource Is Nothing Then
				Return
			End If
			ObjectContext.DeleteObject(row)
			ObjectContext.SaveChanges()
			DataSource.Reload()
		End Sub
		Public Sub AddNewRow()
			Dim dialog As DXWindow = CreateDialogWindow(CreateNewRow(), False)
			AddHandler dialog.Closed, AddressOf OnNewRowDialogClosed
			dialog.ShowDialog()
		End Sub
		Public Sub RemoveSelectedRows()
            Dim selectedRowHandles() As Integer = Grid.GetSelectedRowHandles()
			If selectedRowHandles IsNot Nothing OrElse selectedRowHandles.Length > 0 Then
				Dim rows As New List(Of Object)()
				For Each rowHandle As Integer In selectedRowHandles
					rows.Add(Grid.GetRow(rowHandle))
				Next rowHandle
				For Each row As Object In rows
					ObjectContext.DeleteObject(row)
				Next row
				ObjectContext.SaveChanges()
				DataSource.Reload()
            ElseIf Grid.CurrentItem IsNot Nothing Then
                RemoveRow(Grid.CurrentItem)
			End If
		End Sub
		Public Sub EditRow()
            If Grid Is Nothing OrElse Grid.CurrentItem Is Nothing Then
                Return
            End If
            Dim dialog As DXWindow = CreateDialogWindow(Grid.CurrentItem, True)
			AddHandler dialog.Closed, AddressOf OnEditRowDialogClosed
			dialog.ShowDialog()
		End Sub
		Protected Overridable Function CreateDialogWindow(ByVal content As Object, Optional ByVal isEditingMode As Boolean = False) As DXWindow
			Dim dialog As DXDialog = New DXDialog With {.Tag = content, .Buttons = DialogButtons.OkCancel, .Title = If(isEditingMode, "Edit Row", "Add New Row"), .SizeToContent = SizeToContent.WidthAndHeight}
			Dim c As ContentControl = New ContentControl With {.Content = content}
			If isEditingMode Then
				dialog.Title = "Edit Row"
				c.ContentTemplate = EditRowForm
			Else
				dialog.Title = "Add New Row"
				c.ContentTemplate = NewRowForm
			End If
			dialog.Content = c
			Return dialog
		End Function
		Protected Overridable Sub OnNewRowDialogClosed(ByVal sender As Object, ByVal e As EventArgs)
			RemoveHandler (CType(sender, DXWindow)).Closed, AddressOf OnNewRowDialogClosed
			If CBool((CType(sender, DXWindow)).DialogResult) Then
				AddNewRow((CType(sender, DXWindow)).Tag)
			End If
		End Sub
		Protected Overridable Sub OnEditRowDialogClosed(ByVal sender As Object, ByVal e As EventArgs)
			RemoveHandler (CType(sender, DXWindow)).Closed, AddressOf OnEditRowDialogClosed
			If CBool((CType(sender, DXDialog)).DialogResult) Then
				ObjectContext.SaveChanges()
				DataSource.Reload()
			Else
                ObjectContext.Refresh(RefreshMode.StoreWins, Grid.CurrentItem)
			End If
		End Sub
		Protected Overridable Sub OnViewKeyDown(ByVal sender As Object, ByVal e As KeyEventArgs)
			If (Not AllowKeyDownActions) Then
				Return
			End If
			If e.Key = Key.Delete Then
				RemoveSelectedRows()
				e.Handled = True
			End If
			If e.Key = Key.Enter Then
				EditRow()
				e.Handled = True
			End If
		End Sub
		Protected Overridable Sub OnViewRowDoubleClick(ByVal sender As Object, ByVal e As RowDoubleClickEventArgs)
			EditRow()
			e.Handled = True
		End Sub
		Protected Overridable Sub OnGridLoaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
			RemoveHandler Grid.Loaded, AddressOf OnGridLoaded
			Initialize()
		End Sub
        Protected Overridable Sub OnCurrentItemChanged(ByVal sender As Object, ByVal e As CurrentItemChangedEventArgs)
            RemoveRowCommand.CheckCanExecuteChanged()
            EditRowCommand.CheckCanExecuteChanged()
        End Sub
        Protected Overrides Sub OnAttached()
            MyBase.OnAttached()
            Grid.ItemsSource = DataSource.Data
            If View IsNot Nothing Then
                Initialize()
            Else
                AddHandler Grid.Loaded, AddressOf OnGridLoaded
            End If
        End Sub
        Protected Overrides Sub OnDetaching()
            Uninitialize()
            MyBase.OnDetaching()
        End Sub
        Private Sub Initialize()
            AddHandler View.KeyDown, AddressOf OnViewKeyDown
            AddHandler View.RowDoubleClick, AddressOf OnViewRowDoubleClick
            AddHandler Grid.CurrentItemChanged, AddressOf OnCurrentItemChanged
        End Sub
        Private Sub Uninitialize()
            RemoveHandler View.KeyDown, AddressOf OnViewKeyDown
            RemoveHandler View.RowDoubleClick, AddressOf OnViewRowDoubleClick
            RemoveHandler Grid.CurrentItemChanged, AddressOf OnCurrentItemChanged
        End Sub
	End Class
	Public Class CustomCommand
		Implements ICommand
		Private _executeMethod As Action
		Private _canExecuteMethod As Func(Of Boolean)
		Public Sub New(ByVal executeMethod As Action, ByVal canExecuteMethod As Func(Of Boolean))
			_executeMethod = executeMethod
			_canExecuteMethod = canExecuteMethod
		End Sub
		Public Function CanExecute(ByVal parameter As Object) As Boolean Implements ICommand.CanExecute
			Return _canExecuteMethod()
		End Function
		Public Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged
		Public Sub Execute(ByVal parameter As Object) Implements ICommand.Execute
			_executeMethod()
		End Sub
		Public Sub CheckCanExecuteChanged()
			RaiseEvent CanExecuteChanged(Me, EventArgs.Empty)
		End Sub
	End Class
End Namespace