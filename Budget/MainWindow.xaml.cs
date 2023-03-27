using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Newtonsoft.Json;

namespace Budget
{
    public partial class MainWindow
    {
        private List<string> types = new List<string>();
        private List<Note> Notes = new List<Note>();

        internal class Note
        {
            public string Name { get; set; }
            public int Amount { get; set; }
            public string Type { get; set; }
            public bool IsIncome { get; set; }
            public DateTime Date { get; set; }
            public Note(string name, int amount, string type, bool isIncome, DateTime date)
            {
                Name = name;
                Amount = amount;
                Type = type;
                IsIncome = isIncome;
                Date = date;
            }
        }
        
        public MainWindow()
        {
            InitializeComponent();
            Notes = Load(Notes, "C:/Users/alexa/OneDrive/Рабочий стол/Практическая/BudgetNotes.json");
            types = Load(types, "C:/Users/alexa/OneDrive/Рабочий стол/Практическая/BudgetTypes.json");
            
            foreach (var type in types)
            {
                TypeComboBox.Items.Add(type);
            }
            
            Calendar.SelectedDate = DateTime.Now;
            TotalSumUpdate();
        }

        private void NoteSum_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(isNum);
        }

        private bool isNum(char c)
        {
            if (c >= '0' && c <= '9')
            {
                return true;
            }
            if (c == '-')
            {
                return true;
            }

            return false;
        }

        private void AddType_OnClick(object sender, RoutedEventArgs e)
        {
            TypeWindow typeWindow = new TypeWindow();
            if (typeWindow.ShowDialog() == true)
            {
                string type = typeWindow.NewTypeTextBox.Text;
                TypeComboBox.Items.Add(type);
                types.Add(type);
                Save(types, "C:/Users/alexa/OneDrive/Рабочий стол/Практическая/BudgetTypes.json");
            }
        }

        private void AddNoteButton_OnClick(object sender, RoutedEventArgs e)
        {
            AddNoteButton.Content = "Добавить";
            Note note = new Note(NoteName.Text, Int32.Parse(NoteSum.Text), TypeComboBox.SelectedItem.ToString(), isIncome(Int32.Parse(NoteSum.Text)), Convert.ToDateTime(Calendar.SelectedDate));
            Notes.Add(note);
            Save(Notes, "C:/Users/alexa/OneDrive/Рабочий стол/Практическая/BudgetNotes.json");
            if (note.Amount < 0)
            {
                note.Amount *= -1;
                BudgetItemsDataGrid.Items.Add(note);
                note.Amount *= -1;
            }
            else
            {
                BudgetItemsDataGrid.Items.Add(note);
            }
            NoteName.Text = "Имя записи";
            NoteSum.Clear();
            TotalSumUpdate();
        }
        
        private void Calendar_OnSelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            BudgetItemsDataGrid.Items.Clear();
            NoteName.Text = "Имя записи";
            NoteSum.Clear();
            
            foreach (var n in Notes)
            {
                if (Convert.ToDateTime(Calendar.SelectedDate).DayOfYear == n.Date.DayOfYear)
                {
                    if (n.Amount < 0)
                    {
                        n.Amount *= -1;
                        BudgetItemsDataGrid.Items.Add(n);
                        n.Amount *= -1;
                    }
                    else
                    {
                        BudgetItemsDataGrid.Items.Add(n);
                    }
                }
            }
        }
        
        private void Save<T>(List<T> savedList, string filePath)
        {
            string json = JsonConvert.SerializeObject(savedList);
            File.WriteAllText(filePath, json);
        }
//"C:/Users/alexa/OneDrive/Рабочий стол/Практическая/notes_1.json"
        private List<T> Load<T>(List<T> loadList, string filePath)
        {
            try
            {
                var json = File.ReadAllText(filePath);
                loadList.Clear();
                loadList = JsonConvert.DeserializeObject<List<T>>(json);
                return loadList;
            }
            catch (Exception e)
            {
                return loadList = new List<T>();
            }
        }

        private void TotalSumUpdate()
        {
            int sum = 0;
            foreach (Note item in Notes)
            {
                sum += item.Amount;
            }
            TotalSum.Text = $"Итог: {sum}";
        }

        private bool isIncome(int amount)
        {
            if (amount >= 0)
            {
                return true;
            }

            return false;
        }

        private void Delete_OnClick(object sender, RoutedEventArgs e)
        {
            Note selected = BudgetItemsDataGrid.SelectedItem as Note;
            foreach (var note in Notes)
            {
                if (selected == note)
                {
                    Notes.Remove(note);
                    BudgetItemsDataGrid.Items.Remove(selected);
                    BudgetItemsDataGrid.Items.Refresh();
                    Save(Notes, "C:/Users/alexa/OneDrive/Рабочий стол/Практическая/BudgetNotes.json");
                    TotalSumUpdate();
                    break;
                }
            }
        }

        private void Edit_OnClick(object sender, RoutedEventArgs e)
        {
            Note selected = BudgetItemsDataGrid.SelectedItem as Note;
            AddNoteButton.Content = "Сохранить";
            foreach (var note in Notes)
            {
                if (selected == note)
                {
                    NoteName.Text = selected.Name;
                    if (selected.IsIncome == true)
                    {
                        NoteSum.Text = selected.Amount.ToString();
                    }
                    else
                    {
                        NoteSum.Text = selected.Amount .ToString();
                    }
                    TypeComboBox.SelectedItem = selected.Type;
                    Notes.Remove(note);
                    BudgetItemsDataGrid.Items.Remove(selected);
                    BudgetItemsDataGrid.Items.Refresh();
                    Save(Notes, "C:/Users/alexa/OneDrive/Рабочий стол/Практическая/BudgetNotes.json");
                    TotalSumUpdate();
                    break;
                }
            }
        }
    }
}