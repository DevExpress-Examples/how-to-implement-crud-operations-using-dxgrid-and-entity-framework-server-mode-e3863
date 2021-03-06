﻿'------------------------------------------------------------------------------
' <auto-generated>
'    This code was generated from a template.
'
'    Manual changes to this file may cause unexpected behavior in your application.
'    Manual changes to this file will be overwritten if the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------


Imports Microsoft.VisualBasic
Imports System
Imports System.Data.Objects
Imports System.Data.Objects.DataClasses
Imports System.Data.EntityClient
Imports System.ComponentModel
Imports System.Xml.Serialization
Imports System.Runtime.Serialization

<Assembly: EdmSchemaAttribute()>

Namespace EntityServer
	#Region "Contexts"

	''' <summary>
	''' No Metadata Documentation available.
	''' </summary>
	Partial Public Class DatabaseEntities
		Inherits ObjectContext
		#Region "Constructors"

		''' <summary>
		''' Initializes a new DatabaseEntities object using the connection string found in the 'DatabaseEntities' section of the application configuration file.
		''' </summary>
		Public Sub New()
			MyBase.New("name=DatabaseEntities", "DatabaseEntities")
			Me.ContextOptions.LazyLoadingEnabled = True
			OnContextCreated()
		End Sub

		''' <summary>
		''' Initialize a new DatabaseEntities object.
		''' </summary>
		Public Sub New(ByVal connectionString As String)
			MyBase.New(connectionString, "DatabaseEntities")
			Me.ContextOptions.LazyLoadingEnabled = True
			OnContextCreated()
		End Sub

		''' <summary>
		''' Initialize a new DatabaseEntities object.
		''' </summary>
		Public Sub New(ByVal connection As EntityConnection)
			MyBase.New(connection, "DatabaseEntities")
			Me.ContextOptions.LazyLoadingEnabled = True
			OnContextCreated()
		End Sub

		#End Region

		#Region "Partial Methods"

		Partial Private Sub OnContextCreated()
		End Sub

		#End Region

		#Region "ObjectSet Properties"

		''' <summary>
		''' No Metadata Documentation available.
		''' </summary>
		Public ReadOnly Property Items() As ObjectSet(Of Item)
			Get
				If (_Items Is Nothing) Then
					_Items = MyBase.CreateObjectSet(Of Item)("Items")
				End If
				Return _Items
			End Get
		End Property
		Private _Items As ObjectSet(Of Item)

		#End Region
		#Region "AddTo Methods"

		''' <summary>
		''' Deprecated Method for adding a new object to the Items EntitySet. Consider using the .Add method of the associated ObjectSet&lt;T&gt; property instead.
		''' </summary>
		Public Sub AddToItems(ByVal item As Item)
			MyBase.AddObject("Items", item)
		End Sub

		#End Region
	End Class


	#End Region

	#Region "Entities"

	''' <summary>
	''' No Metadata Documentation available.
	''' </summary>
	<EdmEntityTypeAttribute(NamespaceName:="DatabaseModel", Name:="Item"), Serializable(), DataContractAttribute(IsReference:=True)> _
	Partial Public Class Item
		Inherits EntityObject
		#Region "Factory Method"

		''' <summary>
		''' Create a new Item object.
		''' </summary>
		''' <param name="id">Initial value of the Id property.</param>
		Public Shared Function CreateItem(ByVal id As Global.System.Int32) As Item
			Dim item As New Item()
			item.Id = id
			Return item
		End Function

		#End Region
		#Region "Primitive Properties"

		''' <summary>
		''' No Metadata Documentation available.
		''' </summary>
		<EdmScalarPropertyAttribute(EntityKeyProperty:=True, IsNullable:=False), DataMemberAttribute()> _
		Public Property Id() As Global.System.Int32
			Get
				Return _Id
			End Get
			Set(ByVal value As System.Int32)
				If _Id <> value Then
					OnIdChanging(value)
					ReportPropertyChanging("Id")
					_Id = StructuralObject.SetValidValue(value)
					ReportPropertyChanged("Id")
					OnIdChanged()
				End If
			End Set
		End Property
		Private _Id As Global.System.Int32
		Partial Private Sub OnIdChanging(ByVal value As Global.System.Int32)
		End Sub
		Partial Private Sub OnIdChanged()
		End Sub

		''' <summary>
		''' No Metadata Documentation available.
		''' </summary>
		<EdmScalarPropertyAttribute(EntityKeyProperty:=False, IsNullable:=True), DataMemberAttribute()> _
		Public Property Name() As Global.System.String
			Get
				Return _Name
			End Get
			Set(ByVal value As System.String)
				OnNameChanging(value)
				ReportPropertyChanging("Name")
				_Name = StructuralObject.SetValidValue(value, True)
				ReportPropertyChanged("Name")
				OnNameChanged()
			End Set
		End Property
		Private _Name As Global.System.String
		Partial Private Sub OnNameChanging(ByVal value As Global.System.String)
		End Sub
		Partial Private Sub OnNameChanged()
		End Sub

		#End Region

	End Class

	#End Region

End Namespace
