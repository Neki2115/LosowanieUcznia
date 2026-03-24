using System;
using Microsoft.Maui.Controls;
using school.Models;
using school.Services;

namespace school.Views;

public partial class EditPage : ContentPage
{
    private SchoolClass schoolClass;
    private string originalClassName;

    public EditPage(SchoolClass sc)
    {
        InitializeComponent();

        schoolClass = sc ?? throw new ArgumentNullException(nameof(sc));
        originalClassName = schoolClass.ClassName;

        classNameEntry.Text = schoolClass.ClassName;
        studentsList.ItemsSource = schoolClass.Students;
    }

    private void OnAdd(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(nameEntry.Text))
            return;

        schoolClass.Students.Add(new Student { Name = nameEntry.Text.Trim() });
        nameEntry.Text = string.Empty;
        RefreshList();
    }

    private void RefreshList()
    {
        studentsList.ItemsSource = null;
        studentsList.ItemsSource = schoolClass.Students;
    }

    private async void OnSave(object sender, EventArgs e)
    {
        var newName = (classNameEntry.Text ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(newName))
        {
            await DisplayAlert("Error", "Class name cannot be empty.", "OK");
            return;
        }

        if (!string.Equals(newName, originalClassName, StringComparison.Ordinal))
        {
            var all = FileService.GetAllClasses();
            if (all.Contains(newName))
            {
                await DisplayAlert("Error", "A class with that name already exists.", "OK");
                return;
            }
        }

        schoolClass.ClassName = newName;

        FileService.SaveClass(schoolClass);

        if (!string.Equals(newName, originalClassName, StringComparison.Ordinal))
        {
            FileService.DeleteClass(originalClassName);
            originalClassName = newName;
        }

        await DisplayAlert("OK", "Students saved.", "OK");
        await Navigation.PopAsync();
    }

    private void OnDeleteStudent(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is Student student)
        {
            schoolClass.Students.Remove(student);
            RefreshList();
        }
    }

    private async void OnEditStudent(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is Student student)
        {
            string result = await DisplayPromptAsync(
                "Edit student",
                "Enter new student name:",
                "OK",
                "Cancel",
                placeholder: student.Name);

            if (!string.IsNullOrWhiteSpace(result))
            {
                student.Name = result.Trim();
                RefreshList();
            }
        }
    }
}