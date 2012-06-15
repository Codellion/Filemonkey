using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FileMonkey.Pandora.dal;
using FileMonkey.Pandora.dal.entities;
using System.Windows.Forms;
using System.Data;

namespace FileMonkey.Picasso
{
    /// <summary>
    /// Lógica de interacción para InspectorDetail.xaml
    /// </summary>
    public partial class InspectorDetail : Window
    {
        public Inspector inspector { set; get; }
        public Boolean optionPressed { set; get; }
        
        public InspectorDetail()
        {
            InitializeComponent();
            inspector = new Inspector { Path = "Seleccione una carpeta para rastrear" };

            App.ActualInspectorDetail = this;
            this.Owner = App.Home;
        }

        public InspectorDetail(Inspector inspector)
        {
            InitializeComponent();
            this.inspector = inspector;

            App.ActualInspectorDetail = this;            
            this.Owner = App.Inspectors;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtName.Text = inspector.Name;
            txtPath.Text = inspector.Path;
            slPeriod.Value = inspector.CheckPeriod;
            txtPeriod.Text = InspectorDetail.GetPeriodText(inspector.CheckPeriod);

            if (inspector.Action == (int)Inspector.TypeActions.MoveSubDir)
            {
                rbtMoveSubDir.IsChecked = true;

                if(!String.IsNullOrEmpty(inspector.SubDirAction))
                {
                    txtPathAction.Text = inspector.SubDirAction;
                }
            }
            else
            {
                rbtDeleteFiles.IsChecked = true;                
            }

            if (inspector.InspectorId != 0)
            {
                rbtMoveSubDir.IsEnabled = false;
                rbtDeleteFiles.IsEnabled = false;
            }
                
            RulesRefresh(null);
        }

        private void RulesRefresh(RuleFile rule_, Boolean added = false)
        {
            List<RuleFile> lstRules = new List<RuleFile>();

            if (rule_ == null)
            {
                using (var db = new DataContext())
                {
                    db.AttachEntity(inspector);

                    inspector.Rules.ToList().ForEach(rule => SetImageRule(rule));
                    lstRules = inspector.Rules.ToList();
                }
            }
            else
            {

                lstRules = (List<RuleFile>)lstVRules.ItemsSource;

                if (added)
                {
                    lstRules.Add(rule_);
                }
                else
                    lstRules.Remove(rule_);
            }

            lstVRules.ItemsSource = null;
            lstVRules.ItemsSource = lstRules;
        }

        private void slPeriod_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            txtPeriod.Text = InspectorDetail.GetPeriodText((int)e.NewValue);
            inspector.CheckPeriod = (int)e.NewValue;
        }

        public static String GetPeriodText(int pValue)
        {
            String result = String.Empty;

            if (pValue == 0)
            {
                result = "5 segundos";
            }            
            else if (pValue < 4)
            {
                result = ((int)pValue * 15).ToString() + " segundos";
            }
            else if (pValue < 11)
            {
                String value = "1";

                switch ((int)pValue)
                {
                    case 5: value = "2";
                        break;
                    case 6: value = "5";
                        break;
                    case 7: value = "10";
                        break;
                    case 8: value = "15";
                        break;
                    case 9: value = "30";
                        break;
                    case 10: value = "45";
                        break;
                }

                result = value + " minutos";
            }
            else
            {
                result = ((int)pValue - 10).ToString() + " horas";
            }

            return result;
        }

        private void SetImageRule(RuleFile rule)
        {
            switch ((RuleFile.TypeFileRule)rule.RuleType)
            {
                case RuleFile.TypeFileRule.Date:
                    rule.ImagePath = @"/images/icon_ruleFileDate.png";
                    break;
                case RuleFile.TypeFileRule.Extension:
                    rule.ImagePath = @"/images/icon_ruleFileExtension.png";
                    break;
                case RuleFile.TypeFileRule.FileName:
                    rule.ImagePath = @"/images/icon_ruleFileName.png";
                    break;
            }
        }
       
