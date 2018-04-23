using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Grid;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.ServerMode;
using DevExpress.Xpf.Bars;
using System.Data.Objects;
using DevExpress.Xpf.Mvvm;
using DevExpress.Xpf.Mvvm.UI.Interactivity;

namespace EntityServer {
    public class EntityServerModeCRUDBehavior: Behavior<GridControl> {
        public static readonly DependencyProperty NewRowFormProperty =
            DependencyProperty.Register("NewRowForm", typeof(DataTemplate), typeof(EntityServerModeCRUDBehavior), new PropertyMetadata(null));
        public static readonly DependencyProperty EditRowFormProperty =
            DependencyProperty.Register("EditRowForm", typeof(DataTemplate), typeof(EntityServerModeCRUDBehavior), new PropertyMetadata(null));
        public static readonly DependencyProperty ObjectContextProperty =
            DependencyProperty.Register("ObjectContext", typeof(ObjectContext), typeof(EntityServerModeCRUDBehavior), new PropertyMetadata(null));
        public static readonly DependencyProperty EntityTypeProperty =
            DependencyProperty.Register("EntityType", typeof(Type), typeof(EntityServerModeCRUDBehavior), new PropertyMetadata(null));
        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register("DataSource", typeof(EntityServerModeDataSource), typeof(EntityServerModeCRUDBehavior), new PropertyMetadata(null));
        public static readonly DependencyProperty AllowKeyDownActionsProperty =
            DependencyProperty.Register("AllowKeyDownActions", typeof(bool), typeof(EntityServerModeCRUDBehavior), new PropertyMetadata(false));

        public DataTemplate NewRowForm {
            get { return (DataTemplate)GetValue(NewRowFormProperty); }
            set { SetValue(NewRowFormProperty, value); }
        }
        public DataTemplate EditRowForm {
            get { return (DataTemplate)GetValue(EditRowFormProperty); }
            set { SetValue(EditRowFormProperty, value); }
        }
        public ObjectContext ObjectContext {
            get { return (ObjectContext)GetValue(ObjectContextProperty); }
            set { SetValue(ObjectContextProperty, value); }
        }
        public Type EntityType {
            get { return (Type)GetValue(EntityTypeProperty); }
            set { SetValue(EntityTypeProperty, value); }
        }
        public EntityServerModeDataSource DataSource {
            get { return (EntityServerModeDataSource)GetValue(DataSourceProperty); }
            set { SetValue(DataSourceProperty, value); }
        }
        public bool AllowKeyDownActions {
            get { return (bool)GetValue(AllowKeyDownActionsProperty); }
            set { SetValue(AllowKeyDownActionsProperty, value); }
        }

        GridControl Grid { get { return AssociatedObject; } }
        public TableView View { get { return Grid != null ? (TableView)Grid.View : null; } }

        #region Commands
        public ICommand NewRowCommand { get; private set; }
        public CustomCommand RemoveRowCommand { get; private set; }
        public CustomCommand EditRowCommand { get; private set; }
        protected virtual void ExecuteNewRowCommand() {
            AddNewRow();
        }
        protected virtual bool CanExecuteNewRowCommand() {
            if(ObjectContext == null || DataSource == null) return false;
            return true;
        }
        protected virtual void ExecuteRemoveRowCommand() {
            RemoveSelectedRows();
        }
        protected virtual bool CanExecuteRemoveRowCommand() {
            if(ObjectContext == null || DataSource == null || Grid == null || View == null || Grid.CurrentItem == null) return false;
            return true;
        }
        protected virtual void ExecuteEditRowCommand() {
            EditRow();
        }
        protected virtual bool CanExecuteEditRowCommand() {
            return CanExecuteRemoveRowCommand();
        }
        #endregion

