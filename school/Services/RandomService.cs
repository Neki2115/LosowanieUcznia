using System;
using System.Linq;
using school.Models;

namespace school.Services;

public static class RandomService
{
    private static readonly Random rand = new();

    public static Student DrawStudent(SchoolClass schoolClass)
    {
        if (schoolClass == null || schoolClass.Students == null || schoolClass.Students.Count == 0)
        {
            return null;
        }

        int index = rand.Next(schoolClass.Students.Count);
        return schoolClass.Students[index];
    }
}