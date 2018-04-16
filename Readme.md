# How to implement CRUD operations using DXGrid and Entity Framework (Server Mode)


<p>This example shows how to use EntityServerModeDataSource with DXGrid, and how to implement CRUD operations (e.g., add, remove, edit) in your application via special behavior.</p>
<p><strong>Note</strong> that the test sample requires the SQL Express service to be installed on your machine.</p>
<p>We have created the EntityServerModeCRUDBehavior attached behavior for GridControl. For instance:</p>


```xml
<dxg:GridControl>
	<dxmvvm:Interaction.Behaviors>
		<crud:EntityServerModeCRUDBehavior ...>
			<crud:EntityServerModeCRUDBehavior.DataSource>
				<dxsm:EntityServerModeDataSource .../>
			</crud:EntityServerModeCRUDBehavior.DataSource>
		</crud:EntityServerModeCRUDBehavior>
	</dxmvvm:Interaction.Behaviors>
</dxg:GridControl>

```


<p>The EntityServerModeCRUDBehavior class contains the NewRowForm and EditRowForm properties to provide the "Add Row" and "Edit Row" actions. With these properties, you can create the Add and Edit forms according to your requirements:</p>


```xml
<DataTemplate x:Key="EditRecordTemplate">
   <StackPanel Margin="8" MinWidth="200">
       <Grid>
           <Grid.ColumnDefinitions>
               <ColumnDefinition/>
               <ColumnDefinition/>
           </Grid.ColumnDefinitions>
           <Grid.RowDefinitions>
               <RowDefinition/>
               <RowDefinition/>
           </Grid.RowDefinitions>
           <TextBlock Text="ID:" VerticalAlignment="Center" Grid.Row="0" Grid.Column="0" Margin="0,0,6,4" />
           <dxe:TextEdit x:Name="txtID" Grid.Row="0" Grid.Column="1" EditValue="{Binding Path=Id, Mode=TwoWay}" Margin="0,0,0,4" />
           <TextBlock Text="Name:" VerticalAlignment="Center" Grid.Row="1" Grid.Column="0" Margin="0,0,6,4" />
           <dxe:TextEdit x:Name="txtCompany" Grid.Row="1" Grid.Column="1" EditValue="{Binding Path=Name, Mode=TwoWay}" Margin="0,0,0,4" />
       </Grid>
   </StackPanel>
</DataTemplate>

<crud:EntityServerModeCRUDBehavior NewRowForm="{StaticResource ResourceKey=EditRecordTemplate}" EditRowForm="{StaticResource ResourceKey=EditRecordTemplate}"/>

```


<p>This behavior requires the following information from your data model:</p>
<p>- EntityType - the type of rows;</p>
<p>- ObjectContext - database entities;</p>
<p>- DataSource - an object of the EntityServerModeDataSource type.</p>


```xml
<crud:EntityServerModeCRUDBehavior EntityType="{x:Type local:Item}" ObjectContext="{StaticResource ResourceKey=DatabaseEntites}">
   <crud:EntityServerModeCRUDBehavior.DataSource>
       <dxsm:EntityServerModeDataSource KeyExpression="Id" QueryableSource="{Binding Items, Source={StaticResource DatabaseEntites}}"/>
   </crud:EntityServerModeCRUDBehavior.DataSource>
</crud:EntityServerModeCRUDBehavior>

```


<p>See the <a href="http://documentation.devexpress.com/#WPF/clsDevExpressXpfCoreServerModeEntityServerModeDataSourcetopic"><u>EntityServerModeDataSource Class</u></a> to learn more about EntityServerModeDataSource.</p>
<p><br /> Behavior class descendants support the following commands: NewRowCommand, RemoveRowCommand, EditRowCommand. You can bind your interaction controls with these commands with ease. For instance:</p>


```xml
<crud:EntityServerModeCRUDBehavior x:Name="helper"/>

<StackPanel Grid.Row="1" Orientation="Horizontal" HorizontalAlignment="Center">
   <Button Height="22" Width="60" Command="{Binding Path=NewRowCommand, ElementName=helper}">Add</Button>
   <Button Height="22" Width="60" Command="{Binding Path=RemoveRowCommand, ElementName=helper}" Margin="6,0,6,0">Remove</Button>
   <Button Height="22" Width="60" Command="{Binding Path=EditRowCommand, ElementName=helper}">Edit</Button>
</StackPanel>

```


<p>By default, the EntityServerModeCRUDBehavior solution supports the following end-user interaction capabilities:</p>
<p>1. An end-user can edit selected row values by double-clicking on a grid row or by pressing the Enter key if the AllowKeyDownActions property is True.</p>
<p>2. An end-user can remove selected rows via the Delete key press if the AllowKeyDownActions property is True.</p>
<p>3. An end-user can add new rows, remove and edit them via the NewRowCommand, RemoveRowCommand, and EditRowCommand commands.</p>

<br/>


