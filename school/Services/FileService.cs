using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using school.Models;

namespace school.Services;

public static class FileService
{
    private static readonly string basePath;

    static FileService()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        DirectoryInfo? projectDir = null;

        while (dir != null)
        {
            if (dir.GetFiles("*.csproj").Any())
            {
                projectDir = dir;
                break;
            }

            dir = dir.Parent;
        }

        if (projectDir != null)
        {
            basePath = Path.GetFullPath(Path.Combine(projectDir.FullName, "RSC"));
        }
        else
        {
            basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "RSC"));
        }

        if (!Directory.Exists(basePath))
        {
            Directory.CreateDirectory(basePath);
        }
    }

    public static void SaveClass(SchoolClass schoolClass)
    {
        string path = Path.Combine(basePath, $"{schoolClass.ClassName}.txt");
        var lines = schoolClass.Students.Select(s => s.Name);
        File.WriteAllLines(path, lines, Encoding.UTF8);
    }

    public static SchoolClass LoadClass(string className)
    {
        string path = Path.Combine(basePath, $"{className}.txt");

        if (!File.Exists(path))
        {
            return new SchoolClass
            {
                ClassName = className,
                Students = new List<Student>()
            };
        }

        var students = File.ReadAllLines(path)
                           .Select(n => new Student { Name = n })
                           .ToList();

        return new SchoolClass
        {
            ClassName = className,
            Students = students
        };
    }

    public static List<string> GetAllClasses()
    {
        if (!Directory.Exists(basePath))
        {
            return new List<string>();
        }

        return Directory.GetFiles(basePath, "*.txt")
                        .Select(f => Path.GetFileNameWithoutExtension(f))
                        .ToList();
    }

    public static bool DeleteClass(string className)
    {
        string path = Path.Combine(basePath, $"{className}.txt");

        if (File.Exists(path))
        {
            File.Delete(path);
            return true;
        }

        return false;
    }
}