        public EntityServerModeCRUDBehavior() {
            NewRowCommand = new DelegateCommand(ExecuteNewRowCommand, CanExecuteNewRowCommand);
            RemoveRowCommand = new CustomCommand(ExecuteRemoveRowCommand, CanExecuteRemoveRowCommand);
            EditRowCommand = new CustomCommand(ExecuteEditRowCommand, CanExecuteEditRowCommand);
        }
        public virtual object CreateNewRow() {
            return Activator.CreateInstance(EntityType);
        }
        public void AddNewRow(object newRow) {
            if(ObjectContext == null || DataSource == null) return;
            ObjectContext.AddObject(EntityType.Name + "s", newRow);
            ObjectContext.SaveChanges();
            DataSource.Reload();
        }
        public void RemoveRow(object row) {
            if(ObjectContext == null || DataSource == null) return;
            ObjectContext.DeleteObject(row);
            ObjectContext.SaveChanges();
            DataSource.Reload();
        }
        public void AddNewRow() {
            DXWindow dialog = CreateDialogWindow(CreateNewRow(), false);
            dialog.Closed += OnNewRowDialogClosed;
            dialog.ShowDialog();
        }
        public void RemoveSelectedRows() {
            int[] selectedRowHandles = Grid.GetSelectedRowHandles();
            if(selectedRowHandles != null || selectedRowHandles.Length > 0) {
                List<object> rows = new List<object>();
                foreach(int rowHandle in selectedRowHandles)
                    rows.Add(Grid.GetRow(rowHandle));
                foreach(object row in rows)
                    ObjectContext.DeleteObject(row);
                ObjectContext.SaveChanges();
                DataSource.Reload();
            }
            else if(Grid.CurrentItem != null)
                RemoveRow(Grid.CurrentItem);
        }
        public void EditRow() {
            if(Grid == null || Grid.CurrentItem == null) return;
            DXWindow dialog = CreateDialogWindow(Grid.CurrentItem, true);
            dialog.Closed += OnEditRowDialogClosed;
            dialog.ShowDialog();
        }
        protected virtual DXWindow CreateDialogWindow(object content, bool isEditingMode = false) {
            DXDialog dialog = new DXDialog {
                Tag = content,
                Buttons = DialogButtons.OkCancel,
                Title = isEditingMode ? "Edit Row" : "Add New Row",
                SizeToContent = SizeToContent.WidthAndHeight
            };
            ContentControl c = new ContentControl { Content = content };
            if(isEditingMode) {
                dialog.Title = "Edit Row";
                c.ContentTemplate = EditRowForm;
            }
            else {
                dialog.Title = "Add New Row";
                c.ContentTemplate = NewRowForm;
            }
            dialog.Content = c;
            return dialog;
        }
        protected virtual void OnNewRowDialogClosed(object sender, EventArgs e) {
            ((DXWindow)sender).Closed -= OnNewRowDialogClosed;
            if((bool)((DXWindow)sender).DialogResult) {
                AddNewRow(((DXWindow)sender).Tag);
            }
        }
        protected virtual void OnEditRowDialogClosed(object sender, EventArgs e) {
            ((DXWindow)sender).Closed -= OnEditRowDialogClosed;
            if((bool)((DXDialog)sender).DialogResult) {
                ObjectContext.SaveChanges();
                DataSource.Reload();
            }
            else
                ObjectContext.Refresh(RefreshMode.StoreWins, Grid.CurrentItem);
        }
        protected virtual void OnViewKeyDown(object sender, KeyEventArgs e) {
            if(!AllowKeyDownActions)
                return;
            if(e.Key == Key.Delete) {
                RemoveSelectedRows();
                e.Handled = true;
            }
            if(e.Key == Key.Enter) {
                EditRow();
                e.Handled = true;
            }
        }
        protected virtual void OnViewRowDoubleClick(object sender, RowDoubleClickEventArgs e) {
            EditRow();
            e.Handled = true;
        }
        protected virtual void OnGridLoaded(object sender, RoutedEventArgs e) {
            Grid.Loaded -= OnGridLoaded;
            Initialize();
        }
        protected virtual void OnCurrentItemChanged(object sender, CurrentItemChangedEventArgs e) {
            RemoveRowCommand.CheckCanExecuteChanged();
            EditRowCommand.CheckCanExecuteChanged();
        }
        protected override void OnAttached() {
            base.OnAttached();
            Grid.ItemsSource = DataSource.Data;
            if(View != null)
                Initialize();
            else Grid.Loaded += OnGridLoaded;
        }
        protected override void OnDetaching() {
            Uninitialize();
            base.OnDetaching();
        }
        void Initialize() {
            View.KeyDown += OnViewKeyDown;
            View.RowDoubleClick += OnViewRowDoubleClick;
            Grid.CurrentItemChanged += OnCurrentItemChanged;
        }
        void Uninitialize() {
            View.KeyDown -= OnViewKeyDown;
            View.RowDoubleClick -= OnViewRowDoubleClick;
            Grid.CurrentItemChanged -= OnCurrentItemChanged;
        }
    }
    public class CustomCommand: ICommand {
        Action _executeMethod;
        Func<bool> _canExecuteMethod;
        public CustomCommand(Action executeMethod, Func<bool> canExecuteMethod) {
            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
        }
        public bool CanExecute(object parameter) {
            return _canExecuteMethod();
        }
        public event EventHandler CanExecuteChanged;
        public void Execute(object parameter) {
            _executeMethod();
        }
        public void CheckCanExecuteChanged() {
            if(CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }
    }
}