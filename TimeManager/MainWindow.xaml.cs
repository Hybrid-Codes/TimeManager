using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using TimeManagerLibrary; // Import your custom class library

namespace TimeManager
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<Module> modules = new ObservableCollection<Module>();
        private Semester currentSemester;

        public MainWindow()
        {
            InitializeComponent();
            ModuleListView.ItemsSource = modules;
            currentSemester = new Semester();
            DataContext = this;
        }
        
        
        private void AddModuleButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate input and add a module
            if (int.TryParse(CreditsTextBox.Text, out int credits) && int.TryParse(ClassHoursTextBox.Text, out int classHours))
            {
                Module module = new Module
                {
                    Code = ModuleCodeTextBox.Text,
                    Name = ModuleNameTextBox.Text,
                    Credits = credits,
                    ClassHoursPerWeek = classHours
                };
                modules.Add(module);

                // Calculate and set self-study hours
                double selfStudyHours = module.CalculateSelfStudyHours(currentSemester.WeeksInSemester);
                module.SelfStudyHours = selfStudyHours;

                // Clear input fields
                ModuleCodeTextBox.Clear();
                ModuleNameTextBox.Clear();
                CreditsTextBox.Clear();
                ClassHoursTextBox.Clear();
            }
            else
            {
                MessageBox.Show("Invalid input. Please enter valid numeric values.");
            }
        }
        private void SetSemesterButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(WeeksTextBox.Text, out int weeks) && StartDatePicker.SelectedDate.HasValue)
            {
                currentSemester.WeeksInSemester = weeks;
                currentSemester.StartDate = StartDatePicker.SelectedDate.Value;
            }
            else
            {
                MessageBox.Show("Invalid input. Please enter valid values.");
            }
        }

        private void RecordStudyHoursButton_Click(object sender, RoutedEventArgs e)
        {
            if (ModuleComboBox.SelectedItem is Module selectedModule &&
                RecordDatePicker.SelectedDate.HasValue &&
                double.TryParse(StudyHoursTextBox.Text, out double studyHours))
            {
                DateTime selectedDate = RecordDatePicker.SelectedDate.Value.Date;

                // Find or create a record for the selected date
                var record = selectedModule.StudyRecords.FirstOrDefault(r => r.Date == selectedDate);
                if (record == null)
                {
                    record = new StudyRecord { Date = selectedDate };
                    selectedModule.StudyRecords.Add(record);
                }

                // Update the study hours for the selected date
                record.Hours += studyHours;

                // Update the remaining hours
                UpdateRemainingHours(selectedModule);

                // Clear input fields
                StudyHoursTextBox.Clear();
            }
            else
            {
                MessageBox.Show("Invalid input. Please select a module, enter a valid date, and specify study hours.");
            }
        }

        private void UpdateRemainingHours(Module module)
        {
            // Calculate remaining hours for the current week
            DateTime currentDate = DateTime.Now;
            DateTime startOfWeek = currentDate.AddDays(-(int)currentDate.DayOfWeek);

            var currentWeekRecords = module.StudyRecords
                .Where(record => record.Date >= startOfWeek)
                .ToList();

            double totalStudyHours = currentWeekRecords.Sum(record => record.Hours);
            double remainingHours = module.SelfStudyHours - totalStudyHours;

            RemainingHoursTextBlock.Text = $"Remaining Hours for {module.Name}: {remainingHours:F2} hours";
        }
    }
}