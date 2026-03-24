using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;
using school.Services;
using school.Models;

namespace school.Views;

public partial class MainPage : ContentPage
{
    private SchoolClass currentClass;
    private List<string> classes = new();

    public MainPage()
    {
        InitializeComponent();
        LoadClasses();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadClasses();
    }

    private void LoadClasses()
    {
        classes = FileService.GetAllClasses();
        classesList.ItemsSource = classes;

        classesPanel.IsVisible = classes.Any();
        toggleClassesButton.Text = classesPanel.IsVisible ? "Hide classes" : "Show classes";
    }

    private async void OnAddClass(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(classEntry.Text))
        {
            await DisplayAlert("Error", "Please enter a class name.", "OK");
            return;
        }

        if (FileService.GetAllClasses().Contains(classEntry.Text))
        {
            await DisplayAlert("Error", "A class with that name already exists.", "OK");
            return;
        }

        var newClass = new SchoolClass { ClassName = classEntry.Text };

        FileService.SaveClass(newClass);

        currentClass = newClass;

        await DisplayAlert("OK", "Class added.", "OK");

        classEntry.Text = "";

        LoadClasses();

        await Navigation.PushAsync(new EditPage(currentClass));
    }

    private void OnToggleClasses(object sender, EventArgs e)
    {
        classesPanel.IsVisible = !classesPanel.IsVisible;
        toggleClassesButton.Text = classesPanel.IsVisible ? "Hide classes" : "Show classes";

        if (classesPanel.IsVisible)
        {
            LoadClasses();
        }
    }

    private void OnClassSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection != null && e.CurrentSelection.Count > 0 && e.CurrentSelection[0] is string name)
        {
            currentClass = FileService.LoadClass(name);
            resultLabel.Text = $"Loaded class {name}";
        }
        else
        {
            currentClass = null;
            resultLabel.Text = string.Empty;
        }
    }

    private void OnDrawStudent(object sender, EventArgs e)
    {
        if (currentClass == null)
        {
            resultLabel.Text = "Please select a class first.";
            return;
        }

        if (currentClass.Students == null || !currentClass.Students.Any())
        {
            resultLabel.Text = "No students in the class.";
            return;
        }

        var student = RandomService.DrawStudent(currentClass);
        resultLabel.Text = student?.Name ?? "No students";
    }

    private async void OnEditClass(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is string className)
        {
            var sc = FileService.LoadClass(className);
            await Navigation.PushAsync(new EditPage(sc));
            LoadClasses();
        }
    }

    private async void OnDeleteClass(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is string className)
        {
            bool confirm = await DisplayAlert("Delete", $"Delete class '{className}'? This action cannot be undone.", "Delete", "Cancel");
            if (!confirm) return;

            try
            {
                bool ok = FileService.DeleteClass(className);

                if (ok)
                {
                    if (currentClass != null && currentClass.ClassName == className)
                    {
                        currentClass = null;
                        resultLabel.Text = string.Empty;
                    }

                    await DisplayAlert("OK", $"Class '{className}' deleted.", "OK");
                    LoadClasses();
                }
                else
                {
                    await DisplayAlert("Error", "Class file not found.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to delete class: {ex.Message}", "OK");
            }
        }
    }

    private async void OnEdit(object sender, EventArgs e)
    {
        if (currentClass == null)
        {
            await DisplayAlert("Error", "Please select a class first.", "OK");
            return;
        }

        await Navigation.PushAsync(new EditPage(currentClass));
    }
}