        private void lstVRules_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lstVRules.SelectedIndex != -1)
            {
                RuleFile ruleSelected = (RuleFile)lstVRules.SelectedValue;

                NewRule ruleDetail = new NewRule(ruleSelected);
                ruleDetail.ShowDialog();

                ruleSelected = ruleDetail.rule;

                using (var db = new DataContext())
                {                  
                    db.PersistEntity(ruleSelected);
                    db.SaveChanges();
                }

                App.Single.UpdateWork(inspector);

                List<RuleFile> lstRules = (List<RuleFile>)lstVRules.ItemsSource;

                lstVRules.ItemsSource = null;
                lstVRules.ItemsSource = lstRules;
            }
        }

        private void panels_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            StackPanel panel = (StackPanel)sender;
            System.Windows.Controls.Label label = (System.Windows.Controls.Label)panel.Children[1];
            label.FontWeight = FontWeights.ExtraBold;
        }

        private void panels_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            StackPanel panel = (StackPanel)sender;
            System.Windows.Controls.Label label = (System.Windows.Controls.Label)panel.Children[1];
            label.FontWeight = FontWeights.Normal;
        }

        private void pnlNewInspector_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!optionPressed)
                return;

            using (var db = new DataContext())
            {                
                inspector.Name = txtName.Text;
                inspector.Path = txtPath.Text;

                if (rbtMoveSubDir.IsChecked.HasValue && rbtMoveSubDir.IsChecked.Value)
                {
                    inspector.Action = (int) Inspector.TypeActions.MoveSubDir;
                    inspector.SubDirAction = txtPathAction.Text;
                }
                else
                {
                    inspector.Action = (int)Inspector.TypeActions.DeleteFiles;
                    inspector.SubDirAction = String.Empty;
                }

                Boolean exist = true;

                if (inspector.InspectorId == 0)
                {                    
                    exist = false;
                }

                db.PersistEntity(inspector);
                db.SaveChanges();

                if (!exist)
                {
                    App.Single.AddWork(inspector);
                }
                else
                {
                    App.Single.UpdateWork(inspector);
                }
            }

            this.Close();
        }

        private void pnlChoosePath_MouseUp(object sender, MouseButtonEventArgs e)
        {
            SetFileBrowser(txtPath);
        }

        private void SetFileBrowser(TextBlock control)
        {
            string selectedFolder = string.Empty;
            FolderBrowserDialog selectFolderDialog = new FolderBrowserDialog();
            selectFolderDialog.ShowNewFolderButton = true;
                
            if (selectFolderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                control.Text = selectFolderDialog.SelectedPath;
            }
        }

        private void pnlNewRule_MouseUp(object sender, MouseButtonEventArgs e)
        {
            NewRule newRule = new NewRule();
            newRule.ShowDialog();

            if (newRule.rule != null)
            {
                RuleFile rule = newRule.rule;

                using (var db = new DataContext())
                {
                    db.AttachEntity(inspector);
                    inspector.Rules.Add(rule);

                    Boolean exist = true;

                    if (inspector.InspectorId == 0)
                    {
                        inspector.Name = txtName.Text;
                        inspector.Path = txtPath.Text;

                        if (rbtMoveSubDir.IsChecked.HasValue && rbtMoveSubDir.IsChecked.Value)
                        {
                            inspector.Action = (int)Inspector.TypeActions.MoveSubDir;
                            inspector.SubDirAction = txtPathAction.Text;
                        }
                        else
                        {
                            inspector.Action = (int)Inspector.TypeActions.DeleteFiles;
                            inspector.SubDirAction = String.Empty;
                        }

                        db.PersistEntity(inspector);
                        exist = false;
                    }

                    db.SaveChanges();

                    if(!exist)
                    {
                        App.Single.AddWork(inspector);
                    }
                    else
                    {
                        App.Single.UpdateWork(inspector);
                    }
                }

                RulesRefresh(rule, true);
            }
        }

        private void pnlDeleteRule_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (lstVRules.SelectedIndex != -1)
            {
                RuleFile ruleSelected = (RuleFile)lstVRules.SelectedValue;

                using (var db = new DataContext())
                {
                    db.RemoveEntity(ruleSelected);
                    db.SaveChanges();

                    App.Single.UpdateWork(inspector);
                }

                RulesRefresh(ruleSelected);
            }
            else
            {
                System.Windows.MessageBox.Show("Por favor, seleccione una regla de la lista");
            }
        }

        private void pnlChoosePathAction_MouseUp(object sender, MouseButtonEventArgs e)
        {
            SetFileBrowser(txtPathAction);
        }

        private void rbtMoveSubDir_Checked(object sender, RoutedEventArgs e)
        {
            pnlChoosePathActionTot.Visibility = System.Windows.Visibility.Visible;
        }

        private void rbtDeleteFiles_Checked(object sender, RoutedEventArgs e)
        {
            pnlChoosePathActionTot.Visibility = System.Windows.Visibility.Hidden;
        }

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            optionPressed = true;
        }
    }
}
